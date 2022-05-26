

using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.LegacyAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Rsbc.Unit.Tests.Dmf.LegacyAdapter
{

    public class DpsTest : ApiIntegrationTestBaseWithLogin
    {
        public string testDl;
        public string testSurcode;
        public DpsTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {

            testDl = Configuration["ICBC_TEST_DL"];
            testSurcode = Configuration["ICBC_TEST_SURCODE"];
        }


        private void Login()
        {
            // TODO - do a JWT login

        }

        /// <summary>
        /// Test the case exist service - parameters are license number and surcode.
        /// </summary>
        [Fact]
        public async void DoesCaseExist()
        {            
            Login();

            var request = new HttpRequestMessage(HttpMethod.Get, "/Dps/Exist?driversLicense=" + testDl + "&surcode=" + testSurcode);

            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }

    }

}
