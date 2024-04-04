using HealthChecks.UI.Client;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rsbc.Dmf.DriverPortal.Api;
using Rsbc.Dmf.DriverPortal.Api.Services;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls()
    .UseKestrel(options =>
    {
        options.Listen(IPAddress.Any, 8080);
    });

// add services to DI container
var services = builder.Services;
var env = builder.Environment;

services.AddAuthentication("introspection")
    //JWT tokens handling
    .AddJwtBearer("jwt", options =>
    {
        options.SaveToken = true;
        options.MapInboundClaims = true;
        
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        builder.Configuration.GetSection("auth:jwt").Bind(options);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };

        // if token does not contain a dot, it is a reference token, forward to introspection auth scheme
        options.ForwardDefaultSelector = ctx =>
        {
            var authHeader = (string)ctx.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) return null;
            return authHeader.Substring("Bearer ".Length).Trim().Contains('.') ? null : "introspection";
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                await Task.CompletedTask;
                var userInfo = ctx.Principal.FindFirstValue("userInfo");
            },
            OnAuthenticationFailed = async ctx =>
            {
                await Task.CompletedTask;
            }
        };
    })
    .AddOAuth2Introspection("introspection", options =>
    {
        //options.EnableCaching = true;
        //options.CacheDuration = TimeSpan.FromMinutes(1);
        builder.Configuration.GetSection("auth:introspection").Bind(options);
        //options.SkipTokensWithDots = false;
        options.Events = new OAuth2IntrospectionEvents
        {
            OnTokenValidated = async ctx =>
            {
                var userService = ctx.HttpContext.RequestServices.GetRequiredService<IUserService>();
                ctx.Principal = await userService.Login(ctx.Principal);
                ctx.Success();
            },

            OnUpdateClientAssertion =
                async ctx => { await Task.CompletedTask; }

        };

    })
    .AddIdentityCookies();

services.AddAuthorization(options =>
{
    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
    {
        policy
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes("jwt")
            .RequireClaim("scope", "driver-portal-api");
    });

    //options.DefaultPolicy = options.GetPolicy(JwtBearerDefaults.AuthenticationScheme) ?? null!;
    options.AddPolicy(Policy.Driver, new DriverPolicyFactory().Create());

    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        //JwtBearerDefaults.AuthenticationScheme,
        OAuth2IntrospectionDefaults.AuthenticationScheme,
        "OIDC");
    defaultAuthorizationPolicyBuilder =
        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser().AddAuthenticationSchemes("introspection"); //.RequireClaim("scope", "driver-portal-api");
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    //options.AddPolicy(Policy.Driver, new DriverPolicyFactory().Create());

    //options..DefaultScheme = "introspection";
    //options.DefaultChallengeScheme = "introspection";

});

services.AddCors();
services.AddControllersWithViews().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    // ignore omitted parameters on models to enable optional params (e.g. User update)
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Driver Portal API",
        Description = "An ASP.NET Core Web API for managing Driver Portal items"
    });
    options.CustomSchemaIds(type => SwashbuckleHelper.GetSchemaId(type));
});

// grpc clients
services.AddDocumentStorageClient(builder.Configuration);
services.AddCaseManagementAdapterClient(builder.Configuration);
services.AddIcbcAdapterClient(builder.Configuration);

// Health Checks
services.AddHealthChecks()
    .AddCheck("driver-portal", () => HealthCheckResult.Healthy("OK"));

services.AddHttpClient();
services.AddHttpContextAccessor();
services.AddMemoryCacheService();
services.AddTransient<ICachedIcbcAdapterClient, CachedIcbcAdapterClient>();
services.AddTransient<IUserService, UserService>();
services.AddTransient<DocumentFactory>();
// NOTE temporary logger code, replace after adding logger e.g. Serilog/Splunk
using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace)
    .AddConsole());
services.AddAutoMapperSingleton(loggerFactory);

var app = builder.Build();

string pathBase = builder.Configuration["BASE_PATH"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

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

// configure HTTP request pipeline


// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHsts();

app.UseSwagger();
app.UseSwaggerUI();

// UsePathBase will not work without this, potentially related to this https://github.com/dotnet/aspnetcore/issues/38448
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}")
    .RequireAuthorization();

app.MapFallbackToFile("index.html");
app.MapSwagger();

app.Run();