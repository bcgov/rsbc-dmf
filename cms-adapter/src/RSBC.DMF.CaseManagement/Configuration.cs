using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rsbc.Dmf.CaseManagement.Dynamics;

namespace Rsbc.Dmf.CaseManagement
{
    public static class Configuration
    {
        public static IServiceCollection AddCaseManagement(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDynamics(configuration);
            services.AddTransient<ICaseManager, CaseManager>();
            return services;
        }
    }
}