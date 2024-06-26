//using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Net;
//using System.Security.Claims;

Console.WriteLine($"Starting Partner Portal Api.");
var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls()
    .UseKestrel(options => { options.Listen(IPAddress.Any, 8080); });
var services = builder.Services;
var env = builder.Environment;

var config = builder.Configuration;
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

// TODO add authentication here

services.AddControllers();

services.AddSerilogLogger(config, builder.Environment.EnvironmentName == "Development");

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
    //app.UseAuthentication();
    //app.UseAuthorization();
    app.MapControllers();
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