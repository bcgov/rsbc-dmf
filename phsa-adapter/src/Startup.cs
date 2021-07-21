using HealthChecks.UI.Client;
using Hl7.Fhir.Serialization;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PhsaAdapter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private readonly string MyAllowSpecificOrigins = "script-src 'self' 'unsafe-eval' 'unsafe-inline'";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin();
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
            /*
            .AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.Formatting = Formatting.Indented;
                opts.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                opts.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

                // ReferenceLoopHandling is set to Ignore to prevent JSON parser issues with the user / roles model.
                opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            */
            // JSON.NET
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Insert(0,
                    new JsonStringEnumConverter()
                );
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.RemoveAll<OutputFormatterSelector>();
            services.TryAddSingleton<OutputFormatterSelector, FhirOutputFormatterSelector>();

            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication",
                    options => { })
                .AddOAuth2Introspection("introspection", options =>
                {
                    options.SkipTokensWithDots = true;
                    Configuration.GetSection("auth").Bind(options);
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
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "phsa-adapter");
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PHSA Adapter", Version = "v1" });
            });

            // health checks.
            services.AddHealthChecks()
                .AddCheck("phsa-adapter", () => HealthCheckResult.Healthy("OK"));
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

            app.UseRouting();

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
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
            if (introspectionResponse.IsError) throw new Exception($"Error introspecting token: {introspectionResponse.ErrorType} - {introspectionResponse.Error}");
            if (!introspectionResponse.IsActive) throw new Exception($"Token {phsaReferenceToken} is not active");

            //TODO: remove '+' removal when PHSA fixes the JWT format
            var phsaIdToken = introspectionResponse.Claims.FirstOrDefault(c => c.Type == "id_token")?.Value.Trim().Replace("+", "");
            var sessionKey = new JwtSecurityTokenHandler().ReadJwtToken(phsaIdToken).Claims.FirstOrDefault(c => c.Type == "sessionKey")?.Value;
            if (sessionKey == null) return null;
            return Encoding.UTF8.GetString(Convert.FromBase64String(sessionKey));
        }
    }
}