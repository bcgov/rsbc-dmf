using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Web;
using System.Threading.Tasks;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public abstract class ApiIntegrationTestBase
    {
        protected HttpClient _client { get; }
        protected readonly IConfiguration Configuration;

        public ApiIntegrationTestBase(HttpClientFixture fixture)
        {
            _client = fixture.Client;
            Configuration = fixture.Configuration;
        }

        protected void Login()
        {
            // determine if authentication is enabled.
            if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
            {
                string encodedSecret = HttpUtility.UrlEncode(Configuration["JWT_TOKEN_KEY"]);
                var request = new HttpRequestMessage(HttpMethod.Get, "/Authentication/Token?secret=" + encodedSecret);
                var response = _client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                var token = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(token))
                {
                    // Add the bearer token to the client.
                    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
            }
        }

        protected async Task<T> Send<T>(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}