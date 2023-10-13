using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Rsbc.Dmf.BcMailAdapter.Tests
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
                .UseConfiguration(Configuration);
                //.UseStartup<Startup>()
                
        }
    }

    
}
