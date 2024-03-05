using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Collections.Generic;

namespace Rsbc.Dmf.DriverPortal.Tests.Integration
{
    public class CallbackTests : ApiIntegrationTestBase
    {
        public CallbackTests(IConfiguration configuration) : base(configuration) { }

        [Fact]
        public async Task Driver_Get_Callbacks()
        {
            var driverId = _configuration["DRIVER_WITH_CALLBACKS"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{CALLBACK_API_BASE}/driver");
            var response = await HttpClientSendRequest<IEnumerable<Callback>>(request);

            Assert.NotNull(response);
        }
    }
}
