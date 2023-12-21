using Newtonsoft.Json;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System;
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
            // TODO move to configuration
            var caseId = "aa937e8f-171b-ec11-b82d-00505683fbf4";
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8080/api/Cases/GetCase/" + caseId);

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            // parse as JSON.
            var jsonString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            CaseDetail clientResult = JsonConvert.DeserializeObject<CaseDetail>(jsonString);

            // content should match
            Assert.Equal(clientResult.CaseId, caseId);
        }
    }
}