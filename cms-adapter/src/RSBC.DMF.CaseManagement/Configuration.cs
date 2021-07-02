using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RSBC.DMF.CaseManagement.Dynamics;

namespace RSBC.DMF.CaseManagement
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