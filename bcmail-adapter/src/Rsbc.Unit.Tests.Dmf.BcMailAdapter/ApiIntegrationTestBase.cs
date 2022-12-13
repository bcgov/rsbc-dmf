

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

using System.Web;

namespace Rsbc.Unit.Tests.Dmf.BcMailAdapter
{   
    public abstract class ApiIntegrationTestBase 
    {
  
        protected HttpClient _client { get; }

        protected readonly IConfiguration Configuration;

        protected string testDl;
        protected string testSurcode;

        public ApiIntegrationTestBase(HttpClientFixture fixture)
        {
            _client = fixture.Client;
            Configuration = fixture.Configuration;

            testDl = Configuration["ICBC_TEST_DL"] ?? "2222222";
            testSurcode = Configuration["ICBC_TEST_SURCODE"] ?? "TST";
        }


        protected string GetCaseId()
        {
            string result = null;
            if (!string.IsNullOrEmpty(testDl))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/Cases/Exist?licenseNumber=" + testDl + "&surcode=" + testSurcode);

                var response = _client.SendAsync(request).GetAwaiter().GetResult();
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                

                result = JsonConvert.DeserializeObject<string>(responseContent);
            }

            return result;
        }

        protected string GetCaseIdByDl()
        {
            string result = null;
            if (!string.IsNullOrEmpty(testDl))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/Cases/ExistByDl?licenseNumber=" + testDl);

                var response = _client.SendAsync(request).GetAwaiter().GetResult();
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();



                result = JsonConvert.DeserializeObject<string>(responseContent);
            }

            return result;
        }


        

        protected void Login()
        {
            // determine if authentication is enabled.

            if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]) && !_client.DefaultRequestHeaders.Contains("Authorization"))
            {
                string encodedSecret = HttpUtility.UrlEncode(Configuration["JWT_TOKEN_KEY"]);
                var request = new HttpRequestMessage(HttpMethod.Get, "/Authentication/Token?secret=" + encodedSecret);
                var response = _client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                var token = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(token))
                {
                    // Add the bearer token to the client.
                    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
            }

        }
    }
   
}
