using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace M3alam.Infrastructure.Context
{
 
    public class AppM3alamDbContextFactoryextFactory : IDesignTimeDbContextFactory<M3alamDbContext>
    {
        public M3alamDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<M3alamDbContext>();
            optionsBuilder.UseSqlServer("Server=tcp:m3alam-db.database.windows.net,1433;Initial Catalog=M3alam_Database;Persist Security Info=False;User ID=Agile_Root;Password=M3alamAdmin;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            return new M3alamDbContext(optionsBuilder.Options);
        }
    }
}
