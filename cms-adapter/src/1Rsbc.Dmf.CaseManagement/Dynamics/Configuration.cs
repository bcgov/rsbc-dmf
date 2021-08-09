using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    internal static class Configuration
    {
        public static IServiceCollection AddDynamics(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DynamicsOptions>(opts => configuration.GetSection("Dynamics").Bind(opts));
            services.AddHttpClient("adfs_token", (sp, c) =>
            {
                var options = sp.GetRequiredService<IOptions<DynamicsOptions>>().Value;
                c.BaseAddress = new Uri(options.Adfs.OAuth2TokenEndpoint);
            });
            services.AddTransient<ISecurityTokenProvider, CachedADFSSecurityTokenProvider>();
            services.AddScoped(sp =>
            {
                var options = sp.GetRequiredService<IOptions<DynamicsOptions>>().Value;
                var tokenProvider = sp.GetRequiredService<ISecurityTokenProvider>();
                var logger = sp.GetRequiredService<ILogger<DynamicsContext>>();
                return new DynamicsContext(new Uri(options.DynamicsApiBaseUri), new Uri(options.DynamicsApiEndpoint), async () => await tokenProvider.AcquireToken(), logger);
            });

            return services;
        }
    }
}