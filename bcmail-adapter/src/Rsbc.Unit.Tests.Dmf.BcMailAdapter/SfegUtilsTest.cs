using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Interfaces;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Rsbc.Dmf.BcMailAdapter.Tests
{
    public class SfegUtilsTest
    {
        IConfiguration Configuration;
        SfegUtils sfegUtils;
        
        CaseManager.CaseManagerClient CaseManagerClient { get; set; }

        /// <summary>
        /// Setup the test
        /// </summary>        
        public SfegUtilsTest()
        {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();

            sfegUtils = new SfegUtils(Configuration, CaseManagerClient);
        }

        [Fact]
        public async void CanSendDocumentsToBcMail()
        {
            
            await sfegUtils.SendDocumentsToBcMail();
        }
    }
}
