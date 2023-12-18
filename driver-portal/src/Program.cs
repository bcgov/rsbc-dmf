using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using HealthChecks.UI.Client;
using Grpc.Net.Client;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;

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
}


// Health Checks
services.AddHealthChecks()
    .AddCheck("driver-portal", () => HealthCheckResult.Healthy("OK"));

services.AddHttpClient();


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

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");
app.MapSwagger();

app.Run();

