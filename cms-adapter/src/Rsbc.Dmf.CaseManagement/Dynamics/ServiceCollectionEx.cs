using Microsoft.Extensions.DependencyInjection;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public static class ServiceCollectionEx
    {
        public static void AddDynamicsServices(this IServiceCollection services)
        {
            services.AddTransient<IMapper<incident, CaseDetail>, CaseMapper>();
        }
    }
}
