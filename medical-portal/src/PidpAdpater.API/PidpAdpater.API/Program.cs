//using HealthChecks.UI.Client;
//using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;

Console.WriteLine($"Starting Pidp Adapter.");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls()
    .UseKestrel(options => { options.Listen(IPAddress.Any, 7215); });

// add services to DI container
var services = builder.Services;
var env = builder.Environment;