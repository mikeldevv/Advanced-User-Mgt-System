using UMS.Contracts;
using UMS.Persistence;

namespace UMS.Integration.Spec.Helpers;

public static class Utilities
{
    public static void SeedTestData(UserDbContext db)
    {
        var users = new[]
        {
            new User
            {
                FirstName = "Jane",
                LastName = "Doe",
                EmailAddress = "janedoe@example.com",
                PasswordHash =  BCrypt.Net.BCrypt.HashPassword("jane123"),
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FirstName = "mike",
                LastName = "mill",
                EmailAddress = "mikemill@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("mike123"),
                CreatedAt = DateTime.UtcNow
            }
        };

        db.Users.AddRange(users);
        db.SaveChanges();
    }
}