    using System.Net;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using FakeItEasy;
    using Microsoft.AspNetCore.Mvc.Testing;
    using UMS.Api.Dtos;
    using UMS.Integration.Helpers;
    using Xunit;

    namespace UMS.Integration.IntegrationTests;

    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public UserControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        }

        [Fact]
        public async Task RegisterUser_WithValidData_Returns201Created()
        {
            // Arrange
            A.CallTo(() => _factory.FakeCache.Exists(A<string>._))
                .Returns((true, TimeSpan.FromMinutes(10)));
            A.CallTo(() => _factory.FakeCache.Write(A<string>._, A<string>._, A<bool>._, A<TimeSpan?>._))
                .DoesNothing();
            A.CallTo(() => _factory.FakeCache.Read(A<string>._))
                .Returns(("123456", TimeSpan.FromMinutes(10)));
            
              
            RegisterUserRequestDto registerUserRequestDto = new RegisterUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "test1@example.com",
                Otp = "123456",
                Password = "Password@123"
            };
            
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/user/register", registerUserRequestDto);
            
            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_WithExistingEmail_Returns400BadRequest()
        {
            // Arrange
            RegisterUserRequestDto registerUserRequestDto = new RegisterUserRequestDto
            {
                FirstName = "John",
                LastName = "Smith",
                EmailAddress = "johnsmith@example.com",
                Otp = "123456",
                Password = "Password@123"
            };
            
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/user/register", registerUserRequestDto);
            
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("User with Email 'johnsmith@example.com' already exists", await response.Content.ReadAsStringAsync());
        }
        
        [Fact]
        public async Task RegisterUser_WithInvalidOtp_Returns400BadRequest()
        {
            // Arrange
            RegisterUserRequestDto registerUserRequestDto = new RegisterUserRequestDto
            {
                FirstName = "John",
                LastName = "Smith",
                EmailAddress = "newjohnsmith@example.com",
                Otp = "123",
                Password = "Password@123"
            };
            
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/user/register", registerUserRequestDto);
            
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Invalid OTP.", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhoAmI_AuthenticatedUser_ReturnsOkWithUserData()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, "1");
        
            // Act
            HttpResponseMessage response = await _client.GetAsync("/api/user/whoami");
        
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            WhoAmIDto? whoAmIDto = await response.Content.ReadFromJsonAsync<WhoAmIDto>();
            
            Assert.Equal("Jane", whoAmIDto?.FirstName);
            Assert.Equal("Doe", whoAmIDto?.LastName);
            Assert.Equal("janedoe@example.com", whoAmIDto?.EmailAddress);
        }
        
        [Fact]
        public async Task WhoAmI_UnAuthenticatedUser_Returns401UnAuthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, "0");
        
            // Act
            HttpResponseMessage response = await _client.GetAsync("/api/user/whoami");
        
            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async Task ResetPassword_AuthenticatedUser_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, "1");
            
            ResetPasswordRequestDto resetPasswordRequestDto = new ResetPasswordRequestDto
            {
                Otp = "123456",
                NewPassword = "jane1234"
            };
            
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/user/resetpassword", resetPasswordRequestDto);
            
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task ResetPassword_AuthenticatedUserWithInvalidOtp_Returns400BadRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, "1");
            
            ResetPasswordRequestDto resetPasswordRequestDto = new ResetPasswordRequestDto
            {
                Otp = "123",
                NewPassword = "jane1234"
            };
            
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/user/resetpassword", resetPasswordRequestDto);
            
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task ResetPassword_UnAuthenticatedUser_Returns401UnAuthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, "User1");
            ResetPasswordRequestDto resetPasswordRequestDto = new ResetPasswordRequestDto
            {
                Otp = "123456",
                NewPassword = "some new password"
            };
            
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/user/resetpassword", resetPasswordRequestDto);
            
            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_AuthenticatedUser_ReturnsOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, "1");

            ChangePasswordRequestDto changePasswordRequestDto = new ChangePasswordRequestDto
            {
                OldPassword = "jane1234",
                NewPassword = "jane12345"
            };
            
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/user/changepassword", changePasswordRequestDto);
            
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task ChangePassword_UnAuthenticatedUser_Returns401UnAuthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, "User1");

            ChangePasswordRequestDto changePasswordRequestDto = new ChangePasswordRequestDto
            {
                OldPassword = "jane1234",
                NewPassword = "jane12345"
            };
            
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/user/changepassword", changePasswordRequestDto);
            
            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [Fact]
        public async Task ChangePassword_AuthenticatedUserWithInvalidPassword_Returns400BadRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add(TestAuthHandler.UserId, "2");

            ChangePasswordRequestDto changePasswordRequestDto = new ChangePasswordRequestDto
            {
                OldPassword = "john1234",
                NewPassword = "john12345"
            };
            
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/user/changepassword", changePasswordRequestDto);
            
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Invalid password.", await response.Content.ReadAsStringAsync());
        }
    }