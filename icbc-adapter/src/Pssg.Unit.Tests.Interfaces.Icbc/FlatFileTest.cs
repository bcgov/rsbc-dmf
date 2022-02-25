

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
    public class FlatFileTest
    {

        IConfiguration Configuration;
        FlatFileUtils flatFileUtils;

        /// <summary>
        /// Setup the test
        /// </summary>        
        public FlatFileTest()
        {
            Configuration = new ConfigurationBuilder()
                // The following line is the only reason we have a project reference for the document storage adapter.
                // If you were to use this code on a different project simply add user secrets as appropriate to match the environment / secret variables below.
                .AddUserSecrets<Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();
            flatFileUtils = new FlatFileUtils(Configuration);
        }


        [Fact]
        public async void BasicConnectionTest()
        {
            flatFileUtils.CheckForWork(null);
        }
    }
}
