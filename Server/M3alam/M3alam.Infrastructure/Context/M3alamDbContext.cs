

using Microsoft.EntityFrameworkCore;

namespace M3alam.Infrastructure.Context
{
    public class M3alamDbContext : DbContext

    {
        public M3alamDbContext(DbContextOptions<M3alamDbContext> options)
          : base(options)
        { }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
          
        }
    }
}
