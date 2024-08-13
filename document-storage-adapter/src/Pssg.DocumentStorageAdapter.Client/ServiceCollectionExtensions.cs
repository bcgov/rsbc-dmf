using Grpc.Core;
using Grpc.Net.Client;
using System.Net.Http;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Pssg.DocumentStorageAdapter.Client
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDocumentStorageClient(this IServiceCollection services, IConfiguration configuration)
        {
            var documentStorageAdapterURI = configuration["DOCUMENT_STORAGE_ADAPTER_URI"];
            if (!string.IsNullOrEmpty(documentStorageAdapterURI))
            {
                var httpClientHandler = new HttpClientHandler();

                // Return `true` to allow certificates that are untrusted/invalid                    
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                httpClient.DefaultRequestVersion = HttpVersion.Version20;

                var grpcChannelOptions = new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null };
                var initialChannel = GrpcChannel.ForAddress(documentStorageAdapterURI, grpcChannelOptions);

                var initialClient = new DocumentStorageAdapter.DocumentStorageAdapterClient(initialChannel);
                // call the token service to get a token.
                var tokenRequest = new TokenRequest
                {
                    Secret = configuration["DOCUMENT_STORAGE_ADAPTER_JWT_SECRET"]
                };

                var tokenReply = initialClient.GetToken(tokenRequest, new CallOptions().WithWaitForReady(true));
                if (tokenReply != null && tokenReply.ResultStatus == ResultStatus.Success)
                {
                    // Add the bearer token to the client.
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                    var channel = GrpcChannel.ForAddress(documentStorageAdapterURI, grpcChannelOptions);

                    services.AddTransient(_ => new DocumentStorageAdapter.DocumentStorageAdapterClient(channel));
                }
            }
        }
    }
}
