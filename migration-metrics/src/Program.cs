
using System.Text.Json.Serialization;
using MigrationMetrics.Helpers;
using MigrationMetrics.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

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

services.AddDbContext<DataContext>();
services.AddCors();
services.AddControllersWithViews().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    // ignore omitted parameters on models to enable optional params (e.g. User update)
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// configure DI for application services
services.AddScoped<IMonthlyCountStatService, MonthlyCountStatService>();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MigrationMetrics API",
        Description = "An ASP.NET Core Web API for managing MigrationMetrics items",
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

// health checks. 
services.AddHealthChecks()
    .AddCheck("migration-metrics", () => HealthCheckResult.Healthy("OK"));

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

app.MapSwagger();

app.UseHttpLogging();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");



app.Run();

