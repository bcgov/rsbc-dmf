using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using pdipadapter.Infrastructure.Auth;
using pdipadapter.Infrastructure.Services;
using PidpAdapter.API.Infrastructure;
using System.Net;
using System.Security.Claims;

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

Console.WriteLine($"Keycloak Authentication initialized.");

services.AddScoped<IPidpAdapterAuthorizationService, PidpAdapterAuthorizationService>();

//services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
//services.AddControllers(options => options.Conventions.Add(new RouteTokenTransformerConvention(new KabobCaseParameterTransformer())))
//    .AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Startup>())
//    .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb))
//    .AddHybridModelBinder(); 
services.AddHttpClient();

//services.AddSingleton<IAuthorizationHandler, RealmAccessRoleHandler>();
services.AddTransient<IClaimsTransformation, KeycloakClaimTransformer>();
services.AddHttpContextAccessor();
services.AddTransient<ClaimsPrincipal>(s => s.GetService<IHttpContextAccessor>().HttpContext.User);
//services.AddScoped<IProxyRequestClient, ProxyRequestClient>();

services.AddDistributedMemoryCache();
services.AddCmsAdapterGrpcService(config);

Console.WriteLine($"cms-adapter grpc service registered.");

services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:8089", "http://localhost:9092", "http://localhost:4200", "https://medical-portal-pidp-0137d5-dev.apps.silver.devops.gov.bc.ca") //use config later
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("Program configuration completed.");