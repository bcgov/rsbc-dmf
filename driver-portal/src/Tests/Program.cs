using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.DriverPortal.Api;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var env = builder.Environment;

services.AddControllersWithViews().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

services.AddAutoMapperSingleton(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole()));

var app = builder.Build();
app.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
app.Run();

public partial class Program { } 