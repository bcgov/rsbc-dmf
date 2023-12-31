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

        [Fact]
        public async Task GetLettersToDriver()
        {
            // get case detail with driver id
            var caseId = _configuration["DOCS_CASE_ID"];
            var driverId = _configuration["DOCS_DRIVER_ID"];
            var request = new HttpRequestMessage(HttpMethod.Get, $"{CASE_API_BASE}/{caseId}");
            var caseResult = await HttpClientSendRequest<CaseDetail>(request);

            Assert.Equal(caseResult.CaseId, caseId);
            Assert.Equal(caseResult.DriverId, driverId);

            // get documents by driver id
            request = new HttpRequestMessage(HttpMethod.Get, $"{DRIVER_API_BASE}/{driverId}/Documents");
            var caseDocuments = await HttpClientSendRequest<CaseDocuments>(request);

            Assert.NotNull(caseDocuments);
        }
    }
}