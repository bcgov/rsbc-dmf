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
        protected const string DOCUMENT_API_BASE = "/api/Document";

        public ApiIntegrationTestBase(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new CustomWebApplicationFactory(configuration)
                .CreateClient();
        }

        public void Dispose() => _client.Dispose();

        protected async Task<T> HttpClientSendRequest<T>(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
