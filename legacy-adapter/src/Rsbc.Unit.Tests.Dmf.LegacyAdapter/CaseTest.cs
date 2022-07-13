

using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.LegacyAdapter;
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
using System.Net.Http.Headers;

namespace Rsbc.Unit.Tests.Dmf.LegacyAdapter
{
    [Collection(nameof(HttpClientCollection))]
    public class CaseTest : ApiIntegrationTestBase
    {
        
        public CaseTest(HttpClientFixture fixture)
            : base(fixture)
        {
          
        }


        /// <summary>
        /// Test the case exist service - parameters are license number and surcode.
        /// </summary>
        [Fact]
        public async void DoesCaseExist()
        {            
            Login();

            var caseId = GetCaseId();
            Assert.True(caseId != null);
        }

        private string GetCaseId()
        {
            string result = null;
            if (!string.IsNullOrEmpty(testDl))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/Cases/Exist?licenseNumber=" + testDl + "&surcode=" + testSurcode);

                var response = _client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                result = JsonConvert.DeserializeObject<string>(responseContent);
            }
            

            return result;
        }

        [Fact]
        public async void AddCaseDocument()
        {
            Login();

            // start by getting the case.
            var caseId = GetCaseId();
            Assert.True(caseId != null);

            string testData = "This is just a test.";
            string fileName = "fax.pdf";
            byte[] bytes = Encoding.ASCII.GetBytes(testData);

            var request = new HttpRequestMessage(HttpMethod.Post, $"/Cases/{caseId}/Documents");

            MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----TestBoundary");
            var fileContent = new MultipartContent { new ByteArrayContent(bytes) };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "File",
                FileName = fileName
            };
            multiPartContent.Add(fileContent);
            // add the various string parameters.
            string driversLicense = testDl;
            string surcode = testSurcode;
            string batchId = "2707532";
            string faxReceivedDate = "2022-05-19T23:11:53Z";
            string importDate = "2022-05-19";
            string importID = "b86a6b22-7e9d-4e99-8347-cc0e63681da0";
            string originatingNumber = "BCGovtFax";
            int documentPages = 1;
            string documentType = "002";
            string validationMethod = "Single User";
            string validationPrevious = "TESTUSER";

            multiPartContent.Add(new StringContent(driversLicense), "driversLicense");
            multiPartContent.Add(new StringContent(surcode), "surcode");
            multiPartContent.Add(new StringContent(batchId), "batchId");
            multiPartContent.Add(new StringContent(faxReceivedDate), "faxReceivedDate");
            multiPartContent.Add(new StringContent(importDate), "importDate");
            multiPartContent.Add(new StringContent(importID), "importID");
            multiPartContent.Add(new StringContent(originatingNumber), "originatingNumber");
            multiPartContent.Add(new StringContent(documentPages.ToString()), "documentPages");
            multiPartContent.Add(new StringContent(documentType), "002");
            multiPartContent.Add(new StringContent(validationMethod), "validationMethod");
            multiPartContent.Add(new StringContent(validationPrevious), "validationPrevious");

            // create a new request object for the upload, as we will be using multipart form submission.
            request.Content = multiPartContent;

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async void GetDocuments()
        {
            Login();

            var caseId = GetCaseId();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Cases/{caseId}/Documents");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }

    }
}
