using Grpc.Net.Client;
using PidpAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Serilog;
using System.Net;

namespace RSBC.DMF.MedicalPortal.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCaseManagementAdapterClient(this IServiceCollection services, IConfiguration config)
        {
            var serviceUrl = config["CMS_SERVER_URL"];
            var clientSecret = config["CMS_ADAPTER_JWT_SECRET"];
            var validateServerCertificate = config.GetValue("CMS_VALIDATE_SERVER_CERT", true);
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

        public static IServiceCollection AddPidpAdapterClient(this IServiceCollection services, IConfiguration config)
        {
            var serviceUrl = config["PidpAdapter:ServerUrl"];
            var clientSecret = config["PidpAdapter:Secret"];
            var validateServerCertificate = config.GetValue("PidpAdapter:ValidateServerCertificate", true);
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

                var initialChannel = GrpcChannel.ForAddress(serviceUrl, new GrpcChannelOptions { HttpClient = httpClient });

                var initialClient = new PidpManager.PidpManagerClient(initialChannel);
                // call the token service to get a token.
                var tokenRequest = new PidpAdapter.TokenRequest { Secret = clientSecret };

                var tokenReply = initialClient.GetToken(tokenRequest);

                if (tokenReply != null && tokenReply.ResultStatus == PidpAdapter.ResultStatus.Success)
                {
                    // Add the bearer token to the client.
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                    var channel = GrpcChannel.ForAddress(serviceUrl, new GrpcChannelOptions { HttpClient = httpClient });

                    services.AddTransient(_ => new PidpManager.PidpManagerClient(channel));
                }
                else
                {
                    Log.Logger.Information("Error getting token for Pidp Service");
                }
            }
            return services;
        }
    }
}
