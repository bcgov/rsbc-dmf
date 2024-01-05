using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AutoMapper;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public abstract class ApiIntegrationTestBase
    {
        protected HttpClient _client { get; }
        protected readonly IConfiguration _configuration;
        protected const string CASE_API_BASE = "/api/Cases";
        protected const string DRIVER_API_BASE = "/api/Drivers";
        protected const string CALLBACK_API_BASE = "/api/Callback";
        protected IMapper _mapper { get; }

        public ApiIntegrationTestBase(HttpClientFixture fixture)
        {
            _client = fixture.Client;
            _configuration = fixture.Configuration;
            _mapper = fixture.Mapper;
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