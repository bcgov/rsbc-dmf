using Newtonsoft.Json;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    [Collection(nameof(HttpClientCollection))]
    public class CaseTests : ApiIntegrationTestBase
    {
        public CaseTests(HttpClientFixture fixture): base(fixture) { }

        [Fact]
        public async Task GetCase()
        {
            var caseId = Configuration["ICBC_TEST_CASEID"];
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Cases/GetCase/" + caseId);

            var response = await _client.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var clientResult = JsonConvert.DeserializeObject<CaseDetail>(jsonString);

            Assert.Equal(clientResult.CaseId, caseId);
        }
    }
}