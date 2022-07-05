

using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.IcbcAdapter;
using Pssg.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.ViewModels;
using System.Web;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
  


    public class IcbcInterfaceTest : ApiIntegrationTestBaseWithLogin
    {

        public IcbcInterfaceTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
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
