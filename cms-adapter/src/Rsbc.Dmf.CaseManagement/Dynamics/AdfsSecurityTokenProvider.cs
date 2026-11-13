using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public class AdfsSecurityTokenProvider
    {
        private const string CacheKey = "adfs_token";

        private readonly IMemoryCache cache;
        private readonly DynamicsOptions options;
        private readonly IHttpClientFactory httpClientFactory;

        public AdfsSecurityTokenProvider(IMemoryCache cache, IHttpClientFactory httpClientFactory, IOptions<DynamicsOptions> options)
        {
            this.cache = cache;
            this.httpClientFactory = httpClientFactory;
            this.options = options.Value;
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
            using var httpClient = httpClientFactory.CreateClient("adfs_token");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("resource", options.Adfs.ResourceName),
                new KeyValuePair<string, string>("client_id", options.Adfs.ClientId),
                new KeyValuePair<string, string>("client_secret", options.Adfs.ClientSecret),
                new KeyValuePair<string, string>("username", $"{options.Adfs.ServiceAccountDomain}\\{options.Adfs.ServiceAccountName}"),
                new KeyValuePair<string, string>("password", options.Adfs.ServiceAccountPassword),
                new KeyValuePair<string, string>("scope", "openid"),
                new KeyValuePair<string, string>("response_mode", "form_post"),
                new KeyValuePair<string, string>("grant_type", "password"),
            };

            try
            {
                using var content = new FormUrlEncodedContent(pairs);
                using var response = await httpClient.PostAsync(string.Empty, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

                if (result != null && result.ContainsKey("access_token"))
                {
                    return result["access_token"].GetString();
                }

                if (result != null && result.ContainsKey("error"))
                {
                    throw new Exception($"{result["error"].GetString()}: {result["error_description"].GetString()}");
                }

                throw new Exception(responseContent);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to obtain access token from OAuth2TokenEndpoint: {e.Message}", e);
            }
        }
    }
}
