using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Data;
using System.Net;
using System.Security.Claims;

Console.WriteLine($"Starting Partner Portal Api.");
var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls()
    .UseKestrel(options => { options.Listen(IPAddress.Any, 8080); });
var services = builder.Services;

var env = builder.Environment;
var isDevelopment = env.EnvironmentName == "Development";

var config = builder.Configuration;


services.AddKeycloakWebApiAuthentication(
    keycloakOptions =>
    {
        keycloakOptions.Realm = config["KEYCLOAK_REALM"];
        keycloakOptions.Audience = config["KEYCLOAK_AUDIENCE"];
        keycloakOptions.AuthServerUrl = config["KEYCLOAK_URL"];
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
    //options.AddPolicy(
    //    Policies.Oidc,
    //    policy => policy
    //        // confirm this is working by using a bad secret, currently the secret is not being validated
    //        .RequireAuthenticatedUser().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    //// TODO verify if we need to add scope medical-portal-ui and medical-portal-api or if just medical-portal will do, in other projects there are api and ui
    //// the below does not work, since the scope claim looks something like "email profile openid". This problem has already been solved, research the proper way to handle scope
    //// need to add the scope to keycloak admin UI before we can add the scope to FE, which would pass the scope claim to the BE
    ////.RequireClaim(Claims.Scope, "medical-portal")
    //);

    //options.AddPolicy(
    //    Policies.MedicalPractitioner,
    //    policy => policy
    //    .RequireAuthenticatedUser()
    //    .RequireRole(Claims.IdentityProvider, Roles.Practitoner, Roles.Moa));
    //options.AddPolicy(Policies.Enrolled, policy => policy
    //.RequireAuthenticatedUser()
    //    .RequireRole(Claims.IdentityProvider, Roles.Dmft));
});

services.AddSerilogBootstrapLogger();
services.AddCmsAdapterGrpcService(config);
Console.WriteLine($"cms-adapter grpc service registered.");

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

try
{
    var app = builder.Build();
    app.UseRouting();
    app.UseCors(corsPolicy);
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        //    .RequireAuthorization(Policies.MedicalPractitioner, Policies.Enrolled, Policies.Oidc);
    });
    //app.MapControllers();
    app.MapSwagger();
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
        // Flatten the Resource Access claim
        //identity.AddClaims(
        //    identity.GetResourceAccessRoles(Clients.License)
        //        .Select(role => new Claim(identity.RoleClaimType, role))
        //);

        //identity.AddClaims(
        //    identity.GetResourceAccessRoles(Clients.DmftStatus)
        //        .Select(role => new Claim(identity.RoleClaimType, role))
        //);

        // TODO I think this is wrong, we should only need to call this once but this is validating on every request
        //var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        //context.Principal = await userService.Login(context.Principal);
    }
}