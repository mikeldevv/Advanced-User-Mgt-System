namespace UMS.Contracts;

public interface IUserService
{
    Task<User> CreateUser(string firstName, string lastName, string emailAddress, string passwordHash);
    Task<User?> GetUserByEmailAddress(string emailAddress);
    Task<User?> GetUserById(long userId);
    Task<bool> ValidateOtpForUser(long userId, string otp);
    Task<bool> ValidateOtpForRegisterUser(string emailAddress, string otp);
    Task ChangePassword(long userId, string newPasswordHash);
}