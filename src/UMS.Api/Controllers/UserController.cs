using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UMS.Api.Dtos;
using UMS.Api.Dtos.User;
using UMS.Contracts;

namespace UMS.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserController : AbstractController
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authenticationService;

    public UserController(IUserService userService, IAuthenticationService authenticationService)
    {
        _userService = userService;
        _authenticationService = authenticationService;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserRequestDto registerUserRequestDto)
    {
        User? user = await _userService.GetUserByEmailAddress(registerUserRequestDto.EmailAddress);

        if (user is not null)
        {
            return BadRequest($"User with Email '{registerUserRequestDto.EmailAddress}' already exists");
        }
        
        bool isValidOtp = await _userService.ValidateOtpForRegisterUser(registerUserRequestDto.EmailAddress, registerUserRequestDto.Otp);
        if (!isValidOtp)
        {
            return BadRequest("Invalid OTP.");
        }

        var passwordHash = _authenticationService.CreatePasswordHash(registerUserRequestDto.Password);
        User createdUser = await _userService.CreateUser(
            registerUserRequestDto.FirstName,
            registerUserRequestDto.LastName,
            registerUserRequestDto.EmailAddress,
            passwordHash);
        
        var userDto = new UserDto
        {
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            EmailAddress = createdUser.EmailAddress,
        };
        
        
        return CreatedAtAction(nameof(Register), userDto);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<WhoAmIDto>> WhoAmI()
    {
        if (ContextUserId.HasValue)
        {
            User? userFromDb = await _userService.GetUserById(ContextUserId.Value);

            if (userFromDb is not null)
            {
                WhoAmIDto whoAmIDto = new WhoAmIDto
                {
                    FirstName = userFromDb.FirstName,
                    LastName = userFromDb.LastName,
                    EmailAddress = userFromDb.EmailAddress
                };

                return Ok(whoAmIDto);
            }
        }

        return Unauthorized();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto resetPasswordRequestDto)
    {
        if (ContextUserId is not null)
        {
            bool isOtpValid = await _userService.ValidateOtpForUser(ContextUserId.Value, resetPasswordRequestDto.Otp);

            if (isOtpValid)
            {
                await _userService.ChangePassword(ContextUserId.Value,
                    _authenticationService.CreatePasswordHash(resetPasswordRequestDto.NewPassword));

                return Ok();
            }

            return BadRequest("Invalid OTP.");
        }

        return Unauthorized();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto changePasswordRequestDto)
    {
        if (ContextUserId is not null)
        {
            bool isValidPassword = await _authenticationService.VerifyAndChangePassword(ContextUserId.Value,
                changePasswordRequestDto.OldPassword, changePasswordRequestDto.NewPassword);

            if (isValidPassword)
            {
                return Ok();
            }
            
            return BadRequest("Invalid password.");
            
        }

        return Unauthorized();
    }
}