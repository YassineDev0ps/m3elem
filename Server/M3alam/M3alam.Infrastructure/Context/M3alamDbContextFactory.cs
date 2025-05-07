using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace M3alam.Infrastructure.Context
{
    public class AppM3alamDbContextFactoryextFactory : IDesignTimeDbContextFactory<M3alamDbContext>
    {
        public M3alamDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<M3alamDbContext>();
            
            // Get connection string from appsettings.json (or environment variables)
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new M3alamDbContext(optionsBuilder.Options);
        }
    }
}
