using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using NodaTime;
using OneHealthAdapter.Extensions;
using OneHealthAdapter.Infrastructure.Auth;
using OneHealthAdapter.Infrastructure.HttpClients;
using OneHealthAdapter.Services;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text.Json;

namespace OneHealthAdapter;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) => this.Configuration = configuration;
    public void ConfigureServices(IServiceCollection services)
    {
        var config = this.InitializeConfiguration(services);

        services
          .AddHttpClients(config)
          // keycloak auth needed for keycloak role management - I don't think we will need this or the keycloak role management, if we did, I think this should be moved to a new
          // project that would serve the keycloak role management controllers. Role management will likely be managed by OneHealth UI
          //.AddKeycloakAuth(config)
          .AddSingleton<IClock>(NodaTime.SystemClock.Instance)
          .AddSingleton<Microsoft.Extensions.Logging.ILogger>(svc => svc.GetRequiredService<ILogger<Startup>>())
          .AddTransient<IOneHealthManager, OneHealthManager>();

        // TODO jwt auth
        /*
         *             if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
            {
                byte[] key = Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]);
                Array.Resize(ref key, 32);

                // Configure JWT authentication
                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(o =>
                {
                    o.SaveToken = true;
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = false,
                        ValidIssuer = Configuration["JWT_VALID_ISSUER"],
                        ValidAudience = Configuration["JWT_VALID_AUDIENCE"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(key)
                    };
                });
            }
            else
            {
                services.AddAuthentication();
            }
                
            services.AddAuthorization();*/

        services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

        services.AddHttpClient();

        services.AddSingleton<IAuthorizationHandler, RealmAccessRoleHandler>();
        services.AddTransient<IClaimsTransformation, KeycloakClaimTransformer>();
        services.AddHttpContextAccessor();
        services.AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User);

        services.AddDistributedMemoryCache();

        services.AddHealthChecks()
            .AddCheck("liveliness", () => HealthCheckResult.Healthy());

        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new HeaderApiVersionReader("api-version");
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pidp Adapter API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
            options.CustomSchemaIds(x => x.FullName);
        });

        services.AddCors(setupAction => setupAction.AddPolicy(Constants.CorsPolicy, corsPolicyBuilder => corsPolicyBuilder.WithOrigins(config.Settings.Cors.AllowedOrigins)));
        services.AddFluentValidationRulesToSwagger();
    }

    private Configuration InitializeConfiguration(IServiceCollection services)
    {
        var config = new Configuration();
        this.Configuration.Bind(config);
        services.AddSingleton(config);

        Log.Logger.Information("### App Version:{0} ###", Assembly.GetExecutingAssembly().GetName().Version);
        Log.Logger.Information("### Pidp Adapter Configuration:{0} ###", JsonSerializer.Serialize(config));

        return config;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
         
        }
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pidp Api Adapter"));

        app.UseSerilogRequestLogging(options => options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            var userId = httpContext.User.GetUserId();
            if (!userId.Equals(Guid.Empty))
            {
                diagnosticContext.Set("User", userId);
            }
        });
        app.UseRouting();
        app.UseCors(Constants.CorsPolicy);
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<OneHealthService>();
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client");
            });
            if (env.IsDevelopment())
            {
                endpoints.MapGrpcReflectionService();
            }
        });

    }
}
