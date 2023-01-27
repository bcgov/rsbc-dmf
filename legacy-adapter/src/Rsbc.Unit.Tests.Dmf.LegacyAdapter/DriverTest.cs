

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
using System.Web;
using System.Net;
using Rsbc.Dmf.LegacyAdapter.ViewModels;

namespace Rsbc.Unit.Tests.Dmf.LegacyAdapter
{
    /*
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<Startup>
    {
        public IConfiguration Configuration;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .AddEnvironmentVariables()
                .Build();

            builder
                .UseSolutionRelativeContentRoot("")
                .UseEnvironment("Staging")
                .UseConfiguration(Configuration)
                .UseStartup<Startup>();
        }
    
    */

    [Collection(nameof(HttpClientCollection))]

    public class DriverTest : ApiIntegrationTestBase
    {

        public DriverTest(HttpClientFixture fixture)
            : base(fixture)
        {
            
        }



        


        


            [Fact]
        public async void DfwebSubmitComment()
        {
            Login();

            // start by getting comments.  This will allow us to get the CaseId

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Cases");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            List<Rsbc.Dmf.LegacyAdapter.ViewModels.Case> cases = JsonConvert.DeserializeObject< List <Rsbc.Dmf.LegacyAdapter.ViewModels.Case>> (responseContent);

            string caseId = string.Empty;

            if (cases.Count > 0)
            {
                caseId = cases[0].CaseId;
            }            

            request = new HttpRequestMessage(HttpMethod.Post, $"/Drivers/{testDl}/Comments");

            var driver = new Rsbc.Dmf.LegacyAdapter.ViewModels.Driver() 
            { LicenseNumber = testDl, LastName = testSurcode};

            var comment = new Rsbc.Dmf.LegacyAdapter.ViewModels.Comment() 
            {  
                CommentText = "This is a test comment",
                Driver = driver,
                SequenceNumber = 0, //= 4,
                CommentTypeCode = "W",
                UserId = "IDIR\\TESTUSER",
                CaseId = caseId
            };

            var stringContent = JsonConvert.SerializeObject(comment);
            
            request.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");

            response = _client.SendAsync(request).GetAwaiter().GetResult();

            responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            // now check for the comment to be in the response.

            request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Comments");

            response = _client.SendAsync(request).GetAwaiter().GetResult();

            responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            List<Rsbc.Dmf.LegacyAdapter.ViewModels.Comment> comments = JsonConvert.DeserializeObject<List<Rsbc.Dmf.LegacyAdapter.ViewModels.Comment>>(responseContent);

            if (!string.IsNullOrEmpty(Configuration["CMS_ADAPTER_URI"]))
            {
                bool found = false;

                foreach (var item in comments)
                {
                    if (item.CommentText == comment.CommentText)
                    {
                        found = true;
                    }
                }

                Assert.True(found);
            }

            
        }

       // {"userId":"IDIR\\SMILLAR","driver":\{"licenseNumber":"0200103","lastName":"KNI","loadedFromICBC":false,"flag51":false} 
// ,"sequenceNumber":4,"commentTypeCode":"W","commentText":"test new one"}


    private void SubmitCommentNoCase(string commentText, string commentTypeCode)
    {

            var request = new HttpRequestMessage(HttpMethod.Post, $"/Drivers/{testDl}/Comments");

            var driver = new Rsbc.Dmf.LegacyAdapter.ViewModels.Driver()
            { LicenseNumber = testDl, LastName = testSurcode };

            var comment = new Rsbc.Dmf.LegacyAdapter.ViewModels.Comment()
            {
                CommentText = commentText,
                Driver = driver,
                SequenceNumber = 4,
                CommentTypeCode = commentTypeCode,
                UserId = "IDIR\\TESTUSER",
                CaseId = null
            };

            var stringContent = JsonConvert.SerializeObject(comment);

            request.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();


        }

    [Fact]
    public async void DfwebSubmitCommentNoCase()
    {
        Login();

        SubmitCommentNoCase("test new one", "W");

    }

    [Fact]
        public async void DfwebGetComments()
        {
            Login();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Comments");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }
        /*
        [Fact]
        public async void DfwebTestCommentType()
        {
            Login();

            string commentText = DateTime.Now.ToString() + " COMMENT TEST";

            // W - Web Comments; D - Decision Notes; I - ICBC Comments; C - File Comments; N - Sticky Notes;

            var commentTypes = new string[] { "W","D","I","C","N" };

            foreach (var commentType in commentTypes)
            {
                SubmitCommentNoCase(commentText, commentType);
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Comments");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            List<Rsbc.Dmf.LegacyAdapter.ViewModels.Comment> commentResults = JsonConvert.DeserializeObject<List<Rsbc.Dmf.LegacyAdapter.ViewModels.Comment>>(responseContent);

            List<string> foundTypes = new List<string>();

            foreach (var item in commentResults)
            {
                if (item.CommentText == commentText)
                {
                    if (!foundTypes.Contains(item.CommentTypeCode))
                    {
                        foundTypes.Add(item.CommentTypeCode);
                    }
                }
            }
            Assert.Equal (foundTypes.Count, commentTypes.Length);

        }
        */


        [Fact]
        public async void DfwebGetCommentsFilter()
        {
            Login();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Comments?filter=1");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void DfwebGetCommentsSortC()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Comments?sort=C");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void DfwebGetCommentsSortD()
        {
            Login();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Comments?sort=D");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void DfwebGetCommentsSortT()
        {
            Login();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Comments?sort=T");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void DfwebGetCommentsSortU()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Comments?sort=U");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }

        
        [Fact]
        public async void DfcmsAddDocument()
        {
            Login();

            var caseId = GetCaseIdByDl();

            var request = new HttpRequestMessage(HttpMethod.Post, $"/Drivers/{testDl}/Documents");

            var driver = new Rsbc.Dmf.LegacyAdapter.ViewModels.Driver() { LicenseNumber = testDl, LastName = testSurcode };

            string testString = "Test1234";
            // Convert a C# string to a byte array    
            byte[] bytes = Encoding.ASCII.GetBytes(testString);

            var comment = new Rsbc.Dmf.LegacyAdapter.ViewModels.Document()
            {
                FaxReceivedDate = DateTimeOffset.Now,
                ImportDate = DateTimeOffset.Now,
                DocumentId = Guid.NewGuid().ToString(),
                FileContents = bytes,
                Driver = driver,
                SequenceNumber = 3,                
                UserId = "IDIR\\TESTUSER",
                CaseId = caseId,
                DocumentType = "CSFI15 – CS Vision RDR ",
                DocumentTypeCode = "CSFI15",
                BusinessArea = "Driver Fitness"

            };

            var stringContent = JsonConvert.SerializeObject(comment);

            request.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }
        
        [Fact]
        public async void DfcmsAddRemoveDocument()
        {
            DfcmsAddDocument();

            // now check for the comment to be in the response.

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Documents");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            List<Rsbc.Dmf.LegacyAdapter.ViewModels.Document> documents = JsonConvert.DeserializeObject<List<Rsbc.Dmf.LegacyAdapter.ViewModels.Document>>(responseContent);

            foreach (var document in documents)
            {
                request = new HttpRequestMessage(HttpMethod.Post, $"/Documents/Delete/{document.DocumentId}");

                response = _client.SendAsync(request).GetAwaiter().GetResult();

                responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }

            // confirm that the files are gone.

            request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Documents");

            response = _client.SendAsync(request).GetAwaiter().GetResult();

            responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            documents = JsonConvert.DeserializeObject<List<Rsbc.Dmf.LegacyAdapter.ViewModels.Document>>(responseContent);

            Assert.Equal(documents.Count,1);

        }

        

        [Fact]
        public async void DfcmsGetDocuments()
        {
            Login();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Documents");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }
       

        [Fact]
        public async void TestLoginRequired()
        {            
            if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
            {
                if (_client.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _client.DefaultRequestHeaders.Remove("Authorization");
                }
                var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Cases");

                var response = _client.SendAsync(request).GetAwaiter().GetResult();

                // should be 401 if there was no login.
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async void DfwebGetCaseComments()
        {
            Login();

            // start by getting comments.  This will allow us to get the CaseId

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Cases");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            List<Rsbc.Dmf.LegacyAdapter.ViewModels.Case> cases = JsonConvert.DeserializeObject<List<Rsbc.Dmf.LegacyAdapter.ViewModels.Case>>(responseContent);

            string caseId = cases[0].CaseId;

            request = new HttpRequestMessage(HttpMethod.Get, $"/Cases/{caseId}/Comments");

            response = _client.SendAsync(request).GetAwaiter().GetResult();

            responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }

    }

}
