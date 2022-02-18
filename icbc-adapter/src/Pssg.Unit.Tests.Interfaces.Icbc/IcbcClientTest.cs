

using Microsoft.Extensions.Configuration;
using Pssg.IcbcAdapter;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;


namespace Pssg.IcbcAdapter.Tests
{
    public class IcbcClientTest
    {

        IConfiguration Configuration;

        private IcbcClient IcbcClient {  get; set;}

        /// <summary>
        /// Setup the test
        /// </summary>        
        public IcbcClientTest()
        {
            Configuration = new ConfigurationBuilder()
                // The following line is the only reason we have a project reference for the document storage adapter.
                // If you were to use this code on a different project simply add user secrets as appropriate to match the environment / secret variables below.
                .AddUserSecrets<Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();

            IcbcClient = new IcbcClient(Configuration);
        }


        [Fact]
        public async void GetDriverHistoryTest()
        {
            CLNT result = IcbcClient.GetDriverHistory(Configuration["ICBC_TEST_DL"]);
            Assert.NotNull(result);



            result = IcbcClient.GetDriverHistory(Configuration["ICBC_ALTERNATE_TEST_DL"]);
            Assert.NotNull(result);
            
        }
    }
}
