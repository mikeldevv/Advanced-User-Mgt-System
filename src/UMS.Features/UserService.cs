using Microsoft.EntityFrameworkCore;
using UMS.Contracts;
using UMS.Persistence;

namespace UMS.Features;

public class UserService : IUserService
{
    private readonly UserDbContext _dbContext;
    private readonly IOtpService _otpService;

    public UserService(UserDbContext dbContext, IOtpService otpService)
    {
        _dbContext = dbContext;
        _otpService = otpService;
    }

    public async Task<User> CreateUser(string firstName, string lastName, string emailAddress, string passwordHash)
    {
        var newUser = new User
        {
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = emailAddress,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return newUser;
    }

    public Task<User?> GetUserByEmailAddress(string emailAddress)
    {
        return _dbContext.Users.FirstOrDefaultAsync(x => x.EmailAddress == emailAddress);
    }

    public Task<User?> GetUserById(long userId)
    {
        return _dbContext.Users.SingleOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<bool> ValidateOtpForRegisterUser(string emailAddress, string otp)
    {
        return await _otpService.Exists(emailAddress, otp);
    }

    public async Task ChangePassword(long userId, string newPasswordHash)
    {
        User? user = await GetUserById(userId);

        if (user == null)
        {
            return;
        }

        user.PasswordHash = newPasswordHash;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<bool> ValidateOtpForUser(long userId, string otp)
    {
        User? user = await GetUserById(userId);

        if (user == null)
        {
            return false;
        }

        return await _otpService.Exists(user.EmailAddress, otp);
    }
}