using System;
using System.Net.Http;
using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Splunk;

using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Linq;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Invio.Extensions.Authentication.JwtBearer;

namespace Rsbc.Dmf.BcMailAdapter
{
    /// <summary>
    /// Start Up File
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Autentication Handler
        /// </summary>
        public class AllowAnonymous : IAuthorizationHandler
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public Task HandleAsync(AuthorizationHandlerContext context)
            {
                foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
                    context.Succeed(requirement); //Simply pass all requirements

                return Task.CompletedTask;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }
        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Web Host Environment
        /// </summary>
        public IWebHostEnvironment _env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });


            if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
            {
                services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders();

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
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]))
                    };
                })
                .AddJwtBearerQueryStringAuthentication((JwtBearerQueryStringOptions options) =>
                {
                    options.QueryStringParameterName = "access_token";
                    //options.QueryStringBehavior = QueryStringBehaviors.Redact;
                });

            }
            else
            {
                services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
            }
            services.AddAuthorization();

            // basic REST controller 
            services
                
                .AddProblemDetails(opts => {
                    opts.ValidationProblemStatusCode = StatusCodes.Status400BadRequest;

                })
                
                .AddControllers(options => {

                // only allow anonymous access if there is no JWT secret...
                if (_env.IsDevelopment() && string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
                {
                    options.Filters.Add(new AllowAnonymousFilter());
                }
                options.EnableEndpointRouting = false;

            })
                
              .AddProblemDetailsConventions();


            services.AddSwaggerGen(c =>
            {               
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RSBC BC Mail Adapter", Version = "v1" });
                // add Xml comments to the swagger docs
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            

            // health checks. 
            services.AddHealthChecks()
                .AddCheck("bcmail-adapter", () => HealthCheckResult.Healthy("OK"));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configuration
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "RSBC DMF Services for DPS, DFWEB and DFCMS"));
                IdentityModelEventSource.ShowPII = true;
            }
            //app.UseHttpLogging();
            app.UseProblemDetails();
            app.UseForwardedHeaders();
            app.UseRouting();
            app.UseAuthentication();
            app.UseJwtBearerQueryString();
            app.UseAuthorization();

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "RSBC BC Mail Adapter");
                    });

                endpoints.MapControllers().RequireAuthorization();                 
            });

            // enable Splunk logger using Serilog
            if (!string.IsNullOrEmpty(Configuration["SPLUNK_COLLECTOR_URL"]) &&
                !string.IsNullOrEmpty(Configuration["SPLUNK_TOKEN"])
            )
            {
                var fields = new CustomFields();
                if (!string.IsNullOrEmpty(Configuration["SPLUNK_CHANNEL"]))
                    fields.CustomFieldList.Add(new CustomField("channel", Configuration["SPLUNK_CHANNEL"]));
                var splunkUri = new Uri(Configuration["SPLUNK_COLLECTOR_URL"]);
                
                // Fix for bad SSL issues 

                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console()
                    .WriteTo.EventCollector(Configuration["SPLUNK_COLLECTOR_URL"],
                        sourceType: "bcmailadapter", eventCollectorToken: Configuration["SPLUNK_TOKEN"],
                        restrictedToMinimumLevel: LogEventLevel.Information,
#pragma warning disable CA2000 // Dispose objects before losing scope
                        messageHandler: new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                            {
                                return true;
                            }
                        }
#pragma warning restore CA2000 // Dispose objects before losing scope
                    )
                    .CreateLogger();                
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console()
                    .CreateLogger();
            }
            Log.Logger.Information("RSBC BC Mail Adapter Container Started");
            SelfLog.Enable(Console.Error);
        }


        private void ConfigureProblemDetails(ProblemDetailsOptions options)
        {
            // Only include exception details in a development environment. There's really no need
            // to set this as it's the default behavior. It's just included here for completeness :)
            //options.IncludeExceptionDetails = (ctx, ex) => Environment.IsDevelopment();
            options.IncludeExceptionDetails = (ctx, ex) => true;

            // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
            // This is useful if you have upstream middleware that needs to do additional handling of exceptions.
            options.Rethrow<NotSupportedException>();

            // This will map NotImplementedException to the 501 Not Implemented status code.
            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

            // This will map HttpRequestException to the 503 Service Unavailable status code.
            options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

            // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
            // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
            //options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        }
    }


}