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
            var caseId = _configuration["ICBC_TEST_CASEID"];
            var request = new HttpRequestMessage(HttpMethod.Get, $"{CASE_API_BASE}/" + caseId);

            var clientResult = await HttpClientSendRequest<CaseDetail>(request);

            Assert.Equal(clientResult.CaseId, caseId);
        }

        [Fact]
        public async Task GetMostRecentCase()
        {
            var licenseNumber = Configuration["ICBC_TEST_DL"];
            //var licenseNumber = _configuration["ICBC_TEST_DL"];
            var request = new HttpRequestMessage(HttpMethod.Get, $"{CASE_API_BASE}/MostRecent/");

            var clientResult = await HttpClientSendRequest<CaseDetail>(request);

            Assert.NotNull(clientResult);
        }
    }
}