using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.Interfaces.Icbc.Services;
using Xunit;

namespace Pssg.Unit.Tests.Interfaces.Icbc
{
    public class OAuth2TokenServiceIntegrationTest
    {
        [Fact]
        public async Task CanRetrieveToken_UsingUserSecrets()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets("Test")
                .AddEnvironmentVariables()
                .Build();

            var tokenEndpoint = configuration["ICBC_OAUTH2_TOKEN_ENDPOINT"];
            var clientId = configuration["ICBC_OAUTH2_CLIENT_ID"];
            var clientSecret = configuration["ICBC_OAUTH2_CLIENT_SECRET"];

            Assert.False(string.IsNullOrWhiteSpace(tokenEndpoint), "ICBC_OAUTH2_TOKEN_ENDPOINT is missing in secrets or environment.");
            Assert.False(string.IsNullOrWhiteSpace(clientId), "ICBC_OAUTH2_CLIENT_ID is missing in secrets or environment.");
            Assert.False(string.IsNullOrWhiteSpace(clientSecret), "ICBC_OAUTH2_CLIENT_SECRET is missing in secrets or environment.");

            var httpClient = new HttpClient();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<OAuth2TokenService>();
            var service = new OAuth2TokenService(httpClient, configuration, logger);

            // Act & Assert
            var token = await service.RefreshTokenAsync();
            Assert.False(string.IsNullOrWhiteSpace(token), "Failed to retrieve a valid OAuth2 token.");
        }
    }
}