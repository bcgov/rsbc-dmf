using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using HealthChecks.UI.Client;
using PidpAdapter.Extensions;
using PidpAdapter.Infrastructure.HttpClients;
using PidpAdapter.Services;
using Serilog;
using System.Text;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PidpAdapter;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) => this.Configuration = configuration;
    public void ConfigureServices(IServiceCollection services)
    {
        services
          .AddHttpClients(this.Configuration)
          .AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Startup>>());

        if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
        {
            byte[] key = Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]);
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
                    ValidIssuer = Configuration["JWT_VALID_ISSUER"],
                    ValidAudience = Configuration["JWT_VALID_AUDIENCE"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        }
        else
        {
            services.AddAuthentication();
        }

        services.AddAuthorization();

        services.AddHttpClient();

        services.AddHttpContextAccessor();
        services.AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User);
        services.AddAutoMapperSingleton();

        // health checks. 
        services.AddHealthChecks()
            .AddCheck("legacy-adapter", () => HealthCheckResult.Healthy("OK"));

        services.AddGrpc(opts =>
        {
            opts.EnableDetailedErrors = true;
        });
        services.AddGrpcReflection();
        services.AddDistributedMemoryCache();

        services.AddHealthChecks()
            .AddCheck("liveliness", () => HealthCheckResult.Healthy());
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
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<PidpService>();
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
