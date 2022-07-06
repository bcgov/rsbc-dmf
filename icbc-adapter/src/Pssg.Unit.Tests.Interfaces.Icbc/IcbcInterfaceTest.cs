using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Xunit;
using Pssg.Interfaces.Icbc.ViewModels;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    [Collection(nameof(HttpClientCollection))]
    public class IcbcInterfaceTest : ApiIntegrationTestBase
    {
        public IcbcInterfaceTest(HttpClientFixture fixture)
            : base(fixture)
        { }


        private async void TestDl(string testDl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/DriverHistory?driversLicence=" + testDl);

            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            var jsonString = await response.Content.ReadAsStringAsync();

            Driver clientResult = JsonConvert.DeserializeObject<Driver>(jsonString);

            // content should match

            Assert.Equal(clientResult.DriverMasterStatus.LicenceNumber.Value, int.Parse(testDl));
        }

        /// <summary>
        /// Test the Candidates List
        /// </summary>
        [Fact]
        public async void TestCandidatesList()
        {
            string testDl = Configuration["ICBC_TEST_DL"];

            Login();

            var request = new HttpRequestMessage(HttpMethod.Post, "/Icbc/Candidates");

            request.Content = new StringContent("[]", Encoding.UTF8, "application/json");
            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            var jsonString = await response.Content.ReadAsStringAsync();
        }
    }
}
