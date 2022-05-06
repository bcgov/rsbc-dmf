

using Microsoft.Extensions.Configuration;
using Pssg.Dmf.LegacyAdapter;
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

namespace Pssg.Unit.Tests.Dmf.LegacyAdapter
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


    public class LegacyAdapterTest : ApiIntegrationTestBaseWithLogin
    {

        public LegacyAdapterTest(CustomWebApplicationFactory<Startup> factory)
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
        public async void TestCaseExist()
        {
            string testDl = Configuration["ICBC_TEST_DL"];

            Login();

            var request = new HttpRequestMessage(HttpMethod.Get, "/Cases/Exist?driversLicence=" + testDl);

            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void TestCaseDocuments()
        {
            string testDl = Configuration["ICBC_TEST_DL"];

            Login();

            var request = new HttpRequestMessage(HttpMethod.Post, "/Cases/Documents");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async void TestDfwebSubmitComment()
        {
            string testDl = Configuration["ICBC_TEST_DL"];

            Login();

            var request = new HttpRequestMessage(HttpMethod.Post, $"/Drivers/{testDl}/Comments");

            var driver = new Pssg.Dmf.LegacyAdapter.ViewModels.Driver() { LicenseNumber = "1000098"};

            var comment = new Pssg.Dmf.LegacyAdapter.ViewModels.Comment() 
            {  
                CommentText = "This is a test comment",
                Driver = driver,
                SequenceNumber = 3,
                CommentTypeCode = "W",
                UserId = "TESTUSER",
                CaseId = Guid.NewGuid().ToString()           
            };

            var stringContent = JsonConvert.SerializeObject(comment);

            request.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");

            var response = _client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }


    }
}
