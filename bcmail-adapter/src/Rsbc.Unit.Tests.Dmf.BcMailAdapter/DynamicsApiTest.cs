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

        [Fact]
        public void TestUrlSplit()
        {
            string serverRelativeUrl = "dfp_driver/1234/test.txt";
            string originalEntity = serverRelativeUrl.Substring(0, serverRelativeUrl.IndexOf("/"));

            int firstSlashPos = serverRelativeUrl.IndexOf('/') + 1;
            int lastSlashPos = serverRelativeUrl.LastIndexOf('/') + 1;

            string folderName = serverRelativeUrl.Substring(firstSlashPos, lastSlashPos - firstSlashPos - 1);
            string filename = serverRelativeUrl.Substring(lastSlashPos);

            Assert.Equal("dfp_driver",originalEntity);
            Assert.Equal("1234",folderName);
            Assert.Equal("test.txt", filename);
        }


        /// <summary>
        /// Test the CDGS Client
        /// </summary>
       /* [Fact]
        public async void TestDocumentPreview()
        {
            // login to the BC Mail Adapter (this service)
            Login();

            
            var request = new HttpRequestMessage(HttpMethod.Post, $"/Documents/BcMailPreview");


            ViewModels.BcMail bcmail = new ViewModels.BcMail()
            {                   
                  Attachments = new List<Attachment> () { new Attachment() { 
                      ContentType = "html",
                      Body = Encoding.ASCII.GetBytes("BODY"),
                      Header=Encoding.ASCII.GetBytes("HEADER"), 
                      Footer=Encoding.ASCII.GetBytes("FOOTER") 
                     // Css= Encoding.ASCII.GetBytes("color:blue") 
                  }
                  },
                  isPreview = true
            };


            var stringContent = JsonConvert.SerializeObject(bcmail);


            request.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
            

        }*/

    }
}
