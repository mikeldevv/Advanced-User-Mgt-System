using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UMS.Api.Dtos;
using UMS.Contracts;
using Xunit;

namespace UMS.Integration.IntegrationTests;

public class OtpControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OtpControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task SendOtpViaEmail_WithValidData_Returns200Ok()
    {
        // Arrange
        SendOtpRequestDto sendOtpRequestDto = new SendOtpRequestDto
        {
            EmailAddress = "test@example.com",
            Purpose = OtpPurpose.Registration
        };

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/otp/sendOtpViaEmail", sendOtpRequestDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}