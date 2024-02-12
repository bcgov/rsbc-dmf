using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests.Integration
{
    public class DocumentTests : ApiIntegrationTestBase
    {
        public DocumentTests(IConfiguration configuration) : base(configuration) { }

        [Fact]
        public async Task Download_Document()
        {
            var docId = _configuration["DOWNLOAD_DOC_ID"];
            if (string.IsNullOrEmpty(docId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{DOCUMENT_API_BASE}/{docId}");
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var input = await response.Content.ReadAsStreamAsync();
            var memoryStream = new MemoryStream();
            input.CopyTo(memoryStream);

            Assert.True(memoryStream.Length > 100);
        }

        [Fact]
        public async Task Download_Document_No_Url()
        {
            var docId = _configuration["NO_DOWNLOAD_DOC_ID"];
            if (string.IsNullOrEmpty(docId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{DOCUMENT_API_BASE}/{docId}");
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Download_Document_Malformed_Url()
        {
            var docId = _configuration["DOWNLOAD_DOC_MALFORMED_URL"];
            if (string.IsNullOrEmpty(docId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{DOCUMENT_API_BASE}/{docId}");
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Download_Document_Fake_Id()
        {
            // random guid
            var docId = "111a5c4c-a23e-ed11-b834-005056830000";
            if (string.IsNullOrEmpty(_configuration["ICBC_TEST_DL"]))
                return;

            var request = new HttpRequestMessage(HttpMethod.Get, $"{DOCUMENT_API_BASE}/{docId}");
            var response = await _client.SendAsync(request);

            // TODO refactor GetLegacyDocument to return NotFound
            //Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Upload_Document()
        {
            var caseId = _configuration["DOWNLOAD_DOC_ID"];
            if (string.IsNullOrEmpty(caseId))
                return;

            var request = new HttpRequestMessage(HttpMethod.Post, $"{DOCUMENT_API_BASE}/upload");
            var multiPartContent = new MultipartFormDataContent("----TestBoundary");

            // add png file to request
            var bytes = File.ReadAllBytes("Data/edit.png");
            multiPartContent.Add(new ByteArrayContent(bytes), "file", "test.png");
            request.Content = multiPartContent;

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        }
    }
