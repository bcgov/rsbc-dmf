using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Net.Http;
using HealthChecks.UI.Client;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Splunk;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rsbc.Dmf.PhsaAdapter.Handlers;
using Rsbc.Dmf.PhsaAdapter.Formatters;
using Spark.Engine.Formatters;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.Interfaces.IcbcAdapter;

namespace Rsbc.Dmf.PhsaAdapter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "script-src 'self' 'unsafe-eval' 'unsafe-inline'";

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
                
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.Formatting = Formatting.Indented;
                    opts.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    opts.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

                    opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

                    // ReferenceLoopHandling is set to Ignore to prevent JSON parser issues with the user / roles model.
                    opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                
             // JSON.NET
                /*
            .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Insert(0,
                    new JsonStringEnumConverter()
                );
            })*/
                
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0); 
            
            services.RemoveAll<OutputFormatterSelector>();
            services.TryAddSingleton<OutputFormatterSelector, FhirOutputFormatterSelector>();

            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication",
                    options => { });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("BasicAuthentication",
                    new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "PHSA Adapter", Version = "v1"});
            });

            // health checks. 
            services.AddHealthChecks()
                .AddCheck("phsa-adapter", () => HealthCheckResult.Healthy("OK"));

            // Add ICBC adapter

            services.AddHttpClient<IIcbcClient, IcbcClient>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PHSA Adapter v1"));
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
    }
}