

using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.IcbcAdapter;
using Pssg.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.ViewModels;

namespace Rsbc.Dmf.IcbcAdapter.Tests
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

        private async void TestDl(string testDl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/DriverHistory?driversLicence=" + testDl);

            var response = _client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            // parse as JSON.
            var jsonString = await response.Content.ReadAsStringAsync();

            Driver clientResult = JsonConvert.DeserializeObject<Driver>(jsonString);


            // content should match

            Assert.Equal(clientResult.DriverMasterStatus.LicenceNumber.Value, int.Parse(testDl));
        }

        /// <summary>
        /// Test the MS Dynamics interface
        /// </summary>
        [Fact]
        public async void TestDriverHistory()
        {
            string testDl = Configuration["ICBC_TEST_DL"];

            Login();

            TestDl(testDl);

            testDl = Configuration["ICBC_ALTERNATE_TEST_DL"];            

            TestDl(testDl);

        }
    }
}
