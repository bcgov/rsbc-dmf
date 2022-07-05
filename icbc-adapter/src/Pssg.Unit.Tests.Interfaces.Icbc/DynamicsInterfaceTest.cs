

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
  
    public class DynamicsInterfaceTest : ApiIntegrationTestBaseWithLogin
    {

        public DynamicsInterfaceTest(CustomWebApplicationFactory<Startup> factory)
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
