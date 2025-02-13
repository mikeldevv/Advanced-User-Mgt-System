using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TechTalk.SpecFlow.Assist;
using UMS.Api.Dtos;
using Xunit;

namespace UMS.Integration.Spec.Steps;

[Binding]
public sealed class OtpControllerSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private HttpResponseMessage _response;

    public OtpControllerSteps(ScenarioContext scenarioContext, CustomWebApplicationFactory<Program> factory)
    {
        _scenarioContext = scenarioContext;
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
    
    [Given(@"the following request data:")]
    public void GivenTheFollowingRequestData(Table table)
    {
        var requestData = table.CreateInstance<SendOtpRequestDto>();
        _scenarioContext.Set(requestData, "RequestData");
    }
    
    [When(@"I send the OTP via email")]
    public async Task WhenISendTheOtpViaEmail()
    {
        var requestData = _scenarioContext.Get<SendOtpRequestDto>("RequestData");

        _response = await _client.PostAsJsonAsync("/api/otp/sendOtpViaEmail", requestData);
    }
    
    [Then(@"the response status code should be (.*) OK")]
    public void ThenTheResponseStatusCodeShouldBeOk(int expectedStatusCode)
    {
        Assert.Equal((HttpStatusCode)expectedStatusCode, _response.StatusCode);
    }
}