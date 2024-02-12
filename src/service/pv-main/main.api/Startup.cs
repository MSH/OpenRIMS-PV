using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenRIMS.PV.Main.API;
using OpenRIMS.PV.Main.API.Application.IntegrationEvents;
using OpenRIMS.PV.Main.API.Application.IntegrationEvents.Events;
using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.API.Infrastructure.AutofacModules;
using OpenRIMS.PV.Main.API.Infrastructure.Configs.ExceptionHandler;
using OpenRIMS.PV.Main.API.Infrastructure.Configs.Settings;
using OpenRIMS.PV.Main.API.Infrastructure.Extensions;
using OpenRIMS.PV.Main.API.Infrastructure.OperationFilters;
using OpenRIMS.PV.Main.API.Infrastructure.Services;
using OpenRIMS.PV.Main.API.Infrastructure.Settings;
using OpenRIMS.PV.Main.API.Helpers;
using OpenRIMS.PV.BuildingBlocks.EventBus;
using OpenRIMS.PV.BuildingBlocks.EventBus.Abstractions;
using OpenRIMS.PV.BuildingBlocks.EventBusRabbitMQ;
using OpenRIMS.PV.BuildingBlocks.IntegrationEventLogEF;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.Services;
using OpenRIMS.PV.Main.Infrastructure;
using OpenRIMS.PV.Main.Infrastructure.Identity;
using OpenRIMS.PV.Main.Infrastructure.Identity.Entities;
using OpenRIMS.PV.Main.Infrastructure.Repositories;
using OpenRIMS.PV.Main.Services;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Internal;

namespace OpenRIMS.PV.Main.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomMvc()
                .AddCustomDbContext(Configuration)
                .AddCustomSwagger(Configuration)
                .AddCustomIntegrations(Configuration)
                .AddCustomConfiguration(Configuration)
                .AddEventBus(Configuration)
                .AddCustomAuthentication(Configuration)
                .AddDependencies(Configuration);

            // Create the container builder.
            var builder = new ContainerBuilder();

            builder.Populate(services);
            builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new ApplicationModule(Configuration.GetSection(nameof(DatabaseProviderSettings)).GetValue<string>(key: "ConnectionString")));

            this.ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An unexpected fault has occurred. Please try again later.");
                });
            });

            //throw new Exception("Error constructing handler for request of type MediatR.IRequestHandler`2[OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate.AddClinicalEventToPatientCommand,System.Boolean]. Register your handlers withthe container. See the samples in GitHub for examples.");

            //app.UseMiddleware<ExceptionMiddleware>();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
                RequestPath = "/StaticFiles"
            });

            app.UseSwagger()
                .UseSwaggerUI(setupAction =>
                {
                    setupAction.SwaggerEndpoint(
                        "/swagger/OpenAPISpecification/swagger.json",
                        "OpenRIMS.PV. Open API UI");

                    setupAction.RoutePrefix = "";
                    setupAction.DefaultModelExpandDepth(2);
                    setupAction.DefaultModelRendering(ModelRendering.Model);
                    setupAction.DocExpansion(DocExpansion.None);
                    setupAction.EnableDeepLinking();
                    setupAction.DisplayOperationId();

                });

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });

            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetService<IEventBus>();

            if(eventBus == null)
            {
                return;
            }

            eventBus.Subscribe<PatientAddedIntegrationEvent, IIntegrationEventHandler<PatientAddedIntegrationEvent>>();
        }
    }

    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                //setupAction.EnableEndpointRouting = false; // Use the routing logic of ASP.NET Core 2.1 or earlier:
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;

                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status403Forbidden));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status422UnprocessableEntity));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                options.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter());

                // Cater for custom json output media types
                var jsonOutputFormatter = options.OutputFormatters
                    .OfType<SystemTextJsonOutputFormatter>().FirstOrDefault();
                if (jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.identifier.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.detail.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.summary.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.expanded.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.search.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.groupvalue.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.commonmeddra.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.newreports.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.feedback.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.patientsummary.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.dataset.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.spontaneousdataset.v1+json");

                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.outstandingvisitreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.adverseventreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.quarterlyadverseventreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.annualadverseventreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.patienttreatmentreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.patientmedicationreport.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.causalityreport.v1+json");

                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.activitystatusconfirm.v1+json");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.analyserpatientset.v1+json");
                }

                // Cater for custom XML output media types
                var xmlOutputFormatter = options.OutputFormatters
                    .OfType<XmlDataContractSerializerOutputFormatter>().FirstOrDefault();
                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.identifier.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.detail.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.summary.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.expanded.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.search.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.groupvalue.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.commonmeddra.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.newreports.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.feedback.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.dataset.v1+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.spontaneousdataset.v1+xml");

                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.outstandingvisitreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.adverseventreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.quarterlyadverseventreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.annualadverseventreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.patienttreatmentreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.patientmedicationreport.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.causalityreport.v1+xml");

                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.activitystatusconfirm.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.analyserpatientset.v1+xml");

                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.auditlog.v1+xml");
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.main.attachment.v1+xml");
                }
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddXmlDataContractSerializerFormatters()
                .AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var providerSettings = configuration.GetSection(nameof(DatabaseProviderSettings));

            services.AddDbContext<MainDbContext>(options =>
                   {
                       if (providerSettings.GetValue<string>(key: "Type") == "MySQL")
                       {
                            options.UseMySql(
                                providerSettings.GetValue<string>(key: "ConnectionString"),
                                ServerVersion.AutoDetect(providerSettings.GetValue<string>(key: "ConnectionString")),
                                mySqlOptions =>
                                {
                                    mySqlOptions.MigrationsAssembly("OpenRIMS.PV.Main.Infrastructure.MySQL");
                                    mySqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                });
                       }

                       if (providerSettings.GetValue<string>(key: "Type") == "SQL")
                       {
                            options.UseSqlServer(
                                providerSettings.GetValue<string>(key: "ConnectionString"),
                                sqlServerOptionsAction: sqlOptions =>
                                {
                                    sqlOptions.MigrationsAssembly("OpenRIMS.PV.Main.Infrastructure.SQL");
                                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                });
                       }
                   },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );
            services.AddDbContext<IdentityDbContext>(options =>
                   {
                       if (providerSettings.GetValue<string>(key: "Type") == "MySQL")
                       {
                            options.UseMySql(
                                providerSettings.GetValue<string>(key: "ConnectionString"),
                                ServerVersion.AutoDetect(providerSettings.GetValue<string>(key: "ConnectionString")),
                                mySqlOptions =>
                                {
                                    mySqlOptions.MigrationsAssembly("OpenRIMS.PV.Main.Infrastructure.MySQL");
                                    mySqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                });
                       }

                       if (providerSettings.GetValue<string>(key: "Type") == "SQL")
                       {
                            options.UseSqlServer(
                                providerSettings.GetValue<string>(key: "ConnectionString"),
                                sqlServerOptionsAction: sqlOptions =>
                                {
                                    sqlOptions.MigrationsAssembly("OpenRIMS.PV.Main.Infrastructure.SQL");
                                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                });
                       }

                   },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );
            services.AddDbContext<IntegrationEventLogContext>(options =>
                    {
                        if (providerSettings.GetValue<string>(key: "Type") == "MySQL")
                        {
                            options.UseMySql(
                                providerSettings.GetValue<string>(key: "ConnectionString"),
                                ServerVersion.AutoDetect(providerSettings.GetValue<string>(key: "ConnectionString")),
                                mySqlOptions =>
                                {
                                    mySqlOptions.MigrationsAssembly("OpenRIMS.PV.Main.Infrastructure.MySQL");
                                    mySqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                });
                        }

                        if (providerSettings.GetValue<string>(key: "Type") == "SQL")
                        {
                            options.UseSqlServer(
                                providerSettings.GetValue<string>(key: "ConnectionString"),
                                sqlServerOptionsAction: sqlOptions =>
                                {
                                    sqlOptions.MigrationsAssembly("OpenRIMS.PV.Main.Infrastructure.SQL");
                                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                });
                        }
                    },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc(
                    "OpenAPISpecification",
                    new OpenApiInfo()
                    {
                        Title = "OpenRIMS.PV. Open API",
                        Version = "1",
                        Description = "OpenRIMS.PV. Open API Layer",
                        Contact = new OpenApiContact()
                        {
                            Email = "info@openrims.org",
                            Name = "OpenRIMS.PV. Information"
                        }
                    });

                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "basic",
                });

                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" }
                        }, new List<string>() }
                });

                setupAction.OperationFilter<GetAppointmentOperationFilter>();

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                setupAction.IncludeXmlComments(xmlCommentsFullPath);
            });

            return services;
        }

        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection eventBusSettings = configuration.GetSection(nameof(EventBusSettings));

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddAutoMapper(typeof(Startup));

            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddTransient<IIntegrationEventService, IntegrationEventService>();

            if (eventBusSettings.GetValue<bool>("Enabled"))
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = eventBusSettings[nameof(EventBusSettings.EventBusConnection)],
                        DispatchConsumersAsync = true
                    };

                    if (!string.IsNullOrEmpty(eventBusSettings[nameof(EventBusSettings.EventBusUserName)]))
                    {
                        factory.UserName = eventBusSettings[nameof(EventBusSettings.EventBusUserName)];
                    }

                    if (!string.IsNullOrEmpty(eventBusSettings[nameof(EventBusSettings.EventBusPassword)]))
                    {
                        factory.Password = eventBusSettings[nameof(EventBusSettings.EventBusPassword)];
                    }

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(eventBusSettings[nameof(EventBusSettings.EventBusRetryCount)]))
                    {
                        retryCount = int.Parse(eventBusSettings[nameof(EventBusSettings.EventBusRetryCount)]);
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });
            }

            return services;
        }

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext =
                        actionContext as ActionExecutingContext;

                    // if there are modelstate errors & all keys were correctly
                    // found/parsed we're dealing with validation errors
                    if (actionContext.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    // if one of the keys wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparsable input
                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection configAuthSettings = configuration.GetSection(nameof(AuthSettings));
            services.Configure<AuthSettings>(configAuthSettings);

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configAuthSettings[nameof(AuthSettings.SigningKey)]));

            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512Signature);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configureOptions =>
                {
                    configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                    configureOptions.TokenValidationParameters = tokenValidationParameters;
                    configureOptions.SaveToken = true;

                    configureOptions.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Access-Control-Expose-Headers", "X-Token-Expired");
                                context.Response.Headers.Add("X-Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddApiKeySupport(configureOptions => { });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(JwtConstants.Strings.JwtClaims.ApiAccess, policy =>
            //    {
            //        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
            //        policy.RequireAuthenticatedUser();
            //        policy.RequireClaim(JwtConstants.Strings.JwtClaimIdentifiers.Rol, JwtConstants.Strings.JwtClaims.ApiAccess);
            //    });
            //    options.AddPolicy("ApiKey", policy =>
            //    {
            //        policy.AuthenticationSchemes.Add(ApiKeyAuthenticationOptions.DefaultScheme);
            //        policy.RequireAuthenticatedUser();
            //    });
            //});

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IJwtTokenHandler, JwtTokenHandler>();
            services.AddTransient<ITokenFactory, TokenFactory>();
            services.AddTransient<IJwtFactory, JwtFactory>();

            return services;
        }

        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<ICustomAttributeConfigRepository, CustomAttributeConfigRepository>();
            services.AddScoped<ISelectionDataRepository, SelectionDataRepository>();

            services.AddScoped<FormHandler, FormHandler>();

            services.AddTransient<IArtefactService, ArtefactService>();
            services.AddTransient<ICustomAttributeService, CustomAttributeService>();
            services.AddTransient<IExcelDocumentService, ExcelDocumentService>();
            services.AddTransient<IInfrastructureService, InfrastructureService>();
            services.AddTransient<ILinkGeneratorService, LinkGeneratorService>();
            services.AddTransient<IMedDraService, MedDraService>();
            services.AddTransient<IPatientService, PatientService>();
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();
            services.AddTransient<IWordDocumentService, WordDocumentService>();
            services.AddTransient<IWorkFlowService, WorkFlowService>();
            services.AddTransient<IXmlDocumentService, XmlDocumentService>();

            IConfigurationSection smtpSettings = configuration.GetSection(nameof(SMTPSettings));
            services.AddTransient<ISMTPMailService, SMTPMailService>(s => new SMTPMailService(
                smtpSettings[nameof(SMTPSettings.SmtpHost)],
                Convert.ToInt32(smtpSettings[nameof(SMTPSettings.Port)]),
                Convert.ToBoolean(smtpSettings[nameof(SMTPSettings.UseSSL)]),
                smtpSettings[nameof(SMTPSettings.MailboxUserName)],
                smtpSettings[nameof(SMTPSettings.MailboxPassword)],
                smtpSettings[nameof(SMTPSettings.MailboxAddress)],
                smtpSettings.GetValue<bool>("Enabled")));

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("EventBusEnabled"))
            {
                IConfigurationSection eventBusSettings = configuration.GetSection(nameof(EventBusSettings));

                services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                {
                    var subscriptionClientName = eventBusSettings[nameof(EventBusSettings.SubscriptionClientName)];
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(eventBusSettings[nameof(EventBusSettings.EventBusRetryCount)]))
                    {
                        retryCount = int.Parse(eventBusSettings[nameof(EventBusSettings.EventBusRetryCount)]);
                    }

                    return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
                });

                services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            }

            return services;
        }
    }
}
