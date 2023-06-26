using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using PVIMS.Infrastructure.Identity.Entities;
using PVIMS.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure
{
    public class IdentityContextSeed
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

        public async Task SeedAsync(IdentityDbContext context, 
            IWebHostEnvironment env, 
            IOptions<PVIMSSettings> settings, 
            ILogger<IdentityContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(IdentityContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var seedData = settings.Value.SeedData;
                var contentRootPath = env.ContentRootPath;

                if (seedData)
                {
                    using (context)
                    {
                        context.Database.Migrate();

                        await CreateRolesAsync(context);
                        await CreateAdminUserAsync(context);
                    }
                }
            });
        }

        private async Task CreateRolesAsync(IdentityDbContext context)
        {
            List<IdentityRole<Guid>> roles = new List<IdentityRole<Guid>>();

            if (!context.Roles.Any(ce => ce.Name == "Admin"))
                roles.Add(new IdentityRole<Guid> { Name = "Admin", NormalizedName = "ADMIN" } );

            if (!context.Roles.Any(ce => ce.Name == "RegClerk"))
                roles.Add(new IdentityRole<Guid> { Name = "RegClerk", NormalizedName = "REGCLERK" });

            if (!context.Roles.Any(ce => ce.Name == "DataCap"))
                roles.Add(new IdentityRole<Guid> { Name = "DataCap", NormalizedName = "DATACAP" });

            if (!context.Roles.Any(ce => ce.Name == "Clinician"))
                roles.Add(new IdentityRole<Guid> { Name = "Clinician", NormalizedName = "CLINICIAN" });

            if (!context.Roles.Any(ce => ce.Name == "Analyst"))
                roles.Add(new IdentityRole<Guid> { Name = "Analyst", NormalizedName = "ANALYST" });

            if (!context.Roles.Any(ce => ce.Name == "Reporter"))
                roles.Add(new IdentityRole<Guid> { Name = "Reporter", NormalizedName = "REPORTER" });

            if (!context.Roles.Any(ce => ce.Name == "Publisher"))
                roles.Add(new IdentityRole<Guid> { Name = "Publisher", NormalizedName = "PUBLISHER" });

            if (!context.Roles.Any(ce => ce.Name == "ReporterAdmin"))
                roles.Add(new IdentityRole<Guid> { Name = "ReporterAdmin", NormalizedName = "REPORTERADMIN" });

            if (!context.Roles.Any(ce => ce.Name == "PublisherAdmin"))
                roles.Add(new IdentityRole<Guid> { Name = "PublisherAdmin", NormalizedName = "PUBLISHERADMIN" });

            if (!context.Roles.Any(ce => ce.Name == "ClientApp"))
                roles.Add(new IdentityRole<Guid> { Name = "ClientApp", NormalizedName = "CLIENTAPP" });

            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();
        }

        private async Task CreateAdminUserAsync(IdentityDbContext context)
        {
            if (!context.Users.Any(ce => ce.UserName == "Admin"))
            {
                var adminUser = new ApplicationUser
                {
                    Email = "admin@mail.com",
                    Id = Guid.NewGuid(),
                    LastName = "User",
                    FirstName = "Admin",
                    PhoneNumber = "",
                    UserName = "Admin",
                    NormalizedEmail = "ADMIN@MAIL.COM",
                    NormalizedUserName = "ADMIN",
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    Active = true
                };

                adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, "P@55w0rd1");

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();

                List<IdentityUserRole<Guid>> userRoles = new List<IdentityUserRole<Guid>>();
                userRoles.Add(new IdentityUserRole<Guid> { RoleId = context.Roles.SingleOrDefault(r => r.Name == "Admin").Id, UserId = adminUser.Id });
                context.UserRoles.AddRange(userRoles);

                await context.SaveChangesAsync();
            }
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<IdentityContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
