using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using UMS.Api.Dtos;
using IAuthenticationService = UMS.Contracts.IAuthenticationService;

namespace UMS.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthenticationController : AbstractController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost]
    public async Task<ActionResult<AuthCredentialsDto>> Login(LoginUserRequestDto loginUserRequestDto)
    {
        string? jwt =
            await _authenticationService.GetAccessToken(loginUserRequestDto.EmailAddress, loginUserRequestDto.Password);

        if (jwt is null)
        {
            return BadRequest("Invalid Email Address or Password");
        }

        return Ok(new AuthCredentialsDto(jwt));
    }

    [HttpGet("/signin-google")]
    public async Task<IActionResult> GoogleCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync("Google");
        if (authenticateResult.Succeeded)
        {
            var jwt = await HttpContext.GetTokenAsync("access_token");
            if (jwt is null)
            {
                return BadRequest();
            }

            return Ok(new AuthCredentialsDto(jwt));
        }
        
        return RedirectToAction("Login", "Authentication");
    }

}