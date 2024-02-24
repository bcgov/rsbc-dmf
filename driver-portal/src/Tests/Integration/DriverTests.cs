using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.DriverPortal.Api.Controllers;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests.Integration
{
    public class DriverTests : ApiIntegrationTestBase
    {
        public DriverTests(IConfiguration configuration) : base(configuration) { }
        
        [Fact]
        public async Task GetLettersToDriver()
        {
            var driverId = _configuration["DRIVER_WITH_USER"];
            if (string.IsNullOrEmpty(driverId))
                return;

            // get documents by driver id
            var request = new HttpRequestMessage(HttpMethod.Get, $"{DRIVER_API_BASE}/Documents");
            var caseDocuments = await HttpClientSendRequest<CaseDocuments>(request);

            Assert.NotNull(caseDocuments);
        }

        [Fact]
        public async Task Get_All_Documents()
        {
            var driverId = _configuration["DOCS_DRIVER_ID"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{DRIVER_API_BASE}/AllDocuments");
            var result = await HttpClientSendRequest<IEnumerable<Document>>(request);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task User_Registration()
        {
            var driverId = _configuration["DOCS_DRIVER_ID"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var userRegistration = new UserRegistration();
            userRegistration.DriverLicenseNumber = "00200173";
            userRegistration.FirstName = "";
            userRegistration.LastName = "MASON";
            userRegistration.Email = "mason@mailinator.com";
            userRegistration.BirthDate = new System.DateTime(1916, 7, 15);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{DRIVER_API_BASE}/{nameof(DriverController.UserRegistration)}");
            SetContent(request, userRegistration);
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task User_Registration_Wrong_DL()
        {
            var driverId = _configuration["DOCS_DRIVER_ID"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var userRegistration = new UserRegistration();
            userRegistration.DriverLicenseNumber = "1234567";
            userRegistration.FirstName = "John";
            userRegistration.LastName = "Doe";
            userRegistration.Email = "johndoe@gmail.com";
            userRegistration.BirthDate = new System.DateTime(1980, 1, 1);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{DRIVER_API_BASE}/{nameof(DriverController.UserRegistration)}");
            SetContent(request, userRegistration);
            var response = await _client.SendAsync(request);
            response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
        }
    }
}
