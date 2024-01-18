using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.DriverPortal.Api;
using Rsbc.Dmf.DriverPortal.ViewModels;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<ApplicationVersionInfo>()
                .AddEnvironmentVariables()
                .Build();

            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace)
                // see case adapter XUnitLoggerProvider for a logger that will work with xunit
                .AddConsole());

            services.AddSingleton<IConfiguration>(configuration);
            services.AddAutoMapperSingleton(loggerFactory);
        }
    }
}
