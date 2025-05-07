
// M3alam.Infrastructure/Context/M3alamDbContext.cs
using Microsoft.EntityFrameworkCore;
using M3alam.Domain.Entities;  // Make sure to include your entities

namespace M3alam.Infrastructure.Context
{
    public class M3alamDbContext : DbContext
    {
        public M3alamDbContext(DbContextOptions<M3alamDbContext> options)
            : base(options)
        { }

        // DbSet for Users
        public DbSet<User> Users { get; set; }

        // DbSet for Providers
        public DbSet<Provider> Providers { get; set; }

        // Add other DbSets for additional entities as needed

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Additional configuration for entities can go here
            // For example, setting the max length for a column or configuring relationships
            builder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(150);  // Example configuration for Email column
        }
    }
}

