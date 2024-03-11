using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Rsbc.Dmf.CaseManagement.Service;
using System.Net;
using System;

namespace Rsbc.Dmf.DriverPortal.Tests.Integration
{
    public class CallbackTests : ApiIntegrationTestBase
    {
        public CallbackTests(IConfiguration configuration) : base(configuration) { }

        [Fact]
        public async Task Create_Callback()
        {
            var driverId = _configuration["DRIVER_WITH_CALLBACKS"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var bringForwardRequest = new BringForwardRequest()
            {
                Assignee = string.Empty,
                Description = "Test Description1",
                Subject = "Driver Portal Error Test",
                Priority = BringForwardPriority.High
            };
            var request = new HttpRequestMessage(HttpMethod.Get, $"{CALLBACK_API_BASE}/create");
            SetContent(request, bringForwardRequest);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Driver_Get_Callbacks()
        {
            var driverId = _configuration["DRIVER_WITH_CALLBACKS"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{CALLBACK_API_BASE}/driver");
            var response = await HttpClientSendRequest<IEnumerable<ViewModels.Callback>>(request);

            Assert.NotNull(response);
        }

        // NOTE you can use create callback integration test to get a new callback id,
        // if the callback no longer exists in dynamics 
        [Fact]
        public async Task Cancel_Callback()
        {
            var callbackId = _configuration["CALLBACK_ID"];
            if (string.IsNullOrEmpty(callbackId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{CALLBACK_API_BASE}/cancel");
            SetContent(request, Guid.Parse(callbackId));

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
