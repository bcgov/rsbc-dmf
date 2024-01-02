using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rsbc.Dmf.CaseManagement.Service;
using System.Net;
using System.Net.Http;
using Rsbc.Dmf.CaseManagement.Helpers;
using Pssg.DocumentStorageAdapter;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;
using Pssg.DocumentStorageAdapter.Helpers;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public IConfiguration Configuration;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Configuration = new ConfigurationBuilder()
                    .AddUserSecrets<Program>()
                    .AddEnvironmentVariables()
                    .Build();

            DocumentStorageAdapterClient documentStorageAdapterClient = null;
            string documentStorageAdapterURI = Configuration["DOCUMENT_STORAGE_ADAPTER_URI"];

            if (string.IsNullOrEmpty(documentStorageAdapterURI))
            {
                // add the mock
                documentStorageAdapterClient = DocumentStorageHelper.CreateMock(Configuration);
            }
            else
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;


                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                httpClient.DefaultRequestVersion = HttpVersion.Version20;

                var initialChannel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });

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
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                    var channel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = httpClient });

                    documentStorageAdapterClient = new DocumentStorageAdapter.DocumentStorageAdapterClient(channel);
                }
            }

            string cmsAdapterURI = Configuration["CMS_ADAPTER_URI"];

            CaseManager.CaseManagerClient caseManagerClient;
            if (string.IsNullOrEmpty(cmsAdapterURI))
            {
                // setup from Mock
                caseManagerClient = CmsHelper.CreateMock(Configuration);
            }
            else
            {
                var httpClientHandler = new HttpClientHandler();
                // Return `true` to allow certificates that are untrusted/invalid                    
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
                caseManagerClient = new CaseManager.CaseManagerClient(channel);
            }

            builder
                .UseSolutionRelativeContentRoot("")
                .UseEnvironment("Staging")
                .UseConfiguration(Configuration)
                .ConfigureTestServices(
                    services => {
                        services.AddTransient(_ => caseManagerClient);
                        if (documentStorageAdapterClient != null)
                        {
                            services.AddTransient(_ => documentStorageAdapterClient);
                        }
                    });
        }
    }
}