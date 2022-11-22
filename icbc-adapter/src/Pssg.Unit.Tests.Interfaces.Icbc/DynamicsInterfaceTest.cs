using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pssg.Interfaces.Icbc.ViewModels;
using System.Net.Http;
using Xunit;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    [Collection(nameof(HttpClientCollection))]
    public class DynamicsInterfaceTest : ApiIntegrationTestBase
    {
        public DynamicsInterfaceTest(HttpClientFixture fixture)
            : base(fixture)
        { }


        private async void TestDl(string testDl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/DriverHistory?driversLicence=" + testDl);

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            // parse as JSON.
            var jsonString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            

            Driver clientResult = JsonConvert.DeserializeObject<Driver>(jsonString);

            int result = 0;
            int.TryParse(testDl, out result);
            // content should match

            Assert.Equal(clientResult.DriverMasterStatus.LicenceNumber.Value, result);
        }

        /// <summary>
        /// Test the MS Dynamics interface
        /// </summary>
        [Fact]
        public void TestDriverHistory()
        {
            string testDl = Configuration["ICBC_TEST_DL"];

            Login();

            TestDl(testDl);

            testDl = Configuration["ICBC_ALTERNATE_TEST_DL"];            

            TestDl(testDl);

        }
    }
}
