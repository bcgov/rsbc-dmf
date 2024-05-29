using Grpc.Net.Client;
using Microsoft.AspNetCore.Identity;
using pdipadapter.Infrastructure.Auth;
using Rsbc.Dmf.CaseManagement;
using Rsbc.Dmf.CaseManagement.Service;
using Serilog;
using System.Net;

namespace PidpAdapter.API.Infrastructure;
public static class CmsAdapterConfiguration
{
    public static IServiceCollection AddCmsAdapterGrpcService(this IServiceCollection services, PdipadapterConfiguration config)
    {
        var serviceUrl = config.CMS.SERVER_URL;
        var clientSecret = config.CMS.CLIENTSECRET;
        var validateServerCertificate = config.CMS.VALIDATESERVERCERTIFICATE;
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            var httpClientHandler = new HttpClientHandler();
            if (!validateServerCertificate) // Ignore certificate errors in non-production modes.
                                            // This allows you to use OpenShift self-signed certificates for testing.
            {
                // Return `true` to allow certificates that are untrusted/invalid
                httpClientHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }

            var httpClient = new HttpClient(httpClientHandler);
            // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
            httpClient.DefaultRequestVersion = HttpVersion.Version20;
            if (string.IsNullOrEmpty(clientSecret))
            {
                // add the service without authentication.
                var channel = GrpcChannel.ForAddress(serviceUrl, new GrpcChannelOptions { HttpClient = httpClient });

                services.AddTransient(_ => new CaseManager.CaseManagerClient(channel));
                services.AddTransient(_ => new UserManager.UserManagerClient(channel));
            }
            else
            {
                var initialChannel = GrpcChannel.ForAddress(serviceUrl, new GrpcChannelOptions { HttpClient = httpClient });

                var initialClient = new CaseManager.CaseManagerClient(initialChannel);
                // call the token service to get a token.
                var tokenRequest = new TokenRequest { Secret = clientSecret };

                var tokenReply = initialClient.GetToken(tokenRequest);

                if (tokenReply != null && tokenReply.ResultStatus == ResultStatus.Success)
                {
                    // Add the bearer token to the client.
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                    var channel = GrpcChannel.ForAddress(serviceUrl, new GrpcChannelOptions { HttpClient = httpClient });

                    services.AddTransient(_ => new CaseManager.CaseManagerClient(channel));
                    services.AddTransient(_ => new UserManager.UserManagerClient(channel));
                }
                else
                {
                    Log.Logger.Information("Error getting token for Case Management Service");
                }
            }

        }
        return services;
    }
}

