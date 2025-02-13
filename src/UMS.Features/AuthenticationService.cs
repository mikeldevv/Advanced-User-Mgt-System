using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UMS.Contracts;

namespace UMS.Features;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly SigningCredentials _jwtSigningCredentials;

    public AuthenticationService(IConfiguration configuration, IUserService userService
    )
    {
        _userService = userService;
        _jwtSigningCredentials =
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecret"]!)),
                SecurityAlgorithms.HmacSha512Signature);
    }

    public async Task<string?> GetAccessToken(string emailAddress, string password)
    {
        User? user = await _userService.GetUserByEmailAddress(emailAddress);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        return CreateJwtToken(user);
    }

    public async Task<bool> VerifyAndChangePassword(long userId, string oldPassword, string newPassword)
    {
        User? user = await _userService.GetUserById(userId);

        if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
        {
            return false;
        }

        await _userService.ChangePassword(userId, CreatePasswordHash(newPassword));

        return true;
    }

    public string CreatePasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private string CreateJwtToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new("sub", user.UserId.ToString())
        };

        JwtSecurityToken jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: _jwtSigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}