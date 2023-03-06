using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rsbc.Dmf.BcMailAdapter;
using Rsbc.Dmf.BcMailAdapter.Tests.Helpers;
using Rsbc.Interfaces;
using System.Net;
using System.Net.Http;

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

            // you could also create a real CdgsClient here if there was a valid URI.
            ICdgsClient cdgsClient;

            if (Configuration["CDGS_SERVICE_URI"] != null)
            {
                cdgsClient = new CdgsClient(Configuration);                
            }
            else
            {
                cdgsClient = CdgsClientHelper.CreateMock(Configuration);
            }

            builder
                .UseSolutionRelativeContentRoot("")
                .UseEnvironment("Staging")
                .UseConfiguration(Configuration)
                //.UseStartup<Startup>()
                .ConfigureTestServices(
               
                    services => {
                        services.AddTransient(_ => cdgsClient);
                    });
        }
    }

    
}
