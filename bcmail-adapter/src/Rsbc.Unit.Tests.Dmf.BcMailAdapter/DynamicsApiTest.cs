using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rsbc.Interfaces;
using Rsbc.Dmf.BcMailAdapter.ViewModels;
using Xunit;

namespace Rsbc.Dmf.BcMailAdapter.Tests
{
    [Collection(nameof(HttpClientCollection))]
    public class DynamicsApiTest : ApiIntegrationTestBase
    {
        protected HttpClient _client { get; }       
        
      
        public DynamicsApiTest(HttpClientFixture fixture)
            : base(fixture)
        {
            _client = fixture.Client;
            
            
        }



        /// <summary>
        /// Test the CDGS Client
        /// </summary>
        [Fact]
        public async void TestDocumentPreview()
        {
            // login to the BC Mail Adapter (this service)
            Login();

            
            var request = new HttpRequestMessage(HttpMethod.Post, $"/Documents/BcMailPreview");


            ViewModels.BcMail bcmail = new ViewModels.BcMail()
            {
                  Attachments = new List<Attachment> (),
                  isPreview = true
            };


            var stringContent = JsonConvert.SerializeObject(bcmail);


            request.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
            

        }

    }
}
