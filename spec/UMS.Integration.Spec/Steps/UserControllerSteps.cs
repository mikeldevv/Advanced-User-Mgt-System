using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using UMS.Api.Dtos;
using UMS.Contracts;
using UMS.Integration.Spec.Helpers;
using UMS.Persistence;
using Xunit;

namespace UMS.Integration.Spec.Steps;

[Binding]
public sealed class UserControllerSteps
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private HttpResponseMessage _response;

    public UserControllerSteps(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
    }

    [Given(@"a user is authenticated with ID ""(.*)""")]
    public void GivenAUserIsAuthenticatedWithId(string userId)
    {
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, userId);
    }

    [Given(@"a user is unauthenticated")]
    public void GivenAUserIsUnauthenticated()
    {
        _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, "");
    }

    [Given(@"the cache exists with key ""(.*)""")]
    public void GivenTheCacheExistsWithKey(string cacheKey)
    {
        A.CallTo(() => _factory.FakeCache.Exists(A<string>._)).Returns((true, TimeSpan.FromMinutes(10)));
    }

    [Given(@"the cache contains value ""(.*)""")]
    public void GivenTheCacheContainsValue(string cacheValue)
    {
        A.CallTo(() => _factory.FakeCache.Read(A<string>._)).Returns((cacheValue, TimeSpan.FromMinutes(10)));
    }

    [When(@"a user registers with valid data")]
    public void WhenAUserRegistersWithValidData()
    {
        RegisterUserRequestDto registerUserRequestDto = new RegisterUserRequestDto
        {
            FirstName = "John",
            LastName = "Smith",
            EmailAddress = "johnsmith@example.com",
            Otp = "123456",
            Password = "Password@123"
        };

        _response = _client.PostAsJsonAsync("/api/user/register", registerUserRequestDto).Result;
    }

    [When(@"a user registers with existing email")]
    public void WhenAUserRegistersWithExistingEmail()
    {
        RegisterUserRequestDto registerUserRequestDto = new RegisterUserRequestDto
        {
            FirstName = "John",
            LastName = "Smith",
            EmailAddress = "mikemill@example.com",
            Otp = "123456",
            Password = "Password@123"
        };
        _response = _client.PostAsJsonAsync("/api/user/register", registerUserRequestDto).Result;
    }

    [When(@"a user registers with invalid OTP")]
    public void WhenAUserRegistersWithInvalidOtp()
    {
        RegisterUserRequestDto registerUserRequestDto = new RegisterUserRequestDto
        {
            FirstName = "John",
            LastName = "Smith",
            EmailAddress = "newjohnsmith@example.com",
            Otp = "123",
            Password = "Password@123"
        };
        _response = _client.PostAsJsonAsync("/api/user/register", registerUserRequestDto).Result;
    }

    [When(@"the user accesses the WhoAmI endpoint")]
    public void WhenTheUserAccessesTheWhoAmIEndpoint()
    {
        _response = _client.GetAsync("/api/user/whoami").Result;
    }

    [When(@"the user resets the password with valid data")]
    public async void WhenTheUserResetsThePasswordWithValidData()
    {
        ResetPasswordRequestDto resetPasswordRequestDto = new ResetPasswordRequestDto
        {
            Otp = "123456",
            NewPassword = "mike1234"
        };
        _response =  _client.PostAsJsonAsync("/api/user/resetpassword", resetPasswordRequestDto).Result;
    }
    
    [When(@"the user resets the password with invalid OTP")]
    public void WhenTheUserResetsThePasswordWithInvalidOtp()
    {
        ResetPasswordRequestDto resetPasswordRequestDto = new ResetPasswordRequestDto
        {
            Otp = "123",
            NewPassword = "mike1234"
        };
        _response =  _client.PostAsJsonAsync("/api/user/resetpassword", resetPasswordRequestDto).Result;
    }
    
    [When(@"the user attempts to reset the password")]
    public void WhenTheUserAttemptsToResetThePassword()
    {
        ResetPasswordRequestDto resetPasswordRequestDto = new ResetPasswordRequestDto
        {
            Otp = "123456",
            NewPassword = "mike1234"
        };
        _response =  _client.PostAsJsonAsync("/api/user/resetpassword", resetPasswordRequestDto).Result;
    }
    
    [When(@"the user changes the password with valid data")]
    public void WhenTheUserChangesThePasswordWithValidData()
    {
        ChangePasswordRequestDto changePasswordRequestDto = new ChangePasswordRequestDto
        {
            OldPassword = "mike123",
            NewPassword = "mike1234"
        }; 
        _response =  _client.PostAsJsonAsync("/api/user/changepassword", changePasswordRequestDto).Result;
    }
    
    [When(@"the user attempts to change the password")]
    public void WhenTheUserAttemptsToChangeThePassword()
    {
        ChangePasswordRequestDto changePasswordRequestDto = new ChangePasswordRequestDto
        {
            OldPassword = "mike123",
            NewPassword = "mike1234"
        }; 
        _response =  _client.PostAsJsonAsync("/api/user/changepassword", changePasswordRequestDto).Result;
    }
    
    [When(@"the user changes the password with invalid old password")]
    public void WhenTheUserChangesThePasswordWithInvalidOldPassword()
    {
        ChangePasswordRequestDto changePasswordRequestDto = new ChangePasswordRequestDto
        {
            OldPassword = "invalid-password",
            NewPassword = "mike1234"
        }; 
        _response =  _client.PostAsJsonAsync("/api/user/changepassword", changePasswordRequestDto).Result;
    }

    [Then(@"the response should contain user data:")]
    public void ThenTheResponseShouldContainUserData(Table table)
    {
        var whoAmIDto = _response.Content.ReadFromJsonAsync<WhoAmIDto>().Result;
        table.CompareToInstance(whoAmIDto);
    }

    [Then(@"the response status code should be (.*) Created")]
    public void ThenTheResponseStatusCodeShouldBeCreated(int statusCode)
    {
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
    }

    [Then(@"the response status code should be (.*) BadRequest")]
    public void ThenTheResponseStatusCodeShouldBeBadRequest(int statusCode)
    {
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
    }

    [Then(@"the response status code should be (.*) Unauthorized")]
    public void ThenTheResponseStatusCodeShouldBeUnauthorized(int statusCode)
    {
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
    }

    [Then(@"the WhoAmI response status code should be (.*) OK")]
    public void ThenTheWhoAmIResponseStatusCodeShouldBeOk(int expectedStatusCode)
    {
        Assert.Equal((HttpStatusCode)expectedStatusCode, _response.StatusCode);
    }
    
    [Then(@"the reset password response status code should be (.*) OK")]
    public void ThenTheResetPasswordResponseStatusCodeShouldBeOk(int expectedStatusCode)
    {
        Assert.Equal((HttpStatusCode)expectedStatusCode, _response.StatusCode);
    }
    
    [Then(@"the change password response status code should be (.*) OK")]
    public void ThenTheChangePasswordResponseStatusCodeShouldBeOk(int expectedStatusCode)
    {
        Assert.Equal((HttpStatusCode)expectedStatusCode, _response.StatusCode);
    }

    [Then(@"the response should contain ""(.*)""")]
    public void ThenTheResponseShouldContain(string expectedContent)
    {
        var responseContent = _response.Content.ReadAsStringAsync().Result;
        Assert.Contains(expectedContent, responseContent);
    }
}