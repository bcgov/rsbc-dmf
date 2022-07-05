

using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.IcbcAdapter;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Rsbc.Dmf.CaseManagement.Service;
using System.Net.Http;
using System.Net;
using Grpc.Net.Client;
using Pssg.Unit.Tests.Interfaces.Icbc.Helpers;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    public class IcbcLoadTest
    {

        IConfiguration Configuration;

        private IIcbcClient IcbcClient { get; set; }

        private CaseManager.CaseManagerClient CaseManagerClient { get; set; }

        /// <summary>
        /// Setup the test
        /// </summary>        
        public IcbcLoadTest()
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

            // Add Case Management System (CMS) Adapter 

            string cmsAdapterURI = Configuration["CMS_ADAPTER_URI"];

            if (string.IsNullOrEmpty(cmsAdapterURI))
            {
                // setup from Mock
                CaseManagerClient = CmsHelper.CreateMock();
            }
            else
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                

                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                httpClient.DefaultRequestVersion = HttpVersion.Version20;

                if (!string.IsNullOrEmpty(Configuration["CMS_ADAPTER_JWT_SECRET"]))
                {
                    var initialChannel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });

                    var initialClient = new CaseManager.CaseManagerClient(initialChannel);
                    // call the token service to get a token.
                    var tokenRequest = new CaseManagement.Service.TokenRequest
                    {
                        Secret = Configuration["CMS_ADAPTER_JWT_SECRET"]
                    };

                    var tokenReply = initialClient.GetToken(tokenRequest);

                    if (tokenReply != null && tokenReply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                    {
                        // Add the bearer token to the client.
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
                    }
                }

                var channel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });
                CaseManagerClient = new CaseManager.CaseManagerClient(channel);
            }

        }

        private void TestTombstone (int recordLimit)
        {
            int counter = recordLimit;

            var reply = CaseManagerClient.GetDrivers(new EmptyRequest());

            int passed = 0;
            int errors = 0;

            foreach (var item in reply.Items)
            {
                if (counter == 0)
                {
                    break;
                }
                CLNT c = null;
                try
                {
                    c = IcbcClient.GetDriverHistory(item.DriverLicenseNumber);
                }
                catch (Exception)
                {
                    errors++;
                }
                if (c != null)
                {
                    passed++;
                }


                if (counter > -1)
                {
                    counter--;
                }
                
            }
            if (recordLimit < 0)
            {
                Assert.Equal(reply.Items.Count, passed );
            }
            else
            {
                if (reply.Items.Count < recordLimit)
                {
                    recordLimit = reply.Items.Count;
                }
                Assert.Equal(recordLimit, passed );
            }
            
        }

        [Fact]
        public void TestOneRecord()
        {
            TestTombstone(1);
        }

        [Fact]
        public void Test50Records()
        {
            TestTombstone(50);
        }

        [Fact]
        public void TestAllRecords()
        {
            TestTombstone(-1);
        }


    }
}
