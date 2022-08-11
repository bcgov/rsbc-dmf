using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Xunit;
using Pssg.Interfaces.Icbc.ViewModels;
using System.Net;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    [Collection(nameof(HttpClientCollection))]
    public class IcbcInterfaceTest : ApiIntegrationTestBase
    {
        public IcbcInterfaceTest(HttpClientFixture fixture)
            : base(fixture)
        { }

        [Fact]
        public void TestInvalidLogin()
        {
            string testDl = Configuration["ICBC_TEST_DL"];

            var request = new HttpRequestMessage(HttpMethod.Get, "/Authentication/Token?secret=InvalidSecret");
            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var token = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(token))
            {
                // Add the bearer token to the client.
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }

            request = new HttpRequestMessage(HttpMethod.Post, "/Icbc/Candidates");

            request.Content = new StringContent("[]", Encoding.UTF8, "application/json");
           
            response = _client.SendAsync(request).GetAwaiter().GetResult();
            
            Assert.Equal (HttpStatusCode.Unauthorized, response.StatusCode);

        }

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
        /// Test the Empty Candidates List
        /// </summary>
        [Fact]
        public async void TestEmptyCandidatesList()
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

        /// <summary>
        /// Test zulu time
        /// </summary>
        [Fact]
        public async void TestZuluDateCandidatesList()
        {
            string testDl = Configuration["ICBC_TEST_DL"];

            Login();

            var request = new HttpRequestMessage(HttpMethod.Post, "/Icbc/Candidates");

            request.Content = new StringContent("[{\"dlNumber\":\""+testDl
                +"\",\"effectiveDate\":\"2022-06-22T00:00:00.000Z\"}]", Encoding.UTF8, "application/json");
            var response = _client.SendAsync(request).GetAwaiter().GetResult();


            // parse as JSON.
            var jsonString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Test zulu time
        /// </summary>
        [Fact]
        public async void TestGMT8List()
        {
            string testDl = Configuration["ICBC_TEST_DL"];

            Login();

            var request = new HttpRequestMessage(HttpMethod.Post, "/Icbc/Candidates");

            string data = "[{\"dlNumber\":\"" + testDl
                + "\",\"effectiveDate\":\"2022-06-22T08:00:00.000Z\"}]";

            request.Content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = _client.SendAsync(request).GetAwaiter().GetResult();


            // parse as JSON.
            var jsonString = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
        }

       

    }
}
