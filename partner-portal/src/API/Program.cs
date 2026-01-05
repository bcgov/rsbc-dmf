using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Pssg.DocumentStorageAdapter.Client;
using Pssg.Dmf.IcbcAdapter.Client;
using Rsbc.Dmf.CaseManagement.Client;
using Rsbc.Dmf.IcbcAdapter.Client;
using Rsbc.Dmf.PartnerPortal.Api;
using Rsbc.Dmf.PartnerPortal.Api.Model;
using Rsbc.Dmf.PartnerPortal.Api.Services;
using Serilog;
using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

Console.WriteLine($"Starting Partner Portal Api.");
var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls()
    .UseKestrel(options => { options.Listen(IPAddress.Any, 8080); });
var services = builder.Services;

var env = builder.Environment;
var isDevelopment = env.EnvironmentName == "Development";

var config = builder.Configuration;
var appConfiguration = new AppConfiguration(config);
services.AddSingleton(appConfiguration);

services.AddKeycloakWebApiAuthentication(
    keycloakOptions =>
    {
        keycloakOptions.Realm = config["KEYCLOAK_REALM"];
        keycloakOptions.Audience = config["KEYCLOAK_AUDIENCE"];
        keycloakOptions.AuthServerUrl = config["KEYCLOAK_AUTH_URL"];
        keycloakOptions.VerifyTokenAudience = false;
    },
    jwtBearerOptions =>
    {
        jwtBearerOptions.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context => await OnTokenValidatedAsync(context),
            OnAuthenticationFailed = context =>
            {
                Log.Error(context.Exception, "Error validating bearer token");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Task.CompletedTask;
            }
        };
    });

services.AddAuthorization(options =>
{
    options.AddPolicy(
        Policies.Oidc,
        policy => policy
            // confirm this is working by using a bad secret, currently the secret is not being validated
            .RequireAuthenticatedUser().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    );

    // TODO update these after auth is migrated to different keycloak
    options.AddPolicy(
        Policies.PartnerPortal,
        policy => policy
            .RequireAuthenticatedUser());
});

// NOTE temporary logger code, replace after adding logger e.g. Serilog/Splunk
// TODO # "remove loggerFactory.Create and get the loggerFactory from ".AddSerilogBootstrapLogger"
using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace)
    .AddConsole());

services.AddSerilogBootstrapLogger();
services.AddHttpContextAccessor();
services.AddAutoMapperSingleton(loggerFactory);
services.AddMemoryCache();

// grpc clients
services.AddDocumentStorageClient(builder.Configuration);
services.AddCaseManagementAdapterClient(builder.Configuration, loggerFactory);

services.AddTransient<IUserService, UserService>();
services.AddTransient<IExportService, ExportService>();
services.AddTransient<DocumentFactory>();

// Add ICBC Adapter
services.AddIcbcAdapterClient(builder.Configuration, loggerFactory); 
services.AddSingleton<ICachedIcbcAdapterClient, CachedIcbcAdapterClient>();

// session variables for storing the results from driver search
services.AddDistributedMemoryCache();
services.AddSession(options => options.IdleTimeout = TimeSpan.FromHours(1));

var corsPolicy = "CorsPolicy";
services.AddCors(options =>
{
    options.AddPolicy(corsPolicy,
        builder => builder
            .WithOrigins($"{config["CORS_ORIGINS"]}")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

services.AddControllers();

services.AddSerilogLogger(config, isDevelopment);

services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Partner Portal API",
        Description = "An ASP.NET Core Web API for managing Partner Portal items"
    });
});

// Health Checks
services
    .AddHealthChecks()
    .AddCheck("partner-portal", () => HealthCheckResult.Healthy("OK"));

try
{
    var app = builder.Build();
    app.UseRouting();
    app.UseCors(corsPolicy);
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSession();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers()
            .RequireAuthorization(Policies.Oidc);
    });
    app.MapSwagger();
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
    app.Run();
    Console.WriteLine("Program configuration completed.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

async Task OnTokenValidatedAsync(TokenValidatedContext context)
{
    if (context.Principal?.Identity is ClaimsIdentity identity
        && identity.IsAuthenticated)
    {
   
        // TODO I think this is wrong, we should only need to call this once but this is validating on every request
        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        context.Principal = await userService.Login(context.Request, context.Principal);
    }
}

public static class Policies
{
    public const string Oidc = "oidc";
    public const string PartnerPortal = "partner-portal";
}