using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rsbc.Dmf.BcMailAdapter;
using System.Net;
using System.Net.Http;

namespace Rsbc.Unit.Tests.Dmf.BcMailAdapter
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<Startup>
    {
        public IConfiguration Configuration;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            
            

            Configuration = new ConfigurationBuilder()
                    .AddUserSecrets<Startup>()
                    .AddEnvironmentVariables()
                    .Build();

            

            builder
                .UseSolutionRelativeContentRoot("")
                .UseEnvironment("Staging")
                .UseConfiguration(Configuration)
                //.UseStartup<Startup>()
                .ConfigureTestServices(
               
                    services => {                         
                        
                        
                    });
        }
    }

    
}
