using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    [Collection(nameof(HttpClientCollection))]
    public class CallbackTests : ApiIntegrationTestBase
    {
        public CallbackTests(HttpClientFixture fixture) : base(fixture) { }

        [Fact]
        public async Task Get_Callbacks_Service()
        {
            var driverId = _configuration["DRIVER_WITH_CALLBACKS"];
            if (!string.IsNullOrEmpty(driverId))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{CALLBACK_API_BASE}/" + driverId);
                var clientResult = await HttpClientSendRequest<DriverCallbacks>(request);

                Assert.Equal(clientResult.DriverId, driverId);
            }
        }

        // NOTE unit test, consider moving to a unit test project later
        [Fact]
        public void Map_Callback_To_ViewModel()
        {
            var callback = new CaseManagement.Service.Callback();
            callback.Id = _configuration["DRIVER_WITH_CALLBACKS"];
            callback.RequestCallback = new Google.Protobuf.WellKnownTypes.Timestamp();
            callback.Topic = CaseManagement.Service.Callback.Types.CallbackTopic.Upload;
            callback.CallStatus = CaseManagement.Service.Callback.Types.CallbackCallStatus.Open;
            callback.ClosedDate = new Google.Protobuf.WellKnownTypes.Timestamp();

            var callbackViewModel = _mapper.Map<Callback>(callback);

            Assert.NotNull(callbackViewModel);
        }
    }
}
