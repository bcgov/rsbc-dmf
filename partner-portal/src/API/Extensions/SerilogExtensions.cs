using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

public static class SerilogExtensions
{
    public static IServiceCollection AddSerilogBootstrapLogger(this IServiceCollection services)
    {
        return services.AddSerilog(
            new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger()
        );
    }

    public static IServiceCollection AddSerilogLogger(this IServiceCollection services, IConfiguration config, AppConfig appConfig)
    {
        return services.AddSerilog((loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(config)
                    .Enrich.WithMachineName()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails();

                if (appConfig.IsDevelopment())
                {
                    loggerConfiguration.WriteTo.Console();
                }
                else
                {
                    //configure default console logs to output json to allow Kibana indexing
                    loggerConfiguration.WriteTo.Console(formatter: new RenderedCompactJsonFormatter());

                    var splunkUrl = appConfig.Splunk?.Url;
                    var splunkToken = appConfig.Splunk?.Token;
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
            });
    }
}
