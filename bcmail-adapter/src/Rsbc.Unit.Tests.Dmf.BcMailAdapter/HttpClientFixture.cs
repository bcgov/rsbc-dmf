using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.BcMailAdapter;
using System;
using System.Net.Http;

namespace Rsbc.Dmf.BcMailAdapter.Tests
{
    public class HttpClientFixture : IDisposable
    {
        public HttpClientFixture() {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();
            if (!string.IsNullOrEmpty(Configuration["TEST_BASE_URI"]))
            {
                Client = new HttpClient();
                Client.BaseAddress = new Uri(Configuration["TEST_BASE_URI"]);
            }
            else
            {
                Client = new CustomWebApplicationFactory<Startup>().CreateClient();
            }
            
        }

        public void Dispose() => Client.Dispose();
        public HttpClient Client { get; private set; }

        public IConfiguration Configuration { get; private set; }
    }
}
