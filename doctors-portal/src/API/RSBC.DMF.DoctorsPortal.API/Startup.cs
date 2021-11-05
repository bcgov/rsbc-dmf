using HealthChecks.UI.Client;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RSBC.DMF.DoctorsPortal.API.Services;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RSBC.DMF.DoctorsPortal.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        private IConfiguration configuration { get; }
        private const string HealthCheckReadyTag = "ready";
        private readonly IHostEnvironment environment;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication()
                //JWT tokens handling
                .AddJwtBearer("token", options =>
                {
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    configuration.GetSection("auth:token").Bind(options);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };

                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                    // if token does not contain a dot, it is a reference token, forward to introspection auth scheme
                    options.ForwardDefaultSelector = ctx =>
                    {
                        var authHeader = (string)ctx.Request.Headers["Authorization"];
                        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) return null;
                        return authHeader.Substring("Bearer ".Length).Trim().Contains(".") ? null : "introspection";
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async ctx =>
                        {
                            var userService = ctx.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            ctx.Principal = await userService.Login(ctx.Principal);
                            ctx.Success();
                        }
                    };
                })
                //reference tokens handling
                .AddOAuth2Introspection("introspection", options =>
                {
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(1);
                    configuration.GetSection("auth:introspection").Bind(options);
                    options.Events = new OAuth2IntrospectionEvents
                    {
                        OnTokenValidated = async ctx =>
                        {
                            var userService = ctx.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            ctx.Principal = await userService.Login(ctx.Principal);
                            ctx.Success();
                        },
                        OnUpdateClientAssertion =
                        async ctx =>
                        {
                            await Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("OAuth", policy =>
                {
                    policy.RequireAuthenticatedUser().AddAuthenticationSchemes("token");
                    policy.RequireClaim("scope", "doctors-portal-api");
                });
            });
            services.AddControllers(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());
            });
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "RSBC.DMF.DoctorsPortal.API.xml"), true);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RSBC.DMF.DoctorsPortal.API", Version = "v1" });
            });
            var dpBuilder = services.AddDataProtection();
            var keyRingPath = configuration.GetValue("DATAPROTECTION__PATH", string.Empty);
            if (!string.IsNullOrWhiteSpace(keyRingPath))
            {
                //configure data protection folder for key sharing
                dpBuilder.PersistKeysToFileSystem(new DirectoryInfo(keyRingPath));
            }
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                //set known network of forward headers
                options.ForwardLimit = 2;
                var configvalue = configuration.GetValue("app:knownNetwork", string.Empty)?.Split('/');
                if (configvalue.Length == 2)
                {
                    var knownNetwork = new IPNetwork(IPAddress.Parse(configvalue[0]), int.Parse(configvalue[1]));
                    options.KnownNetworks.Add(knownNetwork);
                }
            });
            services.AddCors(opts => opts.AddDefaultPolicy(policy =>
            {
                // try to get array of origins from section array
                var corsOrigins = configuration.GetSection("app:cors:origins").GetChildren().Select(c => c.Value).ToArray();
                // try to get array of origins from value
                if (!corsOrigins.Any()) corsOrigins = configuration.GetValue("app:cors:origins", string.Empty).Split(',');
                corsOrigins = corsOrigins.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray();
                if (corsOrigins.Any())
                {
                    policy.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins(corsOrigins);
                }
            }));
            services.AddDistributedMemoryCache();
            services.AddResponseCompression();
            services.AddHealthChecks().AddCheck("Doctors portal API", () => HealthCheckResult.Healthy("OK"), new[] { HealthCheckReadyTag });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddHttpContextAccessor();
            services.AddCmsAdapterGrpcService(configuration.GetSection("cms"));
            services.AddTransient<ICaseQueryService, CaseService>();
            services.AddTransient<IUserService, UserService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "api/{documentName}/openapi.json";
                });
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/openapi.json", "RSBC.DMF.DoctorsPortal.API v1");
                    c.RoutePrefix = "api";
                });
            }

            app.UseSerilogRequestLogging(opts =>
            {
                opts.GetLevel = ExcludeHealthChecks;
                opts.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
                {
                    diagCtx.Set("User", httpCtx.User.FindFirstValue(ClaimTypes.Upn));
                    diagCtx.Set("Host", httpCtx.Request.Host);
                    diagCtx.Set("UserAgent", httpCtx.Request.Headers["User-Agent"].ToString());
                    diagCtx.Set("RemoteIP", httpCtx.Connection.RemoteIpAddress.ToString());
                    diagCtx.Set("ConnectionId", httpCtx.Connection.Id);
                    diagCtx.Set("Forwarded", httpCtx.Request.Headers["Forwarded"].ToString());
                    diagCtx.Set("ContentLength", httpCtx.Response.ContentLength);
                };
            });

            app.UseHealthChecks("/hc/ready", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/hc/live", new HealthCheckOptions
            {
                // Exclude all checks and return a 200-Ok.
                Predicate = _ => false
            });

            app.UseAuthentication();

            app.UseCors();

            app.UseRouting();
            app.UseResponseCompression();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization("OAuth")
                    ;
            });

            if (!string.IsNullOrEmpty(configuration["USE_SPA"]))
            {
                app.UseSpa(spa =>
                {
                    // To learn more about options for serving an Angular SPA from ASP.NET Core,
                    // see https://go.microsoft.com/fwlink/?linkid=864501
                    if (string.IsNullOrEmpty(configuration["ANGULAR_DEV_SERVER"]))
                    {
                        spa.Options.SourcePath = "../UI/doctors-portal";
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                    else
                    {
                        spa.UseProxyToSpaDevelopmentServer(configuration["ANGULAR_DEV_SERVER"]);
                    }
                });
            }
        }

        private static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception ex) =>
            ex != null
                ? LogEventLevel.Error
                : ctx.Response.StatusCode >= (int)HttpStatusCode.InternalServerError
                    ? LogEventLevel.Error
                    : ctx.Request.Path.StartsWithSegments("/hc", StringComparison.InvariantCultureIgnoreCase)
                        ? LogEventLevel.Verbose
                        : LogEventLevel.Information;
    }
}