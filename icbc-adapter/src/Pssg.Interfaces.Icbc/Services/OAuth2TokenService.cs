using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly SemaphoreSlim _tokenRefreshLock = new(1, 1);


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
            if (IsCachedTokenValid())
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
            await _tokenRefreshLock.WaitAsync();
            try
            {
                if (IsCachedTokenValid())
                {
                    return _cachedToken;
                }

            var tokenEndpoint = _configuration["ICBC_OAUTH2_TOKEN_ENDPOINT"];
            var clientId = _configuration["ICBC_OAUTH2_CLIENT_ID"];
            var clientSecret = _configuration["ICBC_OAUTH2_CLIENT_SECRET"];

            if (string.IsNullOrEmpty(tokenEndpoint) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("OAuth2 configuration is incomplete. Please ensure ICBC_OAUTH2_TOKEN_ENDPOINT, ICBC_OAUTH2_CLIENT_ID, and ICBC_OAUTH2_CLIENT_SECRET are configured.");
            }

            var tokenRequest = new List<KeyValuePair<string, string>>
            {

                new("grant_type", "client_credentials"),
                new("client_id", clientId),
                new("client_secret", clientSecret),
                new("scope", "app") 
            };

            using var requestContent = new FormUrlEncodedContent(tokenRequest);

            try
            {
                var response = await _httpClient.PostAsync(tokenEndpoint, requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Token endpoint returned {StatusCode}: {Content}", response.StatusCode, responseContent);
                    throw new Exception($"Token endpoint error: {response.StatusCode} - {responseContent}");
                }

                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                {
                    throw new Exception("Token endpoint returned an invalid token response.");
                }

                var expiresInSeconds = tokenResponse.ExpiresIn > 0 ? tokenResponse.ExpiresIn : 300;
                _cachedToken = tokenResponse.AccessToken;
                _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresInSeconds);

                _logger.LogInformation("Successfully acquired OAuth2 token");
                return _cachedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to acquire OAuth2 token");
                throw;
            }
            }
            finally
            {
                _tokenRefreshLock.Release();
            }
        }

        private bool IsCachedTokenValid()
        {
            if (string.IsNullOrEmpty(_cachedToken))
            {
                return false;
            }

            return DateTime.UtcNow < _tokenExpiry.AddSeconds(-30);
        }

        /// <summary>
        /// Token response model for OAuth2
        /// </summary>
        private class TokenResponse
        {

            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }

            [JsonPropertyName("scope")]
            public string Scope { get; set; }
        }
    }
}