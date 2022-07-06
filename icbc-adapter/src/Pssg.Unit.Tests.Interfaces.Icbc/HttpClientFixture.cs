using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    public class HttpClientFixture : IDisposable
    {
        public HttpClientFixture() {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();

            Client = new CustomWebApplicationFactory<Startup>().CreateClient();
        }

        public void Dispose() => Client.Dispose();
        public HttpClient Client { get; private set; }

        public IConfiguration Configuration { get; private set; }
    }
}
