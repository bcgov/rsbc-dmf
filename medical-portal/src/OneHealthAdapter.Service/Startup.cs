using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NodaTime;
using OneHealthAdapter.Extensions;
using OneHealthAdapter.Infrastructure.Auth;
using OneHealthAdapter.Infrastructure.HttpClients;
using OneHealthAdapter.Services;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace OneHealthAdapter;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) => this.Configuration = configuration;
    public void ConfigureServices(IServiceCollection services)
    {
        var config = this.InitializeConfiguration(services);

        services
          .AddHttpClients(config)
          .AddSingleton<IClock>(NodaTime.SystemClock.Instance)
          .AddSingleton<Microsoft.Extensions.Logging.ILogger>(svc => svc.GetRequiredService<ILogger<Startup>>());

        if (!string.IsNullOrEmpty(Configuration["Jwt:Secret"]))
        {
            byte[] key = Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]);
            Array.Resize(ref key, 32);

            // Configure JWT authentication
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = false,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(key)
                };
            });
        }
        else
        {
            services.AddAuthentication();
        }

        services.AddAuthorization();

        services.AddHttpClient();

        services.AddSingleton<IAuthorizationHandler, RealmAccessRoleHandler>();
        services.AddTransient<IClaimsTransformation, KeycloakClaimTransformer>();
        services.AddHttpContextAccessor();
        services.AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User);

        services.AddGrpc(opts =>
        {
            opts.EnableDetailedErrors = true;
        });
        services.AddGrpcReflection();
        services.AddDistributedMemoryCache();

        services.AddHealthChecks()
            .AddCheck("liveliness", () => HealthCheckResult.Healthy());
    }

    private Configuration InitializeConfiguration(IServiceCollection services)
    {
        var config = new Configuration();
        this.Configuration.Bind(config);
        services.AddSingleton(config);

        Log.Logger.Information("### App Version:{0} ###", Assembly.GetExecutingAssembly().GetName().Version);
        Log.Logger.Information("### Pidp Adapter Configuration:{0} ###", JsonSerializer.Serialize(config));

        return config;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
         
        }

        app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            var userId = httpContext.User.GetUserId();
            if (!userId.Equals(Guid.Empty))
            {
                diagnosticContext.Set("User", userId);
            }
        });
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<OneHealthService>();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client");
            });
            if (env.IsDevelopment())
            {
                endpoints.MapGrpcReflectionService();
            }
        });

    }
}
