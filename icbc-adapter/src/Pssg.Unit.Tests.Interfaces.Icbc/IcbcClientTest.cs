

using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.IcbcAdapter;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Moq;
using Pssg.Unit.Tests.Interfaces.Icbc.Helpers;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    public class IcbcClientTest
    {

        IConfiguration Configuration;

        private IIcbcClient IcbcClient {  get; set;}

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
            if (Configuration["ICBC_LOOKUP_SERVICE_URI"] != null)
            {
                IcbcClient = new IcbcClient(Configuration);
            }
            else
            {
                IcbcClient = IcbcHelper.CreateMock();
            }

        }


        [Fact]
        public void GetDriverHistoryTest()
        {
            CLNT result = IcbcClient.GetDriverHistory(Configuration["ICBC_TEST_DL"]);
            Assert.NotNull(result);



            result = IcbcClient.GetDriverHistory(Configuration["ICBC_ALTERNATE_TEST_DL"]);
            Assert.NotNull(result);            
        }
    }
}
