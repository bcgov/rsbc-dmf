// See https://aka.ms/new-console-template for more information
// start by getting secrets.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rsbc.Dmf.CaseManagement;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Serilog.Core;


namespace Rsbc.Dmf.CaseManagement.Tools
{


    internal class Program
    {

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
        //.AddEnvironmentVariables()
        .AddUserSecrets<Program>();
            var Configuration = builder.Build();


            var context = new DynamicsContext( (new Uri(options.DynamicsApiBaseUri), new Uri(options.DynamicsApiEndpoint), async () => await tokenProvider.AcquireToken(), logger);

            IServiceCollection services = new ServiceCollection();
            services.Configure<DynamicsOptions>(opts => Configuration.GetSection("Dynamics").Bind(opts));
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
            services.AddDynamics(Configuration);
        }
    

}

