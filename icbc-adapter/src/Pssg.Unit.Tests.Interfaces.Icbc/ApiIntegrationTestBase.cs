

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
            // determine if authentication is enabled.

            if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
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
