using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UMS.Api.Dtos;
using Xunit;

namespace UMS.Integration.IntegrationTests;

public class AuthenticationControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthenticationControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Login_WithValidData_Returns200Ok()
    {
        // Arrange
        LoginUserRequestDto loginUserRequestDto = new LoginUserRequestDto
        {
            EmailAddress = "johnsmith@example.com",
            Password = "john123"
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/authentication/login", loginUserRequestDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        AuthCredentialsDto? authCredentialsDto = await response.Content.ReadFromJsonAsync<AuthCredentialsDto>();

        Assert.NotNull(authCredentialsDto);
    }

    [Fact]
    public async Task Login_WithInvalidData_Returns400BadRequest()
    {
        // Arrange
        LoginUserRequestDto loginUserRequestDto = new LoginUserRequestDto
        {
            EmailAddress = "invalid@example.com",
            Password = "invalid"
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/authentication/login", loginUserRequestDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Invalid Email Address or Password", await response.Content.ReadAsStringAsync());
    }
}