using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Pssg.Interfaces.Icbc.Helpers;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces;
using Rsbc.Dmf.CaseManagement.Helpers;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.BcMailAdapter;
using System.Net.Http;
using System;
using Xunit;
using System.Net;
using Rsbc.Dmf.BcMailAdapter.ViewModels;

namespace Rsbc.Unit.Tests.Dmf.BcMailAdapter
{
    [Collection(nameof(HttpClientCollection))]

    public class LoadTest : ApiIntegrationTestBase

    {

        IConfiguration Configuration;


        private CaseManager.CaseManagerClient CaseManagerClient { get; set; }

        /// <summary>
        /// Setup the test
        /// </summary>        
        public LoadTest(HttpClientFixture fixture)
            : base(fixture)
        {

            Configuration = new ConfigurationBuilder()
                // The following line is the only reason we have a project reference for the document storage adapter.
                // If you were to use this code on a different project simply add user secrets as appropriate to match the environment / secret variables below.
                .AddUserSecrets<Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();

           

            // Add Case Management System (CMS) Adapter 

            string cmsAdapterURI = Configuration["CMS_ADAPTER_URI"];

            if (string.IsNullOrEmpty(cmsAdapterURI))
            {
                // setup from Mock
                CaseManagerClient = CmsHelper.CreateMock(Configuration);
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
                    var tokenRequest = new Rsbc.Dmf.CaseManagement.Service.TokenRequest
                    {
                        Secret = Configuration["CMS_ADAPTER_JWT_SECRET"]
                    };

                    var tokenReply = initialClient.GetToken(tokenRequest);

                    if (tokenReply != null && tokenReply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
                    {
                        // Add the bearer token to the client.
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
                    }
                }

                var channel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });
                CaseManagerClient = new CaseManager.CaseManagerClient(channel);
            }

        }

        private void TestDriver(int recordLimit)
        {
            Login();

            int counter = recordLimit;

            var reply = CaseManagerClient.GetDrivers(new EmptyRequest());

            int passed = 0;
            int errors = 0;

            foreach (var item in reply.Items)
            {

                CLNT c = null;
                try
                {
                    // test get documents

                    var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{item.DriverLicenseNumber}/Documents");
                    
                    var response = _client.SendAsync(request).GetAwaiter().GetResult();

                    var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    response.EnsureSuccessStatusCode();

                    // test get comments

                    request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{item.DriverLicenseNumber}/Comments");

                    response = _client.SendAsync(request).GetAwaiter().GetResult();

                    responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    response.EnsureSuccessStatusCode();

                    passed++;
                }
                catch (Exception)
                {
                    errors++;
                }


                if (counter > -1)
                {
                    counter--;
                }
                if (counter == 0)
                {
                    break;
                }
            }
            if (recordLimit < 0)
            {
                Assert.Equal(reply.Items.Count, passed);
                Assert.Equal(0, errors);
            }
            else
            {
                if (reply.Items.Count < recordLimit)
                {
                    recordLimit = reply.Items.Count;
                }
                Assert.Equal(recordLimit, passed);
                Assert.Equal(0, errors);
            }

        }

        [Fact]
        public void TestOneRecord()
        {
            TestDriver(1);
        }

        [Fact]
        public void Test50Records()
        {
            TestDriver(50);
        }

        [Fact]
        public void TestAllRecords()
        {
            TestDriver(-1);
        }


    }
}
