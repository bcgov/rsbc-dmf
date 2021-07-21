using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace RSBC.DMF.DoctorsPortal.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private IConfiguration configuration { get; }
        private const string HealthCheckReadyTag = "ready";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("token")
                //JWT tokens handling
                .AddJwtBearer("token", options =>
                {
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
                })
                //reference tokens handling
                .AddOAuth2Introspection("introspection", options =>
                {
                    configuration.GetSection("auth:introspection").Bind(options);
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("OAuth", policy =>
                {
                    policy.RequireAuthenticatedUser();
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
            services.AddResponseCompression();
            services.AddHealthChecks().AddCheck("Doctors portal API", () => HealthCheckResult.Healthy("OK"), new[] { HealthCheckReadyTag });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
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
                opts.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
                {
                    diagCtx.Set("User", httpCtx.User.FindFirst(ClaimTypes.Upn)?.Value);
                    diagCtx.Set("Host", httpCtx.Request.Host);
                    diagCtx.Set("UserAgent", httpCtx.Request.Headers["User-Agent"].ToString());
                    diagCtx.Set("RemoteIP", httpCtx.Connection.RemoteIpAddress.ToString());
                    diagCtx.Set("ConnectionId", httpCtx.Connection.Id);
                    diagCtx.Set("Forwarded", httpCtx.Request.Headers["Forwarded"].ToString());
                    diagCtx.Set("ContentLength", httpCtx.Response.ContentLength);
                };
            });

            app.UseForwardedHeaders();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();

            app.UseRouting();
            app.UseResponseCompression();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization("OAuth");
                endpoints.MapHealthChecks("/hc/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains(HealthCheckReadyTag)
                });

                endpoints.MapHealthChecks("/hc/live", new HealthCheckOptions()
                {
                    Predicate = (_) => false
                });
            });
        }
    }
}