

using Microsoft.Extensions.Configuration;
using Pssg.DocumentStorageAdapter;
using Pssg.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Pssg.DocumentStorageAdapter.ViewModels;
using Pssg.DocumentStorageAdapter;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Pssg.DocumentStorageAdapter.Tests
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
    }


    public class DynamicsInterfaceTest : ApiIntegrationTestBaseWithLogin
    {

        public DynamicsInterfaceTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        { }


        private void Login()
        {
            // TODO - do a JWT login

        }

        /// <summary>
        /// Test the MS Dynamics interface
        /// </summary>
        [Fact]
        public async void UploadDownloadTest()
        {
            S3 s3 = new S3(Configuration);

            Random random = new Random();

            string testString = "test";
            
            Login();

            var request = new HttpRequestMessage(HttpMethod.Post, "/file/upload");

            Upload upload = new Upload()
            {
                Body = Convert.ToBase64String( Encoding.ASCII.GetBytes(testString) ),
                ContentType = "text/plain",
                EntityId = Guid.NewGuid(),
                EntityName = "contact",
                FileName = "upload-test" + random.Next().ToString(),
                Tag1 = "TEST-TAG1",
                Tag2 = "TEST-TAG2",
                Tag3 = "TEST-TAG3"
            };

            string expectedFilename = s3.GetServerRelativeUrl(s3.GetDocumentListTitle(upload.EntityName),
                $"{upload.EntityId}", upload.FileName);

            string jsonString = JsonConvert.SerializeObject(upload);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            jsonString = await response.Content.ReadAsStringAsync();
            Download download = JsonConvert.DeserializeObject<Download>(jsonString);

            // filename should match.
            Assert.Equal(expectedFilename, download.FileUrl);

            // download the file

            request = new HttpRequestMessage(HttpMethod.Post, "/file/download");
            jsonString = JsonConvert.SerializeObject(download);
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            // content should match
            /* test for download as file
            string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Equal(result, testString);
            */

            // test for download as object
            jsonString = await response.Content.ReadAsStringAsync();
            Upload received = JsonConvert.DeserializeObject<Upload>(jsonString);

            Assert.Equal(Convert.FromBase64String(received.Body), Encoding.ASCII.GetBytes(testString));


        }

        
        

    }
}
