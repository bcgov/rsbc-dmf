using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Helpers;
using Rsbc.Dmf.CaseManagement.Service;
using System.Net;
using System.Net.Http;
using Rsbc.Dmf.CaseManagement.Helpers;
using Pssg.Interfaces.Icbc.Services;
using Moq;
using Microsoft.Extensions.Logging;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<Startup>
    {
        public IConfiguration Configuration;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            Configuration = new ConfigurationBuilder()
        .AddUserSecrets<Startup>()
              .AddEnvironmentVariables()
      .Build();

            // Setup ICBC client with OAuth2 support for testing
            IIcbcClient icbcClient = SetupIcbcClient();

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
                    var initialChannel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

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

                var channel = GrpcChannel.ForAddress(cmsAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });
                caseManagerClient = new CaseManager.CaseManagerClient(channel);
            }

            builder
           .UseSolutionRelativeContentRoot("")
            .UseEnvironment("Staging")
              .UseConfiguration(Configuration)
            .ConfigureTestServices(services =>
            {
                services.AddTransient(_ => caseManagerClient);
                services.AddTransient(_ => icbcClient);

                // Configure OAuth2 test services if needed
                ConfigureOAuth2TestServices(services);
            });
        }

        private IIcbcClient SetupIcbcClient()
        {
            // Check if OAuth2 is enabled for testing
            bool useOAuth2 = Configuration.GetValue<bool>("ICBC_USE_OAUTH2", true);
            bool hasOAuth2Config = !string.IsNullOrEmpty(Configuration["ICBC_OAUTH2_TOKEN_ENDPOINT"]);
            bool hasLegacyConfig = !string.IsNullOrEmpty(Configuration["ICBC_SERVICE_URI"]);

            if (useOAuth2 && hasOAuth2Config)
            {
                // Create a mock OAuth2 token service for testing
                var mockTokenService = CreateMockOAuth2TokenService();

                // Use EnhancedIcbcClient with OAuth2 support
                return new EnhancedIcbcClient(Configuration, mockTokenService);
            }
            else if (hasLegacyConfig)
            {
                return new EnhancedIcbcClient(Configuration);
            }
            else
            {
                // Use mock client if no real configuration is available
                return IcbcHelper.CreateMock();
            }
        }

        private IOAuth2TokenService CreateMockOAuth2TokenService()
        {
            var mockTokenService = new Mock<IOAuth2TokenService>();

            // Setup mock to return a test token
            mockTokenService.Setup(x => x.GetAccessTokenAsync())
         .ReturnsAsync("test-oauth2-token-123");

            mockTokenService.Setup(x => x.RefreshTokenAsync())
          .ReturnsAsync("test-oauth2-token-123");

            return mockTokenService.Object;
        }

        private void ConfigureOAuth2TestServices(IServiceCollection services)
        {
            // Check if OAuth2 is being used in tests
            bool useOAuth2 = Configuration.GetValue<bool>("ICBC_USE_OAUTH2", true);

            if (useOAuth2)
            {
                // Add mock OAuth2 token service for testing
                services.AddSingleton<IOAuth2TokenService>(CreateMockOAuth2TokenService());

                // Add HttpClient for OAuth2TokenService (even though we're mocking it)
                services.AddHttpClient<OAuth2TokenService>();

                // Add logger for OAuth2TokenService
                services.AddSingleton<ILogger<OAuth2TokenService>>(provider =>
                    {
                        var loggerFactory = provider.GetService<ILoggerFactory>();
                        return loggerFactory?.CreateLogger<OAuth2TokenService>() ??
       new Mock<ILogger<OAuth2TokenService>>().Object;
                    });
            }
        }
    }
}
