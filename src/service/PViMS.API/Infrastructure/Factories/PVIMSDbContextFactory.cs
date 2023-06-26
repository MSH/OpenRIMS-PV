using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PVIMS.Infrastructure;
using System.IO;

namespace PVIMS.API.Infrastructure.Factories
{
    public class PVIMSDbContextFactory : IDesignTimeDbContextFactory<PVIMSDbContext>
    {
        public PVIMSDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PVIMSDbContext>();

            optionsBuilder.UseSqlServer(config["ConnectionString"], sqlServerOptionsAction: o => o.MigrationsAssembly("PViMS.Infrastructure"));

            return new PVIMSDbContext(optionsBuilder.Options);
        }
    }
}
