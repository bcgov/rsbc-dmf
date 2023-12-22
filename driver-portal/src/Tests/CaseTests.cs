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

        private const string CASE_API = "/api/Cases";

        [Fact]
        public async Task GetCase()
        {
            var caseId = Configuration["ICBC_TEST_CASEID"];
            var request = new HttpRequestMessage(HttpMethod.Get, $"{CASE_API}/GetCase/ " + caseId);

            var clientResult = await Send<CaseDetail>(request);

            Assert.Equal(clientResult.CaseId, caseId);
        }

        [Fact]
        public async Task GetMostRecentCase()
        {
            var licenseNumber = Configuration["ICBC_TEST_DL"];
            var request = new HttpRequestMessage(HttpMethod.Get, $"{CASE_API}/GetMostRecentCase/" + licenseNumber);

            var clientResult = await Send<CaseDetail>(request);

            Assert.NotNull(clientResult);
        }
    }
}