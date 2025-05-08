

using M3alam.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace M3alam.Infrastructure.Context
{
    public class M3alamDbContext : DbContext

    {
        public M3alamDbContext(DbContextOptions<M3alamDbContext> options)
          : base(options)
        { }
        DbSet<Message> Messages { get; set; }
        DbSet<Provider> Providers { get; set; }
        DbSet<Rating> Rating { get; set; }
        DbSet<Request> Requests { get; set; }
        DbSet<Seeker> Seekers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
          
        }
    }
}
