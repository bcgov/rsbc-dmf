using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    [Collection(nameof(HttpClientCollection))]
    public class CaseTests : ApiIntegrationTestBase
    {
        public CaseTests(HttpClientFixture fixture) : base(fixture) { }

        [Fact]
        public async Task GetCase()
        {
            var caseId = _configuration["ICBC_TEST_CASEID"];
            if (!string.IsNullOrEmpty(caseId))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{CASE_API_BASE}/" + caseId);
                var clientResult = await HttpClientSendRequest<CaseDetail>(request);

                Assert.Equal(clientResult.CaseId, caseId);
            }
        }


        [Fact]
        public async Task GetLettersToDriver()
        {
            var caseId = _configuration["DOCS_CASE_ID"];
            if (!string.IsNullOrEmpty(caseId))
            {
                var driverId = _configuration["DOCS_DRIVER_ID"];

                // get case details
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
}