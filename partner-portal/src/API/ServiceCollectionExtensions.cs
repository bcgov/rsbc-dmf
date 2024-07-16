using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Serilog;
using System.Net;

namespace Rsbc.Dmf.PartnerPortal.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCaseManagementAdapterClient(this IServiceCollection services, IConfiguration config)
        {
            var serviceUrl = config["CMS_ADAPTER_URI"];
            var clientSecret = config["CMS_ADAPTER_JWT_SECRET"];
            var validateServerCertificate = config.GetValue("CMS_VALIDATE_SERVER_CERT", true);
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                var httpClientHandler = new HttpClientHandler();
                if (!validateServerCertificate) // Ignore certificate errors in non-production modes.
                                                // This allows you to use OpenShift self-signed certificates for testing.
                {
                    // Return `true` to allow certificates that are untrusted/invalid
                    httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }

                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2. 
                httpClient.DefaultRequestVersion = HttpVersion.Version20;
                if (string.IsNullOrEmpty(clientSecret))
                {
                    // add the service without authentication.
                    var channel = GrpcChannel.ForAddress(serviceUrl, new GrpcChannelOptions { HttpClient = httpClient });

                    services.AddTransient(_ => new CaseManager.CaseManagerClient(channel));
                    services.AddTransient(_ => new UserManager.UserManagerClient(channel));
                    services.AddTransient(_ => new DocumentManager.DocumentManagerClient(channel));
                }
                else
                {
                    var initialChannel = GrpcChannel.ForAddress(serviceUrl, new GrpcChannelOptions { HttpClient = httpClient });
                    var initialClient = new CaseManager.CaseManagerClient(initialChannel);

                    // call the token service to get a token.
                    var tokenRequest = new Rsbc.Dmf.CaseManagement.Service.TokenRequest { Secret = clientSecret };

                    var tokenReply = initialClient.GetToken(tokenRequest);
                    if (tokenReply != null && tokenReply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
                    {
                        // Add the bearer token to the client.
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                        var channel = GrpcChannel.ForAddress(serviceUrl, new GrpcChannelOptions { HttpClient = httpClient });

                        services.AddTransient(_ => new CaseManager.CaseManagerClient(channel));
                        services.AddTransient(_ => new UserManager.UserManagerClient(channel));
                        services.AddTransient(_ => new DocumentManager.DocumentManagerClient(channel));
                    }
                    else
                    {
                        Log.Logger.Information("Error getting token for Case Management Service");
                    }
                }
            }

            return services;
        }

        public static void AddDocumentStorageClient(this IServiceCollection services, IConfiguration configuration)

        public static IServiceCollection AddIcbcAdapterClient(this IServiceCollection services, IConfiguration configuration)
        {
            // Add ICBC Adapter
            string icbcAdapterURI = configuration["ICBC_ADAPTER_URI"];
            if (!string.IsNullOrEmpty(icbcAdapterURI))
            {
                var httpClientHandler = new HttpClientHandler();

                // Return `true` to allow certificates that are untrusted/invalid                    
                httpClientHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                var httpClient = new HttpClient(httpClientHandler)
                {
                    Timeout = TimeSpan.FromMinutes(30),
                    DefaultRequestVersion = HttpVersion.Version20
                };

                if (!string.IsNullOrEmpty(configuration["ICBC_ADAPTER_JWT_SECRET"]))
                {
                    var initialChannel = GrpcChannel.ForAddress(icbcAdapterURI,
                    new GrpcChannelOptions
                    { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });
                    var initialClient = new IcbcAdapter.IcbcAdapter.IcbcAdapterClient(initialChannel);

                // call the token service to get a token.
                    var tokenRequest = new IcbcAdapter.TokenRequest
                {
                        Secret = configuration["ICBC_ADAPTER_JWT_SECRET"]
                };
                var tokenReply = initialClient.GetToken(tokenRequest);
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
 
