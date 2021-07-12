using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RSBC.DMF.CaseManagement.Service;
using System;
using Xunit;
using Xunit.Abstractions;

namespace RSBC.DMF.CaseManagement.Tests.Integration
{
    public class WebAppTestBase : IClassFixture<WebApplicationFactory<Startup>>
    {
        //set to null to run tests in this class, requires to be on VPN and Dynamics params configured in secrets.xml
#if RELEASE
        protected const string RequiresDynamics = "Integration tests that requires Dynamics connection via VPN";
#else
        protected const string RequiresDynamics = null;
#endif

        private readonly LoggerFactory loggerFactory;
        protected readonly WebApplicationFactory<Startup> webApplicationFactory;

        protected IConfiguration configuration => webApplicationFactory.Services.GetRequiredService<IConfiguration>();
        protected IServiceProvider services => new Lazy<IServiceProvider>(() => webApplicationFactory.Services.CreateScope().ServiceProvider).Value;
        protected ILogger testLogger => loggerFactory.CreateLogger("SUT");

        public WebAppTestBase(ITestOutputHelper output, WebApplicationFactory<Startup> webApplicationFactory)
        {
            Environment.SetEnvironmentVariable("location__cache__AutoRefreshEnabled", "false");
            loggerFactory = new LoggerFactory(new[] { new XUnitLoggerProvider(output) });

            this.webApplicationFactory = webApplicationFactory.WithWebHostBuilder(builder =>
            {
                builder
                    .ConfigureServices(services => { services.RemoveAll<ILoggerProvider>(); })
                    .ConfigureLogging(lb => { lb.ClearProviders().AddProvider(new XUnitLoggerProvider(output)); });
            });
        }
    }
}