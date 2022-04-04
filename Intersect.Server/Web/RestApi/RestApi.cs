using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Web.Http;
//using System.Web.Http.Routing;

using Intersect.Configuration;
using Intersect.Enums;
using Intersect.Logging;
using Intersect.Server.Localization;
using Intersect.Server.Web.RestApi.Authentication;
using Intersect.Server.Web.RestApi.Authentication.OAuth;
using Intersect.Server.Web.RestApi.Configuration;
using Intersect.Server.Web.RestApi.Constraints;
using Intersect.Server.Web.RestApi.Logging;
using Intersect.Server.Web.RestApi.Middleware;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.RouteProviders;
using Intersect.Server.Web.RestApi.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;

using Owin;

using Swashbuckle.AspNetCore.SwaggerUI;

namespace Intersect.Server.Web.RestApi
{
    // TODO: Migrate to a proper service
    internal sealed class RestApi : IAppConfigurationProvider, IConfigurable<ApiConfiguration>, IDisposable
    {
        private readonly WebApplicationBuilder mBuilder;

        private readonly object mDisposeLock;

        private bool mDisposing;

        private IDisposable mWebAppHandle;

        private WebApplication mWebApplication;

        public RestApi(ushort apiPort)
        {
            mDisposeLock = new object();

            Configuration = ApiConfiguration.Create();

            mBuilder = CreateBuilder(apiPort);

            StartOptions = new StartOptions();

            Configuration.Hosts.ToList().ForEach(host => StartOptions.Urls?.Add(host));

            if (apiPort > 0)
            {
                StartOptions.Urls?.Clear();
                StartOptions.Urls?.Add("http://*:" + apiPort + "/");
            }

            AuthenticationProvider = new OAuthProvider(Configuration);
        }

        public ApiConfiguration Configuration { get; }


        public bool Disposed { get; private set; }

        public bool IsStarted => mWebAppHandle != null;

        public StartOptions StartOptions { get; }

        private AuthenticationProvider AuthenticationProvider { get; }

        private WebApplication Build()
        {
            var app = mBuilder.Build();

            app.UseHttpLogging();
            //app.UseHttpsRedirection();
            //app.UseHsts();
            
            if (app.Environment.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            if (Configuration.SwaggerEnabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;
        }

        private WebApplicationBuilder CreateBuilder(ushort defaultPort)
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddCors(corsOptions =>
            {
                foreach (var corsConfiguration in Configuration.Cors)
                {
                    var policy = new CorsPolicy();

                    foreach (var exposedHeader in corsConfiguration.ExposedHeaders)
                    {
                        policy.ExposedHeaders.Add(exposedHeader);
                    }

                    foreach (var header in corsConfiguration.Headers)
                    {
                        policy.Headers.Add(header);
                    }

                    foreach (var method in corsConfiguration.Methods)
                    {
                        policy.Methods.Add(method);
                    }

                    if (!string.Equals("*", corsConfiguration.Origin, StringComparison.Ordinal))
                    {
                        foreach (var origin in corsConfiguration.Origin.Split(','))
                        {
                            policy.Origins.Add(origin);
                        }
                    }
                    else
                    {
                        policy.Origins.Add("*");
                    }

                    policy.SupportsCredentials = corsConfiguration.SupportsCredentials;

                    var policyName = string.IsNullOrWhiteSpace(corsConfiguration.Name)
                        ? Guid.NewGuid().ToString()
                        : corsConfiguration.Name;

                    corsOptions.AddPolicy(policyName, policy);
                }
            });

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.Configure<RouteOptions>(routeOptions =>
            {
                routeOptions.ConstraintMap.Add(nameof(AdminActions), typeof(AdminActionsConstraint));
                routeOptions.ConstraintMap.Add(nameof(LookupKey), typeof(LookupKey.Constraint));
                routeOptions.ConstraintMap.Add(nameof(ChatMessage), typeof(ChatMessage.Constraint));
            });

            builder.Services
                .AddControllers()
                .AddJsonOptions(jsonOptions =>
                {
                    jsonOptions.AllowInputFormatterExceptionMessages = Configuration.DebugMode;
                    jsonOptions.JsonSerializerOptions.AllowTrailingCommas = true;
                    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    jsonOptions.JsonSerializerOptions.WriteIndented = Configuration.DebugMode;
                });

            if (Configuration.SwaggerEnabled)
            {
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Intersect Authentication API",
                        Description = "API for Authenticating to Intersect game servers",
                        TermsOfService = new Uri("https://ascensiongame.dev/intersect/tos"),
                        Contact = new OpenApiContact
                        {
                            Name = "Support",
                            Url = new Uri("https://ascensiongame.dev/intersect/contact")
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT",
                            Url = new Uri("https://ascensiongame.dev/intersect/license")
                        }
                    });

                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            }

            builder.WebHost.ConfigureKestrel(kestrelOptions =>
            {
                if (!Configuration.Hosts.IsDefaultOrEmpty)
                {
                    foreach (var host in Configuration.Hosts)
                    {
                        var uri = new Uri(host);
                        kestrelOptions.Listen(new DnsEndPoint(uri.Host, uri.Port));
                    }
                }
                else if (defaultPort != default)
                {
                    kestrelOptions.ListenLocalhost(defaultPort);
                }
            });

            return builder;
        }

        public void Configure(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();

            var services = config.Services;
            if (services == null)
            {
                throw new InvalidOperationException();
            }

            Configuration.Cors.Select(configuration => configuration.AsCorsOptions())
                ?.ToList()
                .ForEach(corsOptions => appBuilder.UseCors(corsOptions));

            //var constraintResolver = new DefaultInlineConstraintResolver();
            //constraintResolver.ConstraintMap?.Add(nameof(AdminActions), typeof(AdminActionsConstraint));
            //constraintResolver.ConstraintMap?.Add(nameof(LookupKey), typeof(LookupKey.Constraint));
            //constraintResolver.ConstraintMap?.Add(nameof(ChatMessage), typeof(ChatMessage.Constraint));

            // Map routes
            //config.MapHttpAttributeRoutes(constraintResolver, new VersionedRouteProvider());
            config.DependencyResolver = new IntersectServiceDependencyResolver(Configuration, config);

            // Make JSON the default response type for browsers
            config.Formatters?.JsonFormatter?.Map("accept", "text/html", "application/json");

            if (Configuration.DebugMode)
            {
                appBuilder.SetLoggerFactory(new IntersectLoggerFactory());
            }

            if (Configuration.RequestLogging)
            {
                appBuilder.Use<IntersectRequestLoggingMiddleware>(Configuration.RequestLogLevel);
            }

            appBuilder.Use<IntersectThrottlingMiddleware>(
                Configuration.ThrottlePolicy, null, Configuration.FallbackClientKey, null
            );

            AuthenticationProvider.Configure(appBuilder);

            appBuilder.UseWebApi(config);
        }

        public void Dispose()
        {
            lock (mDisposeLock)
            {
                if (Disposed || mDisposing)
                {
                    return;
                }

                mDisposing = true;
            }

            mWebAppHandle?.Dispose();
            mWebApplication?.DisposeAsync().AsTask().GetAwaiter().GetResult();
            Disposed = true;
        }

        public void Start()
        {
            if (!Configuration.Enabled)
            {
                return;
            }

            try
            {
                mWebApplication = Build();
                mWebApplication.Start();
                //mWebAppHandle = WebApp.Start(StartOptions, Configure);
                //Trace.Listeners.Remove("HostingTraceListener");
                //StartOptions.Urls?.ToList().ForEach(host => Console.WriteLine(Strings.Intro.api.ToString(host)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(Strings.Intro.apifailed);
                Log.Error(Strings.Intro.apifailed + Environment.NewLine + exception);
            }
        }
    }
}
