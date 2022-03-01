

using FileHelpers;
using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.IcbcAdapter;
using Pssg.Interfaces;
using Pssg.Interfaces.FlatFileModels;
using Pssg.Interfaces.Icbc.FlatFileModels;
using Pssg.Interfaces.Icbc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;


namespace Rsbc.Dmf.IcbcAdapter.Tests
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

        [Fact]
        public async void TestGetUpdates()
        {
            flatFileUtils.CheckForWork(null);
        }

        [Fact]
        public async void TestSendUpdates()
        {
            flatFileUtils.SendMedicalUpdates(null);
        }

        [Fact]
        public async void CheckDriverNewFileFormat()
        {
            var engine = new FileHelperEngine<NewDriver>();
            string sampleData = "2222222022222224EXPERIMENTAL_______________________2012-01-011M2002-02-012004-01-011998-01-012002-04-040100";
            var records = engine.ReadString(sampleData);
            Assert.Equal(records[0].LicenseNumber, sampleData.Substring(0,7));
        }

        [Fact]
        public async void CheckMedicalUpdateFormat()
        {
            var engine = new FileHelperEngine<MedicalUpdate>();
            string sampleData = "2222222EXPERIMENTAL_______________________P2012-01-01";
            var records = engine.ReadString(sampleData);
            Assert.Equal(records[0].LicenseNumber, sampleData.Substring(0, 7));
        }
    }
}
