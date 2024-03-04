using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rsbc.Dmf.CaseManagement.Dynamics;

namespace Rsbc.Dmf.CaseManagement
{
    public static class Configuration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDynamics(configuration);
            services.AddTransient<ICaseManager, CaseManager>();
            services.AddTransient<ICssManager, CssManager>();
            services.AddTransient<IUserManager, UserManager>();            
            services.AddTransient<IDocumentManager, DocumentManager>();
            services.AddTransient<IDocumentTypeManager, DocumentTypeManager>();
            return services;
        }
    }
}