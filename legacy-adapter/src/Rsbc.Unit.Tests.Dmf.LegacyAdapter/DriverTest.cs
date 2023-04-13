

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

            stringContent = "{\"CommentText\":\"DE Name Richard Dale Burnaby\\r\\nApril 12, 2023\\r\\nERA location 94258\\r\\nResult DL/LDL  LDL\\r\\nDEs level of concern Medium/High\\r\\nPre-trip  Was not able to turn on headlights in pretrip. Was able to name 4 of 6 traffic signs shown to him. Unsure of the yield sign and Merge sign. GasBrake exercise was completed without errors.\\r\\nNo dangerous actions occurred.\\r\\nNo violations occurred. \\r\\nOther Mr. Corradi has some shoulder mobility issues making using the turn signals and proper use of steering control extremely difficult.  He would have to reach through steering wheel with right hand in attempt to signal causing lane straddling and when turning would be using steering wheels spokes with fingers not showing adequate control of vehicle.  Because of the lack of control, test was shortened.\\r\\nCognitive Completed Adjust Safety Controls was completed on part one with issue of steering control due to left shoulder. Was unable to complete Multi step 1.  Completed 2 of three turns before needing assistance.  Multi step 2 and reverse route were not attempted as test was cut short for safety reasons. \\r\\nCompensation Missing most right turn shoulder checks.  Steering control and signal timing were issues due to injury.\\r\\nCommunication barrier No communication barriers\\r\\nLevel of Concern Medium High Mr.Corradi arrived on time for his assessment. He reacts moderately to all vehicle and pedestrian traffic. He scans well after stopping but he also has some bad driving habits like missed shoulder checks and rolling stops. He had no Dangerous actions and no Violations but test was cut short due to control limitations.  Mr. Corradi seemed quite upset about having to take the re-exam and was asked a few times if he was ok to proceed when I noticed his lack of mobility in his shoulder. Based on his physical health I believe he is a medium high risk due to lack of control of the vehicle.\\r\\n\\r\\nTotal demerits \",\"Driver\":{\"Flag51\":false,\"LastName\":\"cor\",\"LicenseNumber\":\"0904498\",\"LoadedFromICBC\":false,\"MedicalIssueDate\":null},\"SequenceNumber\":2,\"CommentTypeCode\":\"W\",\"UserId\":\"BCEID\\\\LX0P\",\"CaseId\":\"407f23fb-5500-ec11-b82b-fbf509044982\",\"CommentId\":null,\"CommentDate\":null}";
            
            request.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");

            response = _client.SendAsync(request).GetAwaiter().GetResult();

            responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            // now check for the comment to be in the response.

            request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/0904498/Comments");

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
        public async void DfcmsGetAllComments()
        {
            Login();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/AllComments");

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
