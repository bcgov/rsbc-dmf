using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public class HttpClientFixture : IDisposable
    {
        public void Dispose() => Client.Dispose();
        public HttpClient Client { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IMapper Mapper { get; private set; }

        public HttpClientFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();

            var factory = new CustomWebApplicationFactory();
            Client = factory.CreateClient();
            Mapper = factory.Services.GetService<IMapper>();
        }
    }
}