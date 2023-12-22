using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public abstract class ApiIntegrationTestBase
    {
        protected HttpClient _client { get; }
        protected readonly IConfiguration Configuration;
        protected const string CASE_API = "/api/Cases";

        public ApiIntegrationTestBase(HttpClientFixture fixture)
        {
            _client = fixture.Client;
            Configuration = fixture.Configuration;
        }

        protected async Task<T> HttpClientSendRequest<T>(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}