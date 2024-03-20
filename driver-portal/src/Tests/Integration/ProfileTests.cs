using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.DriverPortal.Api.Controllers;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests.Integration
{
    public class ProfileTests : ApiIntegrationTestBase
    {
        public ProfileTests(IConfiguration configuration) : base(configuration) { }

        [Fact]
        public async Task User_Registration()
        {
            var driverId = _configuration["DOCS_DRIVER_ID"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var userRegistration = new UserRegistration();
            userRegistration.DriverLicenseNumber = "00200173";
            userRegistration.Email = "mason1@mailinator.com";
            userRegistration.NotifyByEmail = true;
            userRegistration.NotifyByMail = true;
            userRegistration.Address = new CaseManagement.Service.FullAddress();
            userRegistration.Address.City = "Vancouver";
            userRegistration.Address.Country = "Canada";
            userRegistration.Address.Line1 = "100 Water Street";
            userRegistration.Address.Postal = "V3S 3P8";
            userRegistration.Address.Province = "BC";
            var request = new HttpRequestMessage(HttpMethod.Put, $"{PROFILE_API_BASE}/register");
            SetContent(request, userRegistration);
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        // NOTE this will pass but will have a login unless you delete the login first
        // you also need to replace the following lines in CustomWebApplicationFactory.cs
        // var driverId = "6709a2ab-9ea1-ed11-b83d-00505683fbf4";

        [Fact]
        public async Task User_Registration_No_Login()
        {
            var driverId = _configuration["DRIVER_WITH_NO_LOGIN"];
            if (string.IsNullOrEmpty(driverId))
                return;

            var userRegistration = new UserRegistration();
            userRegistration.DriverLicenseNumber = "01000032";
            userRegistration.Email = "buttar@mailinator.com";
            userRegistration.NotifyByMail = true;
            userRegistration.NotifyByEmail = true;
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
