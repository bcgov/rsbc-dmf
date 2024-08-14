using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.IcbcAdapter;
using System.Net;
using static Rsbc.Dmf.IcbcAdapter.IcbcAdapter;

namespace Pssg.Dmf.IcbcAdapter.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIcbcAdapterClient(this IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger(nameof(AddIcbcAdapterClient));

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
                    var grpcChannelOptions = new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null };
                    var initialChannel = GrpcChannel.ForAddress(icbcAdapterURI, grpcChannelOptions);
                    var initialClient = new IcbcAdapterClient(initialChannel);

                    // call the token service to get a token.
                    var tokenRequest = new TokenRequest
                    {
                        Secret = configuration["ICBC_ADAPTER_JWT_SECRET"]
                    };

                    var tokenReply = initialClient.GetToken(tokenRequest, new CallOptions().WithWaitForReady(true));

                    if (tokenReply != null && tokenReply.ResultStatus == ResultStatus.Success)
                    {
                        // Add the bearer token to the client.
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                        var channel = GrpcChannel.ForAddress(icbcAdapterURI, grpcChannelOptions);
                        services.AddTransient(_ => new IcbcAdapterClient(channel));
                    }
                    else
                    {
                        logger.LogError("GetToken failed {0}.", tokenReply?.ErrorDetail);
                    }
                }
            }

            return services;
        }
    }
}