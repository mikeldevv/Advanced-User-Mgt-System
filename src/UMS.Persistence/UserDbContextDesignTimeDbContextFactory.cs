using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UMS.Persistence;
public class UserDbContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().Load();
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        string? connectionString = config.GetConnectionString(nameof(UserDbContext));

        DbContextOptionsBuilder<UserDbContext> builder = new DbContextOptionsBuilder<UserDbContext>();

        builder.UseNpgsql(connectionString);

        return new UserDbContext(builder.Options);
    }
}