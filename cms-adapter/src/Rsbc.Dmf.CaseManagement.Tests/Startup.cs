using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.CaseManagement.Service;

namespace Rsbc.Dmf.CaseManagement.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Service.Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();

            services.AddDistributedMemoryCache();
            services.AddDynamics(configuration);
            services.AddDynamicsServices();
            services.AddTransient<ICaseManager, CaseManager>();
            services.AddTransient<IDocumentManager, DocumentManager>();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddAutoMapperSingleton();
        }
    }
}