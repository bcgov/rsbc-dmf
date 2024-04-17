//using HealthChecks.UI.Client;
//using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using pdipadapter.Infrastructure.Auth;
using pdipadapter.Infrastructure.Services;
//using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;

Console.WriteLine($"Starting Pidp Adapter.");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls()
    .UseKestrel(options => { options.Listen(IPAddress.Any, 7215); });

var services = builder.Services;
var env = builder.Environment;
var config = new PdipadapterConfiguration();
builder.Configuration.Bind(config);

services.AddSingleton(config);
services.AddKeycloakAuth(config);
services.AddScoped<IPidpAdapterAuthorizationService, PidpAdapterAuthorizationService>();

services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

Console.WriteLine($"Keycloak Authentication initialized.");
