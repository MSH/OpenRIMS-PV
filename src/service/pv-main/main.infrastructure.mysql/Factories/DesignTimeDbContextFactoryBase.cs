using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace OpenRIMS.PV.Main.Infrastructure.MySQL.Factories
{
    public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : DbContext
    {
        public TContext CreateDbContext(string[] args) =>
            Create(Directory.GetCurrentDirectory(), Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

        protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

        public TContext Create()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var basePath = AppContext.BaseDirectory;
            return Create(basePath, environmentName);
        }

        private TContext Create(string basePath, string environmentName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var connstr = config["ConnectionString"];

            if (string.IsNullOrWhiteSpace(connstr))
            {
                throw new InvalidOperationException(
                    "Could not find a connection string named 'ConnectionString'.");
            }
            return Create(connstr);
        }

        private TContext Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"{nameof(connectionString)} is null or empty.",
             nameof(connectionString));

            var serverVersion = new MySqlServerVersion(new Version(5, 7, 29));


            var optionsBuilder = new DbContextOptionsBuilder<TContext>();

            Console.WriteLine($"DesignTimeDbContextFactory.Create(string): Connection string: {connectionString}");

            optionsBuilder.UseMySql(
                           connectionString,
                           serverVersion,
                           mySqlOptions =>
                           {
                               mySqlOptions.MigrationsAssembly(typeof(MainDbContext).GetTypeInfo().Assembly.GetName().Name);
                               //mySqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                           });

            var options = optionsBuilder.Options;
            return CreateNewInstance(options);
        }
    }
}
