using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

            //Assert.False(string.IsNullOrEmpty(result));
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

        //   {"driversLicense":"01000115","batchId":"1007172","faxReceivedDate":"2023-10-06T21:14:28.0326378+00:00","importDate":"2023-10-06T21:14:28.0326451+00:00",
        //   "importID":"220a1883-c173-408c-812b-9c65769fb0ed","originatingNumber":null,"documentPages":2,"documentType":"(PDR)PriorityDoctorsReport","documentTypeCode":"160","validationMethod":"DPS Migration","validationPrevious":"System","priority":"Critical Review","assign":"Adjudicators","submittalStatus":"Uploaded","surcode":"PEL"}


        [Fact]
        public async Task AddCaseDocumentWithPerson()
        {
            Login();
            
            // start by getting the case.
            var caseId = GetCaseIdByDl();

            caseId = Guid.Empty.ToString();

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
            string driversLicense = "01000115";
            string surcode = "PEL";
            string batchId = "1007172";
            string faxReceivedDate = "2023-10-05T23:11:53Z";
            string importDate = "2022-05-19";
            string importID = "220a1883-c173-408c-812b-9c65769fb0ed";
            string originatingNumber = "";
            int documentPages = 2;
            string documentType = "(PDR)PriorityDoctorsReport";
            string documentTypeCode = "160";
            string validationMethod = "DPS Migration";
            string validationPrevious = "System";
            string priority = "Critical Review";
            string assign = "Adjudicators";
            string submittalStatus = "Uploaded";

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

            // TODO - add mock so that the response will be OK in a CI build
            
        }

        [Fact]
        public async Task AddCaseDocument()
        {
            Login();
            if (!string.IsNullOrEmpty(testDl))
            {
                // start by getting the case.
                var caseId = GetCaseIdByDl();

                caseId = Guid.Empty.ToString();

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
                string importDate = "2022-05-29";
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
                multiPartContent.Add(new StringContent(faxReceivedDate), "faxReceivedDateString");
                multiPartContent.Add(new StringContent(importDate), "importDateString");
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
        public void AddUnsolicitedCaseDocument()
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
                string documentType = "Police Report";
                string documentTypeCode = "150";
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

                // TODO - add mock 
                //response.EnsureSuccessStatusCode();
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
        public async void AddCaseDocumentNoSurcode()
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
                string surcode = ""; // intentionally blank to simulate DPS sending BC Mail
                string batchId = "BCMD";
                string faxReceivedDate = "2023-10-11T21:23:15.6925395+00:00";
                string importDate = "2023-10-11T21:23:15.6925413+00:00";
                string importID = "";
                string originatingNumber = "";
                int documentPages = 1;
                string documentType = "";
                string validationMethod = "";
                string validationPrevious = "";

                multiPartContent.Add(new StringContent(driversLicense), "driversLicense");
                multiPartContent.Add(new StringContent(surcode), "surcode");
                multiPartContent.Add(new StringContent(batchId), "batchId");
                multiPartContent.Add(new StringContent(faxReceivedDate), "faxReceivedDate");
                multiPartContent.Add(new StringContent(importDate), "importDate");
                multiPartContent.Add(new StringContent(importID), "importID");
                multiPartContent.Add(new StringContent(originatingNumber), "originatingNumber");
                multiPartContent.Add(new StringContent(documentPages.ToString()), "documentPages");
                multiPartContent.Add(new StringContent(documentType), "documentType");
                multiPartContent.Add(new StringContent("181"), "documentTypeCode");
                multiPartContent.Add(new StringContent(validationMethod), "validationMethod");
                multiPartContent.Add(new StringContent(validationPrevious), "validationPrevious");
                multiPartContent.Add(new StringContent(""), "assign");
                multiPartContent.Add(new StringContent(""), "priority");
                multiPartContent.Add(new StringContent(""), "submittalStatus");

                // create a new request object for the upload, as we will be using multipart form submission.
                request.Content = multiPartContent;

                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                
                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task AddUnclassifiedCaseDocument()
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
                string documentType = "Unclassified";
                string documentTypeCode = "999";
                string validationMethod = "Single User";
                string validationPrevious = "BHAMMED";
                string priority = "Expedited";
                string assign = "Adjudicators";
                string submittalStatus = "Uploaded";
                string queue = "Team - Intake";

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
                multiPartContent.Add(new StringContent(queue), "queue");


                // create a new request object for the upload, as we will be using multipart form submission.
                request.Content = multiPartContent;

                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task AddBatchCaseDocument()
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

               // var request = new HttpRequestMessage(HttpMethod.Post, $"/Cases/{caseId}/Documents");

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
                string documentType = "Unclassified";
                string documentTypeCode = "999";
                string validationMethod = "Single User";
                string validationPrevious = "BHAMMED";
                string priority = "Expedited";
                string assign = "Adjudicators";
                string submittalStatus = "Uploaded";
                string queue = "Team - Intake";

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
                multiPartContent.Add(new StringContent(queue), "queue");


                // create a new request object for the upload, as we will be using multipart form submission.
              

                foreach (int i in Enumerable.Range(0, 2))
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, $"/Cases/{caseId}/Documents");

                    request.Content = multiPartContent;

                    var response = _client.SendAsync(request).GetAwaiter().GetResult();

                    var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    response.EnsureSuccessStatusCode();
                }
            }
        }


        [Fact]
        public async Task AddClassifiedCaseDocument()
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
                string validationPrevious = "";
                string priority = "Expedited";
                string assign = "Team - Adjudicators";
                string submittalStatus = "Uploaded";
               

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
                //multiPartContent.Add(new StringContent(queue), "queue");
                // create a new request object for the upload, as we will be using multipart form submission.
                request.Content = multiPartContent;

                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddRemedialRDPCaseDocument()
        {
            string documentType = "RDPRegistration";
            string documentTypeCode = "110";

            TestRemedialDocumentCode(documentType, documentTypeCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddRemedialIgnitionCaseDocument()
        {
            string documentType = "Ignition Interlock Incident";
            string documentTypeCode = "080";

            TestRemedialDocumentCode(documentType, documentTypeCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentType"></param>
        /// <param name="documentTypeCode"></param>
        private void TestRemedialDocumentCode(string documentType, string documentTypeCode)
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
/*                string documentType = documentType1;
                string documentTypeCode = documentTypeCode;*/
                string validationMethod = "Single User";
                string validationPrevious = string.Empty;
                string priority = "Expedited";
                string assign = "Client Services";
                string submittalStatus = "Received";
                string queue = "Team - Intake";

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
                multiPartContent.Add(new StringContent(queue), "queue");
                
                // create a new request object for the upload, as we will be using multipart form submission.
                request.Content = multiPartContent;

                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                Assert.Equal(response.StatusCode, System.Net.HttpStatusCode.Created);
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
                string documentType = "(DMER)(KMC)";
                string documentTypeCode = "001";

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
                multiPartContent.Add(new StringContent(documentTypeCode), "documentTypeCode");
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
