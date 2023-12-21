using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public class HttpClientFixture : IDisposable
    {
        public void Dispose() => Client.Dispose();
        public HttpClient Client { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public HttpClientFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();

            Client = new CustomWebApplicationFactory<Program>().CreateClient();
        }
    }
}