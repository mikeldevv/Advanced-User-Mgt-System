using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UMS.Api.Dtos;
using Xunit;

namespace UMS.Integration.Spec.Steps;

[Binding]
public sealed class AuthenticationControllerSteps
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private HttpResponseMessage _response;

    public AuthenticationControllerSteps(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
    
    [When(@"I log in with email ""(.*)"" and password ""(.*)""")]
    public async Task WhenILogInWithEmailAndPassword(string email, string password)
    {
        LoginUserRequestDto loginUserRequestDto = new LoginUserRequestDto
        {
            EmailAddress = email,
            Password = password
        };

        _response = await _client.PostAsJsonAsync("/api/authentication/login", loginUserRequestDto);
    }
    
    [Then(@"I should receive a (.*) response")]
    public async Task ThenIShouldReceiveAResponse(int statusCode)
    {
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
        AuthCredentialsDto? authCredentialsDto = await _response.Content.ReadFromJsonAsync<AuthCredentialsDto>();
        Assert.NotNull(authCredentialsDto);
    }
    
    [Then(@"I should receive a 400 Bad Request response containing ""(.*)""")]
    public async Task ThenIShouldReceiveABadRequestResponseContaining(string expectedContent)
    {
        string responseContent = await _response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
        Assert.Contains(expectedContent, responseContent);
    }
}