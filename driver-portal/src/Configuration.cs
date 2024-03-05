using Grpc.Net.Client;
using Pssg.DocumentStorageAdapter;
using System.Net;
using Rsbc.Dmf.CaseManagement.Service;

namespace Rsbc.Dmf.DriverPortal.Api
{
    public static class Configuration
    {
        public static void AddDocumentStorageClient(this IServiceCollection services, IConfiguration configuration)
        {
            var documentStorageAdapterURI = configuration["DOCUMENT_STORAGE_ADAPTER_URI"];
            if (!string.IsNullOrEmpty(documentStorageAdapterURI))
            {
                var httpClientHandler = new HttpClientHandler();

                // Return `true` to allow certificates that are untrusted/invalid                    
                httpClientHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                httpClient.DefaultRequestVersion = HttpVersion.Version20;

                var initialChannel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                var initialClient = new DocumentStorageAdapter.DocumentStorageAdapterClient(initialChannel);
                // call the token service to get a token.
                var tokenRequest = new Pssg.DocumentStorageAdapter.TokenRequest
                {
                    Secret = configuration["DOCUMENT_STORAGE_ADAPTER_JWT_SECRET"]
                };

                var tokenReply = initialClient.GetToken(tokenRequest);

                if (tokenReply != null && tokenReply.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
                {
                    // Add the bearer token to the client.
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                    var channel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                    services.AddTransient(_ => new DocumentStorageAdapter.DocumentStorageAdapterClient(channel));
                }
            }
        }

        public static void AddCaseManagementAdapterClient(this IServiceCollection services, IConfiguration configuration)
        {
            string cmsAdapterURI = configuration["CMS_ADAPTER_URI"];

            if (!string.IsNullOrEmpty(cmsAdapterURI))
            {
                var httpClientHandler = new HttpClientHandler();

                // Return `true` to allow certificates that are untrusted/invalid                    
                httpClientHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                httpClient.DefaultRequestVersion = HttpVersion.Version20;

                if (!string.IsNullOrEmpty(configuration["CMS_ADAPTER_JWT_SECRET"]))
                {
                    var initialChannel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                    var initialClient = new CaseManager.CaseManagerClient(initialChannel);
                    // call the token service to get a token.
                    var tokenRequest = new CaseManagement.Service.TokenRequest
                    {
                        Secret = configuration["CMS_ADAPTER_JWT_SECRET"]
                    };

                    var tokenReply = initialClient.GetToken(tokenRequest);

                    if (tokenReply != null && tokenReply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                    {
                        // Add the bearer token to the client.
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
                    }
                }

                var channel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });
                services.AddTransient(_ => new CaseManager.CaseManagerClient(channel));
                services.AddTransient(_ => new CallbackManager.CallbackManagerClient(channel));
                services.AddTransient(_ => new UserManager.UserManagerClient(channel));
                services.AddTransient(_ => new DocumentManager.DocumentManagerClient(channel));
            }
        }
    }
}
