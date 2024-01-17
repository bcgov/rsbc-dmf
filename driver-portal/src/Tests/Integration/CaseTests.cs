using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Collections.Generic;
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
            if (string.IsNullOrEmpty(caseId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{CASE_API_BASE}/" + caseId);
            var clientResult = await HttpClientSendRequest<CaseDetail>(request);

            Assert.Equal(clientResult.CaseId, caseId);
        }

        [Fact]
        public async Task Get_Closed_Cases()
        {
            var driverId = _configuration["DOCS_DRIVER_ID"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{CASE_API_BASE}/{driverId}/Closed");
            var result = await HttpClientSendRequest<IEnumerable<CaseDetail>>(request);

            Assert.NotNull(result);
        }
    }
}