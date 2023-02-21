using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rsbc.Dmf.BcMailAdapter.ViewModels;
using Rsbc.Interfaces;
using Xunit;

namespace Rsbc.Dmf.BcMailAdapter.Tests
{
    [Collection(nameof(HttpClientCollection))]
    public class CdgsClientTest : ApiIntegrationTestBase
    {
        protected HttpClient _client { get; }       
        protected string _cdgsServiceUri;
      
        public CdgsClientTest(HttpClientFixture fixture)
            : base(fixture)
        {
            _client = fixture.Client;
            _cdgsServiceUri = Configuration["CDGS_SERVICE_URI"] ?? string.Empty;

            if (!string.IsNullOrEmpty(_cdgsServiceUri))
            {
                _client.BaseAddress = new Uri(_cdgsServiceUri);
            }            
        }

        /// <summary>
        /// Test the CDGS Client
        /// </summary>
        [Fact]
        public async void TestCdgsClient()
        {
          
            if (!string.IsNullOrEmpty(_cdgsServiceUri))
            {
                string testString = "test";
                var request = new HttpRequestMessage(HttpMethod.Post, "template/render");

                var result = new PdfResponse()
                {
                    FileName = "Test BC mail Preview",
                    ContentType = "text/plain",
                    Body = Convert.ToBase64String(Encoding.ASCII.GetBytes(testString)),
                };

                string jsonString = JsonConvert.SerializeObject(result);
                request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                // parse as JSON.
                jsonString = await response.Content.ReadAsStringAsync();
                BcMail bcMail = JsonConvert.DeserializeObject<BcMail>(jsonString);

                Assert.Equal(Convert.FromBase64String(bcMail.Attachments[0].Body), Encoding.ASCII.GetBytes(testString));
            }
            
        }

        /// <summary>
        /// Test the CDGS Client
        /// </summary>
        [Fact]
        public async void TestDocxCdgsClient()
        {

            if (!string.IsNullOrEmpty(_cdgsServiceUri))
            {

                // construct docx using marigold library convert that to docx and pass it to cdogs
                // var docx = this.CreateDocument(attachment.Body, attachment.Header, attachment.Footer);

                string testString = "test";
                var request = new HttpRequestMessage(HttpMethod.Post, "template/render");

                var result = new PdfResponse()
                {
                    FileName = "Test BC mail Preview",
                    ContentType = "text/plain",
                    // 
                    Body = Convert.ToBase64String(Encoding.ASCII.GetBytes(testString)),
                };

                string jsonString = JsonConvert.SerializeObject(result);
                request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                // parse as JSON.
                jsonString = await response.Content.ReadAsStringAsync();
                BcMail bcMail = JsonConvert.DeserializeObject<BcMail>(jsonString);

                Assert.Equal(Convert.FromBase64String(bcMail.Attachments[0].Body), Encoding.ASCII.GetBytes(testString));
            }

        }
    }
}
