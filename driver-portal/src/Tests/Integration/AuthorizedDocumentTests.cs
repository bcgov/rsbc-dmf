using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests.Integration
{
    public class AuthorizedDocumentTests
    {
        public class AuthorizationDocumentTests : ApiIntegrationTestBase
        {
            public AuthorizationDocumentTests(IConfiguration configuration) : base(configuration, true) { }

            [Fact]
            public async Task Driver_Authorized()
            {
                var driverId = _configuration["DRIVER_WITH_USER"];
                if (string.IsNullOrEmpty(driverId))
                    return;

                // get documents by driver id
                var request = new HttpRequestMessage(HttpMethod.Get, $"{DRIVER_API_BASE}/{driverId}/Documents");
                var caseDocuments = await HttpClientSendRequest<CaseDocuments>(request);

                Assert.NotNull(caseDocuments);
            }

            [Fact]
            public async Task Driver_Not_Authorized()
            {
                var driverId = _configuration["DRIVER_WITH_CALLBACKS"];
                if (string.IsNullOrEmpty(driverId))
                    return;

                // get documents by driver id
                var request = new HttpRequestMessage(HttpMethod.Get, $"{DRIVER_API_BASE}/{driverId}/Documents");

                var response = await _client.SendAsync(request);

                Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }
    }
}
