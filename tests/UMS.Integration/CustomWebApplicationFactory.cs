using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UMS.Contracts;
using UMS.Integration.Helpers;
using UMS.Persistence;

namespace UMS.Integration;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public ICache FakeCache { get; }
    public string DefaultUserId { get; set; } = "1";

    public CustomWebApplicationFactory()
    {
        FakeCache = A.Fake<ICache>();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<UserDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContextFactory<UserDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
            
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<UserDbContext>();

                db.Database.EnsureCreated();
                
                Utilities.SeedTestData(db);
            }
            
            // Remove any existing registration for ICache
            var existingCacheDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ICache));
            
            if (existingCacheDescriptor != null)
            {
                services.Remove(existingCacheDescriptor);
            }
        
            // Add the mock ICache to the service container
            services.AddSingleton<ICache>(FakeCache);
        });
        
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(FakeCache);
            
            services.Configure<TestAuthHandlerOptions>(options => options.DefaultUserId = DefaultUserId);
            
            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
        });
    }
}