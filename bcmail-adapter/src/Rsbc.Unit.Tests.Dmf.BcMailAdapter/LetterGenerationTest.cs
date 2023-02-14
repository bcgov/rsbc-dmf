using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rsbc.Dmf.BcMailAdapter.ViewModels;
using Rsbc.Interfaces;
using Xunit;

namespace Rsbc.Unit.Tests.Dmf.BcMailAdapter
{
    public class LetterGenerationTest
    {
        protected HttpClient _Client { get; }
        protected readonly IConfiguration Configuration;
        protected string CdgsServiceUri;
      
        public LetterGenerationTest(HttpClientFixture fixture)
        {
            _Client = fixture.Client;
            Configuration = fixture.Configuration;
            CdgsServiceUri = Configuration["CDGS_SERVICE_URI"] ?? "https://cdogs-dev.api.gov.bc.ca/api/v2/";
            _Client.BaseAddress = new Uri(CdgsServiceUri);
        }



        /// <summary>
        /// BcMail Document Preview Test
        /// </summary>
        [Fact]
        public async void BcMailDocumentPreviewTest()
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
            var response = _Client.SendAsync(request).GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            BcMail bcMail = JsonConvert.DeserializeObject<BcMail>(jsonString);

            Assert.Equal(Convert.FromBase64String(bcMail.Attachments[0].Body), Encoding.ASCII.GetBytes(testString));
        }

        }
    }
