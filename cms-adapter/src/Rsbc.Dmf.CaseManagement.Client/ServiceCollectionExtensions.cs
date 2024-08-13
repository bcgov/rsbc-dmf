using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using System.Net;
using System.Net.Http;

namespace Rsbc.Dmf.CaseManagement.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCaseManagementAdapterClient(this IServiceCollection services, IConfiguration config, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger(nameof(AddCaseManagementAdapterClient));

            var serviceUrl = config["CMS_ADAPTER_URI"];
            var clientSecret = config["CMS_ADAPTER_JWT_SECRET"];
            var validateServerCertificate = config["CMS_VALIDATE_SERVER_CERT"];
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                var httpClientHandler = new HttpClientHandler();
                // Ignore certificate errors in non-production modes.
                // This allows you to use OpenShift self-signed certificates for testing.
                if (string.IsNullOrEmpty(validateServerCertificate) || validateServerCertificate == "true") 
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
                    var tokenRequest = new TokenRequest { Secret = clientSecret };

                    var tokenReply = initialClient.GetToken(tokenRequest, new CallOptions().WithWaitForReady(true));

                    if (tokenReply != null && tokenReply.ResultStatus == ResultStatus.Success)
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
                        logger.LogError("Error getting token for Case Management Service");
                    }
                }
            }

            return services;
        }
    }
}
