using Grpc.Net.Client;
using HealthChecks.UI.Client;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
using System.Net;
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

services.AddAuthentication("token")
                //JWT tokens handling
                .AddJwtBearer("token", options =>
                {
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    builder.Configuration.GetSection("auth:token").Bind(options);
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
                    builder.Configuration.GetSection("auth:introspection").Bind(options);
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
                
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme,
                    "OIDC");
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser().AddAuthenticationSchemes("token").RequireClaim("scope", "doctors-portal-api");
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
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
        Description = "An ASP.NET Core Web API for managing Driver Portal items",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

// Add Document Storage Adapter

string documentStorageAdapterURI = builder.Configuration["DOCUMENT_STORAGE_ADAPTER_URI"];

if (!string.IsNullOrEmpty(documentStorageAdapterURI))
{
    var httpClientHandler = new HttpClientHandler();

    // Return `true` to allow certificates that are untrusted/invalid                    
    httpClientHandler.ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;


    var httpClient = new HttpClient(httpClientHandler);
    // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
    httpClient.DefaultRequestVersion = HttpVersion.Version20;

    var initialChannel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

    var initialClient = new DocumentStorageAdapter.DocumentStorageAdapterClient(initialChannel);
    // call the token service to get a token.
    var tokenRequest = new Pssg.DocumentStorageAdapter.TokenRequest
    {
        Secret = builder.Configuration["DOCUMENT_STORAGE_ADAPTER_JWT_SECRET"]
    };

    var tokenReply = initialClient.GetToken(tokenRequest);

    if (tokenReply != null && tokenReply.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
    {
        // Add the bearer token to the client.
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

        var channel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

        services.AddTransient(_ => new DocumentStorageAdapter.DocumentStorageAdapterClient(channel));
    }
}

// Add Case Management System (CMS) Adapter 

string cmsAdapterURI = builder.Configuration["CMS_ADAPTER_URI"];

if (!string.IsNullOrEmpty(cmsAdapterURI))
{
    var httpClientHandler = new HttpClientHandler();

    // Return `true` to allow certificates that are untrusted/invalid                    
    httpClientHandler.ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;


    var httpClient = new HttpClient(httpClientHandler);
    // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
    httpClient.DefaultRequestVersion = HttpVersion.Version20;

    if (!string.IsNullOrEmpty(builder.Configuration["CMS_ADAPTER_JWT_SECRET"]))
    {
        var initialChannel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });

        var initialClient = new CaseManager.CaseManagerClient(initialChannel);
        // call the token service to get a token.
        var tokenRequest = new Rsbc.Dmf.CaseManagement.Service.TokenRequest
        {
            Secret = builder.Configuration["CMS_ADAPTER_JWT_SECRET"]
        };

        var tokenReply = initialClient.GetToken(tokenRequest);

        if (tokenReply != null && tokenReply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
        {
            // Add the bearer token to the client.
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
        }
    }

    var channel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });
    services.AddTransient(_ => new CaseManager.CaseManagerClient(channel));
    services.AddTransient(_ => new UserManager.UserManagerClient(channel));
}


// Health Checks
services.AddHealthChecks()
    .AddCheck("driver-portal", () => HealthCheckResult.Healthy("OK"));

services.AddHttpClient();
services.AddHttpContextAccessor();
services.AddTransient<IUserService, UserService>();

var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}")
    .RequireAuthorization(); 

app.MapFallbackToFile("index.html");
app.MapSwagger();

app.Run();



public partial class Program { } // so you can reference it from tests