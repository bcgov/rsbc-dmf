using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests.Integration
{
    [Collection(nameof(HttpClientCollection))]
    public class DocumentTests : ApiIntegrationTestBase
    {
        public DocumentTests(HttpClientFixture fixture) : base(fixture) { }

        [Fact]
        public async Task GetLettersToDriver()
        {
            var driverId = _configuration["DOCS_DRIVER_ID"];
            if (string.IsNullOrEmpty(driverId))
                return;

            // get documents by driver id
            var request = new HttpRequestMessage(HttpMethod.Get, $"{DRIVER_API_BASE}/{driverId}/Documents");
            var caseDocuments = await HttpClientSendRequest<CaseDocuments>(request);

            Assert.NotNull(caseDocuments);
        }

        [Fact]
        public async Task Get_All_Documents()
        {
            var driverId = _configuration["DOCS_DRIVER_ID"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{DRIVER_API_BASE}/{driverId}/AllDocuments");
            var result = await HttpClientSendRequest<IEnumerable<Document>>(request);

            Assert.NotNull(result);
        }
    }
}
