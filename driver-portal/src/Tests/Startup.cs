using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.DriverPortal.Api;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();

            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole());

            services.AddSingleton<IConfiguration>(configuration);
            services.AddAutoMapperSingleton(loggerFactory);
        }
    }
}
