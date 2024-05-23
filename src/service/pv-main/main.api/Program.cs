using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenRIMS.PV.Main.API;
using OpenRIMS.PV.Main.API.Infrastructure;
using OpenRIMS.PV.Main.API.Infrastructure.Settings;
using OpenRIMS.PV.Main.Infrastructure;
using OpenRIMS.PV.Main.Infrastructure.Identity;
using Serilog;
using System;
using System.IO;
using System.Net;

namespace OpenRIMS.PV.Main.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = BuildWebHost(configuration, args);

                var providerSettings = configuration.GetSection(nameof(DatabaseProviderSettings));

                Log.Information("Applying identity migrations ({ApplicationContext})...", AppName);
                host.MigrateDbContext<IdentityDbContext>((context, services) =>
                {
                    var env = services.GetService<IWebHostEnvironment>();

                    var logger = services.GetService<ILogger<IdentityContextSeed>>();

                    new IdentityContextSeed()
                        .SeedAsync(context, env, providerSettings.GetValue<bool>(key: "SeedData"), logger)
                        .Wait();
                });

                Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                host.MigrateDbContext<MainDbContext>((context, services) =>
                {
                    var env = services.GetService<IWebHostEnvironment>();

                    var logger = services.GetService<ILogger<MainContextSeed>>();

                    new MainContextSeed()
                        .SeedAsync(
                            context, 
                            env, 
                            providerSettings.GetValue<bool>(key: "SeedData"),
                            providerSettings.GetValue<bool>(key: "SeedTestData"), 
                            logger
                        )
                        .Wait();
                });

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .ConfigureKestrel(options =>
                {
                    var ports = GetDefinedPorts(configuration);
                    options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    });

                    options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });

                })
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSerilog()
                .Build();

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var grpcPort = config.GetValue("GRPC_PORT", 5001);
            var port = config.GetValue("PORT", 5000);
            return (port, grpcPort);
        }
    }
}
