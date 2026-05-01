using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Pssg.Interfaces.Icbc.Services
{
    /// <summary>
    /// OAuth2 token service for ICBC API authentication
    /// </summary>
    public class OAuth2TokenService : IOAuth2TokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OAuth2TokenService> _logger;
        private string _cachedToken;
        private DateTime _tokenExpiry;


        public OAuth2TokenService(HttpClient httpClient, IConfiguration configuration, ILogger<OAuth2TokenService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }


        /// <summary>
        /// Gets a valid access token, using cache if available and not expired
        /// </summary>
        /// <returns>Valid access token</returns>
        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
            {
                return _cachedToken;
            }

            // Need to acquire a new token
            return await RefreshTokenAsync();
        }

        /// <summary>
        /// Forces acquisition of a new token
        /// </summary>
        /// <returns>New access token</returns>
        public async Task<string> RefreshTokenAsync()
        {
            var tokenEndpoint = _configuration["ICBC_OAUTH2_TOKEN_ENDPOINT"];
            // https://wsgw.dev.jag.gov.bc.ca/icbc-npe-dev-oauth2-token*
            var clientId = _configuration["ICBC_OAUTH2_CLIENT_ID"];
            var clientSecret = _configuration["ICBC_OAUTH2_CLIENT_SECRET"];
            var scope = _configuration["ICBC_OAUTH2_SCOPE"] ?? "icbc_api";

            if (string.IsNullOrEmpty(tokenEndpoint) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("OAuth2 configuration is incomplete. Please ensure ICBC_OAUTH2_TOKEN_ENDPOINT, ICBC_OAUTH2_CLIENT_ID, and ICBC_OAUTH2_CLIENT_SECRET are configured.");
            }

            var tokenRequest = new List<KeyValuePair<string, string>>
            {

                new("grant_type", "client_credentials"),
                new("client_id", clientId),
                new("client_secret", clientSecret),
                new("scope", "icbc_api") // Adjust scope as needed
            };

            var requestContent = new FormUrlEncodedContent(tokenRequest);

            try
            {
                var response = await _httpClient.PostAsync(tokenEndpoint, requestContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                _cachedToken = tokenResponse.AccessToken;
                _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

                _logger.LogInformation("Successfully acquired OAuth2 token");
                return _cachedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to acquire OAuth2 token");
                throw;
            }
        }

        /// <summary>
        /// Token response model for OAuth2
        /// </summary>
        private class TokenResponse
        {
            public string AccessToken { get; set; }
            public int ExpiresIn { get; set; }
            public string TokenType { get; set; }
            public string Scope { get; set; }
        }
    }
}