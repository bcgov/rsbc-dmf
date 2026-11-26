using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public class EntraIdSecurityTokenProvider
    {
        private const string CacheKey = "entraid_token";

        private readonly IMemoryCache cache;
        private readonly DynamicsOptions options;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<EntraIdSecurityTokenProvider> logger;

        public EntraIdSecurityTokenProvider(
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory,
            IOptions<DynamicsOptions> options,
            ILogger<EntraIdSecurityTokenProvider> logger
        )
        {
            this.cache = cache;
            this.httpClientFactory = httpClientFactory;
            this.options = options.Value;
            this.logger = logger;
        }

        public async Task<string> AcquireToken()
        {
            if (!cache.TryGetValue(CacheKey, out string result))
            {
                result = await AcquireTokenInternal();
                cache.Set(CacheKey, result, TimeSpan.FromMinutes(5));
            }

            return result;
        }

        private async Task<string> AcquireTokenInternal()
        {
            var tokenEndpoint = $"https://login.microsoftonline.com/{options.EntraId.TenantId}/oauth2/v2.0/token";

            using var httpClient = httpClientFactory.CreateClient("entraid_token");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", options.EntraId.ClientId),
                new KeyValuePair<string, string>("client_secret", options.EntraId.ClientSecret),
                new KeyValuePair<string, string>("scope", $"{options.EntraId.ResourceName}/.default"),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
            };

            try
            {
                using var content = new FormUrlEncodedContent(pairs);
                using var response = await httpClient.PostAsync(tokenEndpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

                if (result != null && result.ContainsKey("access_token"))
                {
                    logger.LogDebug("Entra ID token acquired");
                    return result["access_token"].GetString();
                }

                if (result != null && result.ContainsKey("error"))
                {
                    var errorDescription = result.ContainsKey("error_description") ? result["error_description"].GetString() : string.Empty;
                    throw new Exception($"{result["error"].GetString()}: {errorDescription}");
                }

                throw new Exception(responseContent);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to obtain access token from Entra ID endpoint: {e.Message}", e);
            }
        }
    }
}
