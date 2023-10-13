using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Interfaces;
using System.Net;
using System.Net.Http;
using Xunit;
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;
using Rsbc.Dmf.CaseManagement.Helpers;

using Pssg.DocumentStorageAdapter;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;

namespace Rsbc.Dmf.BcMailAdapter.Tests
{
    public class SfegUtilsTest
    {
        IConfiguration Configuration;
        SftpUtils sfegUtils;
        
        CaseManagerClient _caseManagerClient { get; set; }
        DocumentStorageAdapterClient _documentStorageAdapterClient { get; set; }

        /// <summary>
        /// Setup the test
        /// </summary>        
        public SfegUtilsTest()
        {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();

            // Add Case Management System (CMS) Adapter 

            string cmsAdapterURI = Configuration["CMS_ADAPTER_URI"];

            if (string.IsNullOrEmpty(cmsAdapterURI))
            {
                // setup from Mock
                _caseManagerClient = CmsHelper.CreateMock(Configuration);
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
                _caseManagerClient = new CaseManager.CaseManagerClient(channel);


                // DOCUMENT STORAGE CLIENT


                var documentStorageHttpClient = new HttpClient(httpClientHandler);

                string documentStorageAdapterURI = Configuration["DOCUMENT_STORAGE_ADAPTER_URI"];



                if (!string.IsNullOrEmpty(documentStorageAdapterURI))
                {
                    var initialChannel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = documentStorageHttpClient });

                    var initialClient = new DocumentStorageAdapter.DocumentStorageAdapterClient(initialChannel);
                    // call the token service to get a token.
                    var tokenRequest = new Pssg.DocumentStorageAdapter.TokenRequest
                    {
                        Secret = Configuration["DOCUMENT_STORAGE_ADAPTER_JWT_SECRET"]
                    };

                    var tokenReply = initialClient.GetToken(tokenRequest);

                    if (tokenReply != null && tokenReply.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
                    {
                        // Add the bearer token to the client.
                        documentStorageHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
                    }

                    var documentStorageChannel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = documentStorageHttpClient });
                    _documentStorageAdapterClient = new DocumentStorageAdapter.DocumentStorageAdapterClient(documentStorageChannel);


                }

            }
            sfegUtils = new SftpUtils(Configuration, _caseManagerClient, _documentStorageAdapterClient);
        }

        [Fact]
        public void CanVerifyConnection()
        {

            sfegUtils.SendDocumentsToBcMail();
        }

        [Fact]
        public void CanSendDocumentsToBcMail()
        {
            
            sfegUtils.SendDocumentsToBcMail();
        }
    }
}
