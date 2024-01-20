using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public abstract class ApiIntegrationTestBase : IDisposable
    {
        protected readonly HttpClient _client;
        protected readonly IConfiguration _configuration;
        protected const string CASE_API_BASE = "/api/Cases";
        protected const string DRIVER_API_BASE = "/api/Driver";

        public ApiIntegrationTestBase(IConfiguration configuration, bool enableAuthorization = false)
        {
            _configuration = configuration;
            _client = new CustomWebApplicationFactory(configuration, enableAuthorization)
                .CreateClient();
        }

        public void Dispose() => _client.Dispose();

        protected async Task<T> HttpClientSendRequest<T>(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}