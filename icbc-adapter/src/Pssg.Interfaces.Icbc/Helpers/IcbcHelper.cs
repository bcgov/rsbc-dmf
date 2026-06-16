using Microsoft.Extensions.Configuration;
using Moq;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.Services;
using Pssg.Interfaces.IcbcModels;
using Microsoft.Extensions.Logging;

namespace Pssg.Interfaces.Icbc.Helpers
{
    public static class IcbcHelper
    {
        public static IIcbcClient CreateMock()
        {
            var icbcClient = new Mock<IIcbcClient>();
            icbcClient.Setup(x => x.NormalizeDl(It.IsAny<string>(), It.IsAny<IConfiguration>()))
                .Returns<string, IConfiguration>((x, c) => { return x; });

            icbcClient
                .Setup(x => x.GetDriverHistory(It.IsAny<string>()))
                .Returns<string>(x => {
                    string licenceNumber = $"{x}";

                    return new CLNT() { DR1MST = new DR1MST() { LNUM = licenceNumber } };
                }); // clientResult.DriverMasterStatus.LicenceNumber

            icbcClient
                .Setup(x => x.SendMedicalUpdate(It.IsAny<IcbcMedicalUpdate>()))
                .Returns("SUCCESS"); // Mock successful medical update

            return icbcClient.Object;
        }

        /// <summary>
        /// Creates a mock OAuth2TokenService for testing
        /// </summary>
        /// <returns>Mock IOAuth2TokenService</returns>
        public static IOAuth2TokenService CreateMockOAuth2TokenService()
        {
            var mockTokenService = new Mock<IOAuth2TokenService>();

            // Setup mock to return a test token
            mockTokenService.Setup(x => x.GetAccessTokenAsync())
                .ReturnsAsync("test-oauth2-access-token");

            mockTokenService.Setup(x => x.RefreshTokenAsync())
                .ReturnsAsync("test-oauth2-access-token-refreshed");

            return mockTokenService.Object;
        }

        /// <summary>
        /// Creates an EnhancedIcbcClient with mocked OAuth2 service for testing
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        /// <returns>EnhancedIcbcClient with mock OAuth2 service</returns>
        public static EnhancedIcbcClient CreateMockEnhancedClient(IConfiguration configuration)
        {
            var mockTokenService = CreateMockOAuth2TokenService();
            return new EnhancedIcbcClient(configuration, mockTokenService);
        }

        /// <summary>
        /// Creates an EnhancedIcbcClient with legacy authentication for testing
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        /// <returns>EnhancedIcbcClient with legacy authentication</returns>
        public static EnhancedIcbcClient CreateMockEnhancedClientLegacy(IConfiguration configuration)
        {
            return new EnhancedIcbcClient(configuration);
        }
    }
}
