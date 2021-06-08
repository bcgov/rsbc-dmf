using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
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

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.Converters.Insert(0,
                        new JsonStringEnumConverter()
                    );
                });

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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // by positioning this after the health check, no need to filter out health checks from request logging.
            app.UseSerilogRequestLogging();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });


        }
    }
}