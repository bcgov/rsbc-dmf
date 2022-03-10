using Grpc.Net.Client;
using HealthChecks.UI.Client;
using Hl7.Fhir.Serialization;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Pssg.DocumentStorageAdapter;
using Pssg.Rsbc.Dmf.DocumentTriage;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.Interfaces.IcbcAdapter;
using Rsbc.Dmf.PhsaAdapter.Formatters;
using Rsbc.Dmf.PhsaAdapter.Handlers;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Splunk;
using Spark.Engine.Formatters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PhsaAdapter
{
    public class Startup
    {

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        private readonly string MyAllowSpecificOrigins = "script-src 'self' 'unsafe-eval' 'unsafe-inline'";

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment _env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        if (Configuration["CORS_ORIGINS"] != null)
                        {
                            string[] origins = Configuration["CORS_ORIGINS"].Split(" ");
                            builder.WithOrigins(origins);
                        }
                        else
                        {
                            builder.AllowAnyOrigin();
                        }
                        
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();

                        /*
                        builder.WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS", "FETCH");
                        builder.WithHeaders("X-FHIR-Starter", "Origin", "Accept", "X-Requested-With", "Content-Type",
                            "Access-Control-Request-Method", "Access-Control-Request-Headers", "Authorization",
                            "Location","Content-Location");
                        */
                    });
            });

            ParserSettings parserSettings = new ParserSettings();
            SerializerSettings serializerSettings = new SerializerSettings();

            services.TryAddSingleton((provider) => new FhirJsonParser(parserSettings));
            services.TryAddSingleton((provider) => new FhirXmlParser(parserSettings));
            services.TryAddSingleton((provder) => new FhirJsonSerializer(serializerSettings));
            services.TryAddSingleton((provder) => new FhirXmlSerializer(serializerSettings));

            services.TryAddTransient<ResourceJsonInputFormatter>();
            services.TryAddTransient<ResourceJsonOutputFormatter>();

            services.AddControllers(options =>
                {
                    options.InputFormatters.RemoveType<SystemTextJsonInputFormatter>();
                    options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
                    // We remove StringOutputFormatter to make Swagger happy by not
                    // showing text/plain in the list of available media types.
                    options.OutputFormatters.RemoveType<StringOutputFormatter>();

                    options.InputFormatters.Add(new AsyncResourceJsonInputFormatter(new FhirJsonParser(null)));
                    options.InputFormatters.Add(new BinaryInputFormatter());
                    options.OutputFormatters.Add(new AsyncResourceJsonOutputFormatter());
                    options.OutputFormatters.Add(new BinaryOutputFormatter());
                })

                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.Formatting = Formatting.Indented;
                    opts.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    opts.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

                    opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

                    // ReferenceLoopHandling is set to Ignore to prevent JSON parser issues with the user / roles model.
                    opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.RemoveAll<OutputFormatterSelector>();
            services.TryAddSingleton<OutputFormatterSelector, FhirOutputFormatterSelector>();

            // Add an authorization handler that will be used to determine if FHIR OAUTH is enabled.
            services.AddSingleton<IAuthorizationHandler, FhirOauthRequirementHandler>();

            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication",
                    options => { })
                .AddOAuth2Introspection("introspection", options =>
                {
                    options.SkipTokensWithDots = true;
                    Configuration.GetSection("auth:introspection").Bind(options);
                    
                    options.TokenRetriever = req =>
                    {
                        var authHeader = (string)req.Headers["Authorization"];
                        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) return null;
                        var phsaReferenceToken = authHeader?.Substring("Bearer ".Length).Trim();
                        if (string.IsNullOrEmpty(phsaReferenceToken)) return null;

                        var logger = req.HttpContext.RequestServices.GetRequiredService<ILogger<OAuth2IntrospectionDefaults>>();
                        logger.LogInformation("PHSA reference token: {0}", phsaReferenceToken);

                        var userReferenceToken = ExtractUserReferenceTokenFromPhsaToken(phsaReferenceToken).GetAwaiter().GetResult();

                        logger.LogInformation("user reference token: {0}", userReferenceToken);
                        return userReferenceToken;
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("BasicAuthentication",
                    new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build());
                options.AddPolicy("OAuth", policy =>
                {
                    policy.AddAuthenticationSchemes("introspection");
                    //policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new FhirOauthRequirement());                    
                });                
                
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PHSA Adapter", Version = "v1" });
            });

            // health checks.
            services.AddHealthChecks()
                .AddCheck("phsa-adapter", () => HealthCheckResult.Healthy("OK"));

            // Add ICBC adapter

            services.AddHttpClient<IIcbcClient, IcbcClient>();

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

            // Add Document Storage Adapter

            string documentStorageAdapterURI = Configuration["DOCUMENT_STORAGE_ADAPTER_URI"];

            if (!string.IsNullOrEmpty(documentStorageAdapterURI))
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

                var initialChannel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = httpClient  });

                var initialClient = new DocumentStorageAdapter.DocumentStorageAdapterClient(initialChannel);
                // call the token service to get a token.
                var tokenRequest = new Pssg.DocumentStorageAdapter.TokenRequest
                {
                    Secret = Configuration["DOCUMENT_STORAGE_ADAPTER_JWT_SECRET"]
                };

                var tokenReply = initialClient.GetToken(tokenRequest);

                if (tokenReply != null && tokenReply.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
                {
                    // Add the bearer token to the client.
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                    var channel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });

                    services.AddTransient(_ => new DocumentStorageAdapter.DocumentStorageAdapterClient(channel));
                }
            }

            string documentTriageServiceURI = Configuration["DOCUMENT_TRIAGE_SERVICE_URI"];

            if (!string.IsNullOrEmpty(documentTriageServiceURI))
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

                var initialChannel = GrpcChannel.ForAddress(documentTriageServiceURI, new GrpcChannelOptions { HttpClient = httpClient });

                var initialClient = new DocumentTriage.DocumentTriageClient(initialChannel);
                // call the token service to get a token.
                var tokenRequest = new Pssg.Rsbc.Dmf.DocumentTriage.TokenRequest 
                {
                    Secret = Configuration["DOCUMENT_TRIAGE_SERVICE_JWT_SECRET"]
                };

                var tokenReply = initialClient.GetToken(tokenRequest);

                if (tokenReply != null && tokenReply.ResultStatus == Pssg.Rsbc.Dmf.DocumentTriage.ResultStatus.Success)
                {
                    // Add the bearer token to the client.
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                    var channel = GrpcChannel.ForAddress(documentTriageServiceURI, new GrpcChannelOptions { HttpClient = httpClient });

                    services.AddTransient(_ => new DocumentTriage.DocumentTriageClient(channel));
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PHSA Adapter v1"));
                IdentityModelEventSource.ShowPII = true;
            }

            app.UseCors(MyAllowSpecificOrigins);

            // enable Splunk logger using Serilog
            if (!string.IsNullOrEmpty(Configuration["SPLUNK_COLLECTOR_URL"]) &&
                !string.IsNullOrEmpty(Configuration["SPLUNK_TOKEN"])
            )
            {
                var fields = new CustomFields();
                if (!string.IsNullOrEmpty(Configuration["SPLUNK_CHANNEL"]))
                    fields.CustomFieldList.Add(new CustomField("channel", Configuration["SPLUNK_CHANNEL"]));

                // Fix for bad SSL issues

                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console()
                    .WriteTo.EventCollector(Configuration["SPLUNK_COLLECTOR_URL"],
                        sourceType: "phsa", eventCollectorToken: Configuration["SPLUNK_TOKEN"],
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

            SelfLog.Enable(Console.Error);

            Log.Logger.Information("PHSA Adapter Container Starting");

            app.UsePathBase(Configuration["BASE_PATH"] ?? "");
            app.UseRouting();

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

            app.UseAuthentication();
            app.UseCors(MyAllowSpecificOrigins);

            // by positioning this after the health check, no need to filter out health checks from request logging.
            app.UseSerilogRequestLogging();

            app.UseAuthorization();
            app.UseMiddleware<FormatTypeHandler>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private async Task<string> ExtractUserReferenceTokenFromPhsaToken(string phsaReferenceToken)
        {
            try
            {
                var client = new HttpClient();
                var introspectionResponse = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
                {
                    Address = Configuration["FHIR_INTROSPECTION_ENDPOINT"],
                    ClientId = Configuration["FHIR_OAUTH_CLIENT_ID"],
                    ClientSecret = Configuration["FHIR_OAUTH_CLIENT_SECRET"],
                    Method = HttpMethod.Post,
                    ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader,
                    AuthorizationHeaderStyle = BasicAuthenticationHeaderStyle.Rfc2617,
                    Token = phsaReferenceToken
                });
                if (introspectionResponse.IsError)
                    throw new Exception(
                        $"Error introspecting token: {introspectionResponse.ErrorType} - {introspectionResponse.Error}");
                if (introspectionResponse == null) throw new Exception($"Token is null");
                if (introspectionResponse.Claims.FirstOrDefault() != null &&
                    introspectionResponse.Claims.FirstOrDefault().Type == "error")
                {
                    return null;
                }
                if (!introspectionResponse.IsActive) throw new Exception($"Token {phsaReferenceToken} is not active");

                //TODO: remove '+' removal when PHSA fixes the JWT format
                var phsaIdToken = introspectionResponse.Claims.FirstOrDefault(c => c.Type == "id_token")?.Value.Trim()
                    .Replace("+", "");
                var sessionKey = new JwtSecurityTokenHandler().ReadJwtToken(phsaIdToken).Claims
                    .FirstOrDefault(c => c.Type == "sessionKey")?.Value;
                if (sessionKey == null) return null;
                return Encoding.UTF8.GetString(Convert.FromBase64String(sessionKey));
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "ExtractUserReferenceTokenFromPhsaToken error");
                return null;
            }
            
        }
    }
}