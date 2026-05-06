using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.IcbcAdapter;
using Pssg.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.ViewModels;
using System.Web;
using Pssg.Interfaces.Icbc.Services;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    public abstract class ApiIntegrationTestBase
    {

        protected HttpClient _client { get; }

        protected readonly IConfiguration Configuration;

        public ApiIntegrationTestBase(HttpClientFixture fixture)
        {
            _client = fixture.Client;
            Configuration = fixture.Configuration;
        }


        protected void Login()
        {
            // Check if OAuth2 is enabled and configured
            bool useOAuth2 = Configuration.GetValue<bool>("ICBC_USE_OAUTH2", true);

            if (useOAuth2 && !string.IsNullOrEmpty(Configuration["ICBC_OAUTH2_TOKEN_ENDPOINT"]))
            {
                // Use OAuth2 authentication for testing
                LoginWithOAuth2();
            }
            else if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
            {
                // Fallback to JWT token authentication if OAuth2 is not available
                LoginWithJWT();
            }
            // If no authentication is configured, do nothing (anonymous access)
        }

        private void LoginWithOAuth2()
        {
            try
            {
                // Get OAuth2 configuration
                var tokenEndpoint = Configuration["ICBC_OAUTH2_TOKEN_ENDPOINT"];
                var clientId = Configuration["ICBC_OAUTH2_CLIENT_ID"];
                var clientSecret = Configuration["ICBC_OAUTH2_CLIENT_SECRET"];
                var scope = Configuration["ICBC_OAUTH2_SCOPE"] ?? "icbc_api";

                if (string.IsNullOrEmpty(tokenEndpoint) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    // If OAuth2 config is incomplete, fall back to JWT or anonymous
                    if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
                    {
                        LoginWithJWT();
                    }
                    return;
                }

                // Prepare OAuth2 token request
                var tokenRequest = new List<KeyValuePair<string, string>>
                {
                    new("grant_type", "client_credentials"),
                    new("client_id", clientId),
                    new("client_secret", clientSecret),
                    new("scope", scope)
                };

                var requestContent = new FormUrlEncodedContent(tokenRequest);

                // Create separate HttpClient for token request to avoid circular dependency
                using (var tokenClient = new HttpClient())
                {
                    var response = tokenClient.PostAsync(tokenEndpoint, requestContent).GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var tokenResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                        string accessToken = tokenResponse.access_token;

                        if (!string.IsNullOrEmpty(accessToken) && !_client.DefaultRequestHeaders.Contains("Authorization"))
                        {
                            // Add the bearer token to the client
                            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                        }
                    }
                    else
                    {
                        // OAuth2 failed, try JWT as fallback
                        if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
                        {
                            LoginWithJWT();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // OAuth2 failed, try JWT as fallback
                if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
                {
                    LoginWithJWT();
                }
            }
        }

        private void LoginWithJWT()
        {
            // Original JWT authentication logic
            if (!_client.DefaultRequestHeaders.Contains("Authorization"))
            {
                string encodedSecret = HttpUtility.UrlEncode(Configuration["JWT_TOKEN_KEY"]);
                var request = new HttpRequestMessage(HttpMethod.Get, "/Authentication/Token?secret=" + encodedSecret);
                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var token = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!string.IsNullOrEmpty(token) && token != "Invalid secret.")
                    {
                        // Add the bearer token to the client
                        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    }
                }
            }
        }
    }

}
