using Microsoft.EntityFrameworkCore;
using UMS.Contracts;
using UMS.Contracts.InvoicingApp.Poco;

namespace UMS.Persistence;

public class UserDbContext : DbContext
{
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.EmailAddress)
                .IsUnique();
        }
}