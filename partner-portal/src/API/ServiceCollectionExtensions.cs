using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pssg.DocumentStorageAdapter;
using Serilog;
using System.Net;

namespace Rsbc.Dmf.PartnerPortal.Api
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
                httpClientHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                httpClient.DefaultRequestVersion = HttpVersion.Version20;

                var initialChannel = GrpcChannel.ForAddress(documentStorageAdapterURI,
                    new GrpcChannelOptions
                    { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                var initialClient = new DocumentStorageAdapter.DocumentStorageAdapterClient(initialChannel);
                // call the token service to get a token.
                var tokenRequest = new Pssg.DocumentStorageAdapter.TokenRequest
                {
                    Secret = configuration["DOCUMENT_STORAGE_ADAPTER_JWT_SECRET"]
                };

                var tokenReply = initialClient.GetToken(tokenRequest, new CallOptions().WithWaitForReady(true));

                if (tokenReply != null && tokenReply.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
                {
                    // Add the bearer token to the client.
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                    var channel = GrpcChannel.ForAddress(documentStorageAdapterURI,
                        new GrpcChannelOptions
                        { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                    services.AddTransient(_ => new DocumentStorageAdapter.DocumentStorageAdapterClient(channel));
                }
            }
        }

        public static IServiceCollection AddIcbcAdapterClient(this IServiceCollection services, IConfiguration configuration)
        {
            // Add ICBC Adapter
            string icbcAdapterURI = configuration["ICBC_ADAPTER_URI"];
            if (!string.IsNullOrEmpty(icbcAdapterURI))
            {
                var httpClientHandler = new HttpClientHandler();
                // Return `true` to allow certificates that are untrusted/invalid                    
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                var httpClient = new HttpClient(httpClientHandler)
                {
                    Timeout = TimeSpan.FromMinutes(30),
                    DefaultRequestVersion = HttpVersion.Version20
                };

                if (!string.IsNullOrEmpty(configuration["ICBC_ADAPTER_JWT_SECRET"]))
                {
                    var initialChannel = GrpcChannel.ForAddress(icbcAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });
                    var initialClient = new IcbcAdapter.IcbcAdapter.IcbcAdapterClient(initialChannel);

                    // call the token service to get a token.
                    var tokenRequest = new IcbcAdapter.TokenRequest
                    {
                            Secret = configuration["ICBC_ADAPTER_JWT_SECRET"]
                    };

                    var tokenReply = initialClient.GetToken(tokenRequest, new CallOptions().WithWaitForReady(true));

                    if (tokenReply != null && tokenReply.ResultStatus == IcbcAdapter.ResultStatus.Success)
                    {
                        // Add the bearer token to the client.
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");
                        Log.Logger.Information("GetToken successfuly.");

                        var channel = GrpcChannel.ForAddress(icbcAdapterURI,
                        new GrpcChannelOptions
                        { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });
                        services.AddTransient(_ => new IcbcAdapter.IcbcAdapter.IcbcAdapterClient(channel));
                    }
                    else
                    {
                        Log.Logger.Information("GetToken failed {0}.", tokenReply?.ErrorDetail);
                    }
                }
            }

            return services;
        }
    }
}
