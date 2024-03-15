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
            userRegistration.Email = "mason@mailinator.com";
            var request = new HttpRequestMessage(HttpMethod.Put, $"{PROFILE_API_BASE}/register");
            SetContent(request, userRegistration);
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        // NOTE this will pass but will have a login unless you delete the login first
        // you also need to replace the following lines in CustomWebApplicationFactory.cs
        // var driverId = "6709a2ab-9ea1-ed11-b83d-00505683fbf4";
        // new Claim(UserClaimTypes.FamilyName, "BUTTAR"),

        [Fact]
        public async Task User_Registration_No_Login()
        {
            var driverId = _configuration["DRIVER_WITH_NO_LOGIN"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var userRegistration = new UserRegistration();
            userRegistration.DriverLicenseNumber = "01000032";
            userRegistration.Email = "buttar@mailinator.com";
            var request = new HttpRequestMessage(HttpMethod.Put, $"{PROFILE_API_BASE}/register");
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
            userRegistration.Email = "johndoe@gmail.com";
            var request = new HttpRequestMessage(HttpMethod.Put, $"{PROFILE_API_BASE}/{nameof(ProfileController.Register)}");
            SetContent(request, userRegistration);
            var response = await _client.SendAsync(request);
            response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
        }
    }
}
