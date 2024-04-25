using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RSBC.DMF.MedicalPortal.API.Services;

public class Startup
{
    /// <summary>
    /// Register dependencies needed for xunit tests
    /// NOTE to register dependencies used by making calls from HttpClient, use CustomWebApplicationFactory
    /// </summary>
    /// <param name="services">service collection</param>
    public void ConfigureServices(IServiceCollection services)
    {
        var _configuration = new ConfigurationBuilder()
            //.AddUserSecrets<ApplicationVersionInfo>()
            //.SetBasePath(
            .AddJsonFile("AppSettings.json")
            .AddEnvironmentVariables()
            .Build();
        services.AddSingleton<IConfiguration>(_configuration);

        services.AddTransient<ICaseQueryService, CaseService>();

        //using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
        //    .SetMinimumLevel(LogLevel.Trace)
        //    // TODO see XUnit DI package website for a potentially cleaner way to do XUnitLoggerProvider
        //    // see case adapter XUnitLoggerProvider for a logger that will work with xunit
        //    .AddConsole());
        //services.AddAutoMapperSingleton(loggerFactory);
    }
}
