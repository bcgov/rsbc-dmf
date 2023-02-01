

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

    public class CommentTest : ApiIntegrationTestBase
    {

        public CommentTest(HttpClientFixture fixture)
            : base(fixture)
        {
            
        }



        


        


            [Fact]
        public async void TestDeleteComment()
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
            string commentId = null;
            if (!string.IsNullOrEmpty(Configuration["CMS_ADAPTER_URI"]))
            {
                bool found = false;

                foreach (var item in comments)
                {
                    if (item.CommentText == comment.CommentText)
                    {
                        found = true;
                        commentId = item.CommentId;
                        break;
                    }
                }

                Assert.True(found);

                // do a delete

                request = new HttpRequestMessage(HttpMethod.Post, $"/Comments/Delete/{commentId}");

                response = _client.SendAsync(request).GetAwaiter().GetResult();

                responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();
            }

            
        }

       // {"userId":"IDIR\\SMILLAR","driver":\{"licenseNumber":"0200103","lastName":"KNI","loadedFromICBC":false,"flag51":false} 
// ,"sequenceNumber":4,"commentTypeCode":"W","commentText":"test new one"}



    }

}
