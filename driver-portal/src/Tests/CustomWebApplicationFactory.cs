using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rsbc.Dmf.CaseManagement.Helpers;
using Pssg.DocumentStorageAdapter.Helpers;
using Rsbc.Dmf.DriverPortal.Api;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    /// <summary>
    /// web application factory used for testing HttpClient
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly IConfiguration _configuration;

        public CustomWebApplicationFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {           
            // document storage client
            builder.ConfigureTestServices(services =>
            {
                string documentStorageAdapterURI = _configuration["DOCUMENT_STORAGE_ADAPTER_URI"];
                if (true || string.IsNullOrEmpty(documentStorageAdapterURI))
                {
                    // add the mock
                    var documentStorageAdapterClient = DocumentStorageHelper.CreateMock(_configuration);
                    services.AddTransient(_ => documentStorageAdapterClient);
                }
                else
                {
                    services.AddDocumentStorageClient(_configuration);
                }
            });

            // case management client
            builder.ConfigureTestServices(services =>
            {
                string cmsAdapterURI = _configuration["CMS_ADAPTER_URI"];
                if (string.IsNullOrEmpty(cmsAdapterURI))
                {
                    // setup from Mock
                    var caseManagerClient = CmsHelper.CreateMock(_configuration);
                    services.AddTransient(_ => caseManagerClient);
                }
                else
                {
                    services.AddCaseManagementAdapterClient(_configuration);
                }
            });

            builder
                .UseSolutionRelativeContentRoot("")
                .UseEnvironment("Staging")
                .UseConfiguration(_configuration);
        }
    }
}