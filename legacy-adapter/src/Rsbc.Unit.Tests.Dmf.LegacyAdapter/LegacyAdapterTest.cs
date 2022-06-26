

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

namespace Rsbc.Unit.Tests.Dmf.LegacyAdapter
{
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
    }

    public abstract class ApiIntegrationTestBaseWithLogin : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected readonly CustomWebApplicationFactory<Startup> _factory;

        protected HttpClient _client { get; }

        protected readonly IConfiguration Configuration;

        public ApiIntegrationTestBaseWithLogin(CustomWebApplicationFactory<Startup> fixture)
        {
            _factory = fixture;
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>() // Add secrets from the service.
                .AddEnvironmentVariables()
                .Build();
            // determine if this is an external or internal test.
            if (Configuration["TEST_BASE_URI"] != null)
            {
                // allow self signed certificates
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                _client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(Configuration["TEST_BASE_URI"])
                };
            }

            else // local test
            {
                _factory = fixture;
                _client = _factory
                    .CreateClient(new WebApplicationFactoryClientOptions
                    {
                        AllowAutoRedirect = false,
                    });

            }
        }


        protected void Login()
        {
            // determine if authentication is enabled.

            if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
            {
                string encodedSecret = HttpUtility.UrlEncode(Configuration["JWT_TOKEN_KEY"]);
                var request = new HttpRequestMessage(HttpMethod.Get, "/Authentication/Token?secret=" + encodedSecret);
                var response = _client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                var token = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(token))
                {
                    // Add the bearer token to the client.
                    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
            }


        }
    }


    public class LegacyAdapterTest : ApiIntegrationTestBaseWithLogin
    {
        public string testDl;
        public string testSurcode;
        public LegacyAdapterTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {

            testDl = Configuration["ICBC_TEST_DL"];
            testSurcode = Configuration["ICBC_TEST_SURCODE"];
        }



        /// <summary>
        /// Test the MS Dynamics interface
        /// </summary>
        [Fact]
        public async void CaseExist()
        {            
            Login();

            var request = new HttpRequestMessage(HttpMethod.Get, "/Cases/Exist?driversLicence=" + testDl);

            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void CaseDocuments()
        {         
            Login();

            var request = new HttpRequestMessage(HttpMethod.Post, "/Cases/Documents");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
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
                SequenceNumber = 4,
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

            bool found = false;

            foreach (var item in comments)
            {
                if (item.CommentText == comment.CommentText && item.Driver.LastName == comment.Driver.LastName)
                {
                    found = true;
                }
            }

            Assert.True(found);
        }

       // {"userId":"IDIR\\SMILLAR","driver":\{"licenseNumber":"0200103","lastName":"KNI","loadedFromICBC":false,"flag51":false} 
// ,"sequenceNumber":4,"commentTypeCode":"W","commentText":"test new one"}


    [Fact]
    public async void DfwebSubmitCommentNoCase()
    {
        Login();


        var request = new HttpRequestMessage(HttpMethod.Post, $"/Drivers/{testDl}/Comments");

        var driver = new Rsbc.Dmf.LegacyAdapter.ViewModels.Driver()
        { LicenseNumber = testDl, LastName = testSurcode };

        var comment = new Rsbc.Dmf.LegacyAdapter.ViewModels.Comment()
        {
            CommentText = "test new one",
            Driver = driver,
            SequenceNumber = 4,
            CommentTypeCode = "W",
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
        public async void DfwebGetComments()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Comments");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async void DfcmsAddDocument()
        {
            Login();

            var request = new HttpRequestMessage(HttpMethod.Post, $"/Drivers/{testDl}/Documents");

            var driver = new Rsbc.Dmf.LegacyAdapter.ViewModels.Driver() { LicenseNumber = testDl, LastName = testSurcode };

            string testString = "Test1234";
            // Convert a C# string to a byte array    
            byte[] bytes = Encoding.ASCII.GetBytes(testString);

            var comment = new Rsbc.Dmf.LegacyAdapter.ViewModels.Document()
            {
                FaxReceivedDate = DateTimeOffset.Now,
                DocumentId = Guid.NewGuid().ToString(),
                FileContents = bytes,
                Driver = driver,
                SequenceNumber = 3,                
                UserId = "IDIR\\TESTUSER",
                CaseId = Guid.NewGuid().ToString()
            };

            var stringContent = JsonConvert.SerializeObject(comment);

            request.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void DfcmsGetDocuments()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Documents");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void TestLoginRequired()
        {            

            var request = new HttpRequestMessage(HttpMethod.Get, $"/Drivers/{testDl}/Cases");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            // should be 401 if there was no login.
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }

    }

}
