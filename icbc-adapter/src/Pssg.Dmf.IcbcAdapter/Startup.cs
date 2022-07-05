using Grpc.Net.Client;
using Hangfire;
using Hangfire.MemoryStorage;
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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rsbc.Dmf.IcbcAdapter.Services;
using Rsbc.Dmf.CaseManagement.Service;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Splunk;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.SwaggerGen;
using Serilog.Context;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Pssg.Interfaces;

namespace Rsbc.Dmf.IcbcAdapter
{
    public class AllowAnonymous : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
                context.Succeed(requirement); //Simply pass all requirements

            return Task.CompletedTask;
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment _env;


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            
            services.AddCors(options =>
            {
                options.AddPolicy(name: "default",
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                                  });
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
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(o =>
                {
                    o.SaveToken = true;                   
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        ValidIssuer = Configuration["JWT_VALID_ISSUER"],
                        ValidAudience = Configuration["JWT_VALID_AUDIENCE"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]))
                    };
                });

                
            }
            else
            {
                services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
            }
            
            services.AddAuthorization();

            // basic REST controller for Dynamics.

            services.AddProblemDetails(ConfigureProblemDetails)
                
            .AddControllers(options => {
                if (_env.IsDevelopment())
                {
                    options.Filters.Add(new AllowAnonymousFilter());                    
                }


                //options.Filters.Add( typeof(ServerErrorExceptionFilterAttribute));

                options.EnableEndpointRouting = false;


                })
                    // Adds MVC conventions to work better with the ProblemDetails middleware.
                    .AddProblemDetailsConventions();

            //GlobalConfiguration.Configuration.Filters.Add(
            //new ServerErrorExceptionFilterAttribute());

            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaxReceiveMessageSize = 256 * 1024 * 1024; // 256 MB
                options.MaxSendMessageSize = 256 * 1024 * 1024; // 256 MB
            });

            // Hangfire is used for scheduled jobs
            services.AddHangfire(x => x.UseMemoryStorage());
            services.AddHangfireServer();

            services.AddEndpointsApiExplorer();

            // Swagger is used for API documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ICBC Adapter", Version = "v1" });
                c.EnableAnnotations();

                string baseUri = Configuration["BASE_URI"];
                if (baseUri != null)
                {
                    // ensure baseUri is in the right format.
                    baseUri = baseUri.TrimEnd('/') + @"/";

                    c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(baseUri + "authentication/redirect/" + Configuration["JWT_TOKEN_KEY"]),
                                Scopes = new Dictionary<string, string>
                                {
                                    {"openid", "oidc standard"}
                                }
                            }
                        }
                    });

                    c.OperationFilter<AuthenticationRequirementsOperationFilter>();

                }
                
            });

            // health checks. 
            services.AddHealthChecks()
                .AddCheck("document-storage-adapter", () => HealthCheckResult.Healthy("OK"));

            // Add Case Management System (CMS) Adapter 

            string cmsAdapterURI = Configuration["CMS_ADAPTER_URI"];

            if (!string.IsNullOrEmpty(cmsAdapterURI))
            {
                var httpClientHandler = new HttpClientHandler();
                if (!_env.IsProduction()) // Ignore certificate errors in non-production modes.  
                                          // This allows you to use OpenShift self-signed certificates for testing.
                {
                    // Return `true` to allow certificates that are untrusted/invalid                    
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }

                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                httpClient.DefaultRequestVersion = HttpVersion.Version20;

                if (!string.IsNullOrEmpty(Configuration["CMS_ADAPTER_JWT_SECRET"]))
                {
                    var initialChannel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });

                    var initialClient = new CaseManager.CaseManagerClient(initialChannel);
                    // call the token service to get a token.
                    var tokenRequest = new CaseManagement.Service.TokenRequest
                    {
                        Secret = Configuration["CMS_ADAPTER_JWT_SECRET"]
                    };

                    var tokenReply = initialClient.GetToken(tokenRequest);

                    if (tokenReply != null && tokenReply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                    {
                        // Add the bearer token to the client.
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
                    }
                }

                var channel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });
                services.AddTransient(_ => new CaseManager.CaseManagerClient(channel));
            }

            if (Configuration["ICBC_LOOKUP_SERVICE_URI"] != null)
            {
                services.AddTransient(_ => new IcbcClient(Configuration));
            }

        }

        private void ConfigureProblemDetails(ProblemDetailsOptions options)
        {
            // Only include exception details in a development environment. There's really no nee
            // to set this as it's the default behavior. It's just included here for completeness :)
            //options.IncludeExceptionDetails = (ctx, ex) => Environment.IsDevelopment();
            options.IncludeExceptionDetails = (ctx, ex) => false;


            // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
            // This is useful if you have upstream middleware that needs to do additional handling of exceptions.
            options.Rethrow<NotSupportedException>();

            // This will map NotImplementedException to the 501 Not Implemented status code.
            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

            // This will map HttpRequestException to the 503 Service Unavailable status code.
            options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

            // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
            // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction()) {
                
                //app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ICBC Adapter v1"));

            }

            app.UseProblemDetails();

            app.UseForwardedHeaders();

            app.UseCors("default");

            app.UseRouting();

            bool startHangfire = true;
#if DEBUG
            // do not start Hangfire if we are running tests.        
            foreach (var assem in Assembly.GetEntryAssembly().GetReferencedAssemblies())
            {
                if (assem.FullName.ToLowerInvariant().StartsWith("xunit") || assem.FullName.Contains("Unit.Tests"))
                {
                    startHangfire = false;
                    break;
                }
            }
#endif

            if (startHangfire)
            {
                // enable Hangfire, using the default authentication model (local connections only)
                app.UseHangfireServer();

                DashboardOptions dashboardOptions = new DashboardOptions
                {
                    AppPath = null
                };

                app.UseHangfireDashboard("/hangfire", dashboardOptions);
            }

            if (!string.IsNullOrEmpty(Configuration["ENABLE_HANGFIRE_JOBS"]))
            {
                SetupHangfireJobs(app);
            }

            app.UseAuthentication();
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
                endpoints.MapGrpcService<IcbcAdapterService>();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "RSBC ICBC Adapter");
                    });

                endpoints.MapControllers();

                
            });

            app.UseSerilogRequestLogging(
            options =>
            {
                options.MessageTemplate =
                    "{RemoteIpAddress} {RequestScheme} {RequestHost} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.EnrichDiagnosticContext = (
                    diagnosticContext,
                    httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                };
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
                        sourceType: "icbc-adapter", eventCollectorToken: Configuration["SPLUNK_TOKEN"],
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
            Log.Logger.Information("Document Storage Adapter Container Started");
            SelfLog.Enable(Console.Error);
        }

        // <summary>
        /// Setup the Hangfire jobs.
        /// </summary>
        /// <param name="app"></param>    
        private void SetupHangfireJobs(IApplicationBuilder app)
        {

            Log.Logger.Information("Starting setup of Hangfire job ...");

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    Log.Logger.Information("Creating Hangfire jobs for check candidates ...");

                    string interval = Cron.Daily();
                    if (!string.IsNullOrEmpty(Configuration["QUEUE_CHECK_INTERVAL"]))
                    {
                        interval = (Configuration["QUEUE_CHECK_INTERVAL"]);
                    }

                    var caseManagerClient = serviceScope.ServiceProvider.GetService<CaseManager.CaseManagerClient>();

                    RecurringJob.AddOrUpdate(() => new FlatFileUtils(Configuration, caseManagerClient).CheckForCandidates(null), interval);

                    RecurringJob.AddOrUpdate(() => new FlatFileUtils(Configuration, caseManagerClient).CheckConnection(null), interval);

                    RecurringJob.AddOrUpdate(() => new FlatFileUtils(Configuration, caseManagerClient).SendMedicalUpdates(null), interval);



                    Log.Logger.Information("Hangfire jobs setup.");
                }
            }
            catch (Exception e)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendLine("Failed to setup Hangfire job.");

                Log.Logger.Error(e, "Hangfire setup failed.");
            }
        }
    }

    /// <summary>
    /// Helper filter for Swagger authentication
    /// </summary>
    public class AuthenticationRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Security == null)
                operation.Security = new List<OpenApiSecurityRequirement>();


            var scheme = new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearer" } };
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [scheme] = new List<string>()
            });
        }
    }


}