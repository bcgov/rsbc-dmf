using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
        public async void DoesCaseExist7Digit()
        {
            Login();
            string sevenDigitTest = "2222222";

            string result = null;
            if (!string.IsNullOrEmpty(testDl))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/Cases/ExistByDl?licenseNumber=" + sevenDigitTest);

                var response = _client.SendAsync(request).GetAwaiter().GetResult();
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
                result = JsonConvert.DeserializeObject<string>(responseContent);
            }

            var caseId = GetCaseId();
            Assert.True(caseId != null);
        }

        /// <summary>
        /// Test the surcode validation with Three characters
        /// </summary>
        [Fact]
        public async void TestThreeDigitSurCodeIntegration()
        {
            Login();
            string licenseNumber = "02100110";
            string surcode = "SMI";

            TestSurCode(licenseNumber, surcode);
        }

        /// <summary>
        /// Test the surcode validation with two characters
        /// </summary>
        [Fact]
        public async void TestTwoDigitSurCodeIntegration()
        {
            Login();
            string licenseNumber = "001000101";
            string surcode = "LO";

            TestSurCode(licenseNumber, surcode);
        }

        /// <summary>
        /// Test the surcode validation with single character
        /// </summary>
        [Fact]
        public async void TestSingleDigitSurCodeIntegration()
        {
            Login();
            string licenseNumber = "01000045";
            string surcode = "U";

            TestSurCode(licenseNumber, surcode);
        }

        /// <summary>
        /// Test the surcode validation with four character
        /// </summary>
        [Fact]
        public async void TestLongSurCodeIntegration()
        {
            Login();
            string licenseNumber = "00200699";
            string surcode = "AMTI";

            TestSurCode(licenseNumber, surcode);
        }

        /// <summary>
        /// Test Surcode validation
        /// </summary>
        /// <param name="licenceNumber"></param>
        /// <param name="surcode"></param>
        private void TestSurCode (string licenseNumber, string surcode)
        {
            Login();
            
            string result = null;
            if (!string.IsNullOrEmpty(testDl))
            {

                var request = new HttpRequestMessage(HttpMethod.Get, $"/Cases/Exist?licenseNumber={licenseNumber}&surcode={surcode}");

                var response = _client.SendAsync(request).GetAwaiter().GetResult();
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
                result = JsonConvert.DeserializeObject<string>(responseContent);
            }

            Assert.False(string.IsNullOrEmpty(result));
        }


        /// <summary>
        /// Does Case Exsist
        /// </summary>
        [Fact]
        public async void DoesCaseExist()
        {
            Login();

            var caseId = GetCaseId();
            Assert.True(caseId != null);
        }

        [Fact]
        public async Task AddCaseDocument()
        {
            Login();
            if (!string.IsNullOrEmpty(testDl))
            {
                // start by getting the case.
                var caseId = GetCaseIdByDl();
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
                string batchId = "1001111";
                string faxReceivedDate = "2022-05-19T23:11:53Z";
                string importDate = "2022-05-19";
                string importID = "b86a6b22-7e9d-4e99-8347-cc0e63681da0";
                string originatingNumber = "";
                int documentPages = 1;
                string documentType = "DMER";
                string documentTypeCode = "001";
                string validationMethod = "Single User";
                string validationPrevious = "BHAMMED";
                string priority = "Expedited";
                string assign = "Adjudicators";
                string submittalStatus = "Manual Pass";

                multiPartContent.Add(new StringContent(driversLicense), "driversLicense");
                multiPartContent.Add(new StringContent(surcode), "surcode");
                multiPartContent.Add(new StringContent(batchId), "batchId");
                multiPartContent.Add(new StringContent(faxReceivedDate), "faxReceivedDate");
                multiPartContent.Add(new StringContent(importDate), "importDate");
                multiPartContent.Add(new StringContent(importID), "importID");

                multiPartContent.Add(new StringContent(originatingNumber), "originatingNumber");
                multiPartContent.Add(new StringContent(documentPages.ToString()), "documentPages");
                multiPartContent.Add(new StringContent(documentType), "documentType");
                multiPartContent.Add(new StringContent(documentTypeCode), "documentTypeCode");
                multiPartContent.Add(new StringContent(validationMethod), "validationMethod");
                multiPartContent.Add(new StringContent(validationPrevious), "validationPrevious");

                multiPartContent.Add(new StringContent(priority), "priority");
                multiPartContent.Add(new StringContent(assign), "assign");
                multiPartContent.Add(new StringContent(submittalStatus), "submittalStatus");

                // create a new request object for the upload, as we will be using multipart form submission.
                request.Content = multiPartContent;

                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }
        }


        [Fact]
        public async Task AddUnsolicitatedCaseDocument()
        {
            Login();
            if (!string.IsNullOrEmpty(testDl))
            {
                // start by getting the case.
                var caseId = GetCaseIdByDl();
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
                string batchId = "1001111";
                string faxReceivedDate = "2022-05-19T23:11:53Z";
                string importDate = "2022-05-19";
                string importID = "b86a6b22-7e9d-4e99-8347-cc0e63681da0";
                string originatingNumber = "";
                int documentPages = 1;
                string documentType = "Unsolicited Report of Concern";
                string documentTypeCode = "190";
                string validationMethod = "Single User";
                string validationPrevious = "BHAMMED";
                string priority = "Expedited";
                string assign = "Adjudicators";
                string submittalStatus = "Accept";

                multiPartContent.Add(new StringContent(driversLicense), "driversLicense");
                multiPartContent.Add(new StringContent(surcode), "surcode");
                multiPartContent.Add(new StringContent(batchId), "batchId");
                multiPartContent.Add(new StringContent(faxReceivedDate), "faxReceivedDate");
                multiPartContent.Add(new StringContent(importDate), "importDate");
                multiPartContent.Add(new StringContent(importID), "importID");

                multiPartContent.Add(new StringContent(originatingNumber), "originatingNumber");
                multiPartContent.Add(new StringContent(documentPages.ToString()), "documentPages");
                multiPartContent.Add(new StringContent(documentType), "documentType");
                multiPartContent.Add(new StringContent(documentTypeCode), "documentTypeCode");
                multiPartContent.Add(new StringContent(validationMethod), "validationMethod");
                multiPartContent.Add(new StringContent(validationPrevious), "validationPrevious");

                multiPartContent.Add(new StringContent(priority), "priority");
                multiPartContent.Add(new StringContent(assign), "assign");
                multiPartContent.Add(new StringContent(submittalStatus), "submittalStatus");

                // create a new request object for the upload, as we will be using multipart form submission.
                request.Content = multiPartContent;

                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async void AddCaseDocumentNoOriginatingNumber()
        {
            Login();
            if (!string.IsNullOrEmpty(testDl))
            {
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
                string batchId = "591";
                string faxReceivedDate = "2022-05-19T23:11:53Z";
                string importDate = "2022-05-19";
                string importID = "{b86a6b22-7e9d-4e99-8347-cc0e63681da0}";
                string originatingNumber = "";
                int documentPages = 1;
                string documentType = "002";
                documentType = "Clean Pass";
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
        }


        [Fact]
        public async void AddCaseDocumentSparse()
        {
            if (!string.IsNullOrEmpty(testDl))
            {
                Login();

                // start by getting the case.
                var caseId = GetCaseId();
                Assert.True(caseId != null);

                string testData = "";
                string fileName = "";
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
                //string surcode = "";
                string batchId = "";
                string faxReceivedDate = "";
                string importDate = "";
                string importID = "{b86a6b22-7e9d-4e99-8347-cc0e63681da0}";
                string originatingNumber = "";
                int documentPages = 1;
                string documentType = "";

                string validationMethod = "";
                string validationPrevious = "";

                multiPartContent.Add(new StringContent(driversLicense), "driversLicense");
                //multiPartContent.Add(new StringContent(surcode), "surcode");
                multiPartContent.Add(new StringContent(batchId), "batchId");
                multiPartContent.Add(new StringContent(faxReceivedDate), "faxReceivedDate");
                multiPartContent.Add(new StringContent(importDate), "importDate");
                multiPartContent.Add(new StringContent(importID), "importID");
                multiPartContent.Add(new StringContent(originatingNumber), "originatingNumber");
                multiPartContent.Add(new StringContent(documentPages.ToString()), "documentPages");
                multiPartContent.Add(new StringContent(documentType), "documentType");
                multiPartContent.Add(new StringContent(validationMethod), "validationMethod");
                multiPartContent.Add(new StringContent(validationPrevious), "validationPrevious");

                // create a new request object for the upload, as we will be using multipart form submission.
                request.Content = multiPartContent;

                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }
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

        [Fact]
        public async void GetDocument()
        {
            Login();

            var caseId = GetCaseId();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Cases/{caseId}/Documents");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            var documents = JsonConvert.DeserializeObject<List<Rsbc.Dmf.LegacyAdapter.ViewModels.Document>>(responseContent);

            foreach (var document in documents)
            {

                request = new HttpRequestMessage(HttpMethod.Get, $"/Documents/{document.DocumentId}");

                response = _client.SendAsync(request).GetAwaiter().GetResult();

                responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }
        }

    }
}
