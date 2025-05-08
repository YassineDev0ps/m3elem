
using Microsoft.EntityFrameworkCore;
using M3alam.Domain.Entities;

namespace M3alam.Infrastructure.Context
{
    public class M3alamDbContext : DbContext
    {
        public M3alamDbContext(DbContextOptions<M3alamDbContext> options)
            : base(options) { }

        // DbSet for Users
        public DbSet<User> Users { get; set; }

        // DbSet for Providers
        public DbSet<Provider> Providers { get; set; }
    }
}
