using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using System.Net.Http;

namespace RSBC.DMF.DoctorsPortal.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
              .Enrich.FromLogContext()
              .WriteTo.Console()
              .CreateBootstrapLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.WithMachineName()
                        .Enrich.WithProcessId()
                        .Enrich.WithProcessName()
                        .Enrich.FromLogContext()
                        .Enrich.WithExceptionDetails()
                        ;

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        loggerConfiguration.WriteTo.Console();
                    }
                    else
                    {
                        //configure default console logs to output json to allow Kibana indexing
                        loggerConfiguration.WriteTo.Console(formatter: new RenderedCompactJsonFormatter());

                        var splunkUrl = hostingContext.Configuration.GetValue("SPLUNK__URL", string.Empty);
                        var splunkToken = hostingContext.Configuration.GetValue("SPLUNK__TOKEN", string.Empty);
                        if (string.IsNullOrWhiteSpace(splunkToken) || string.IsNullOrWhiteSpace(splunkUrl))
                        {
                            Log.Warning($"Splunk logging sink is not configured properly, check SPLUNK_TOKEN and SPLUNK_URL env vars");
                        }
                        else
                        {
                            //configure splunk as an event collector
                            loggerConfiguration
                                .WriteTo.EventCollector(
                                    splunkHost: splunkUrl,
                                    eventCollectorToken: splunkToken,
                                    messageHandler: new HttpClientHandler
                                    {
                                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                                    },
                                    renderTemplate: false);
                        }
                    }
                }, writeToProviders: true)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}