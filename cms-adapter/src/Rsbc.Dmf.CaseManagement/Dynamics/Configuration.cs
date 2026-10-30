using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    internal static class Configuration
    {
        public static IServiceCollection AddDynamics(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DynamicsOptions>(opts => configuration.GetSection("Dynamics").Bind(opts));

            services
                .AddHttpClient(
                    "adfs_token",
                    (sp, c) =>
                    {
                        var options = sp.GetRequiredService<IOptions<DynamicsOptions>>().Value;
                        if (!string.IsNullOrWhiteSpace(options.Adfs.OAuth2TokenEndpoint))
                        {
                            c.BaseAddress = new Uri(options.Adfs.OAuth2TokenEndpoint);
                        }
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                        {
                            return true;
                        },
                    }
                );

            services.AddHttpClient("entraid_token");

            services.AddMemoryCache();
            services.AddTransient<AdfsSecurityTokenProvider>();
            services.AddTransient<EntraIdSecurityTokenProvider>();
            services.AddTransient<ISecurityTokenProvider, SecurityTokenProvider>();
            services.AddScoped(sp =>
            {
                var options = sp.GetRequiredService<IOptions<DynamicsOptions>>().Value;
                var tokenProvider = sp.GetRequiredService<ISecurityTokenProvider>();
                var logger = sp.GetRequiredService<ILogger<DynamicsContext>>();
                return new DynamicsContext(
                    new Uri(options.DynamicsApiBaseUri),
                    new Uri(options.DynamicsApiEndpoint),
                    tokenProvider,
                    logger
                );
            });

            return services;
        }
    }
}
