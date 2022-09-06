using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pssg.DocumentStorageAdapter;
using Pssg.DocumentStorageAdapter.Helpers;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Helpers;
using Rsbc.Dmf.CaseManagement.Helpers;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.LegacyAdapter;
using System.Net;
using System.Net.Http;

namespace Rsbc.Unit.Tests.Dmf.LegacyAdapter
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<Startup>
    {
        public IConfiguration Configuration;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            
            CaseManager.CaseManagerClient caseManagerClient;
            DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient = null;
            IIcbcClient icbcClient = null;

            Configuration = new ConfigurationBuilder()
                    .AddUserSecrets<Startup>()
                    .AddEnvironmentVariables()
                    .Build();

            if (Configuration["ICBC_LOOKUP_SERVICE_URI"] != null)
            {
                icbcClient = new IcbcClient(Configuration);
            }
            else
            {
                icbcClient = IcbcHelper.CreateMock();
            }

            // Document Storage Adapter

            // Add Document Storage Adapter

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

            string cmsAdapterURI = Configuration["CMS_ADAPTER_URI"] ?? string.Empty;

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
                caseManagerClient = new CaseManager.CaseManagerClient(channel);
            }

            builder
                .UseSolutionRelativeContentRoot("")
                .UseEnvironment("Staging")
                .UseConfiguration(Configuration)
                //.UseStartup<Startup>()
                .ConfigureTestServices(
               
                    services => {                         
                        services.AddTransient(_ => caseManagerClient);
                        services.AddTransient(_ => icbcClient);

                        if (documentStorageAdapterClient != null)
                        {
                            services.AddTransient(_ => documentStorageAdapterClient);
                        }
                        
                    });
        }
    }

    
}
