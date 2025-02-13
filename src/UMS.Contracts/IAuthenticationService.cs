namespace UMS.Contracts;

public interface IAuthenticationService
{
    Task<string?> GetAccessToken(string emailAddress, string password);
    string CreatePasswordHash(string password);
    Task<bool> VerifyAndChangePassword(long userId, string oldPassword, string newPassword);
}