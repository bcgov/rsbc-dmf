using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using Grpc.Net.Client;
using Xunit.Abstractions;

namespace Rsbc.Dmf.CaseManagement.Tests
{
    public class WebAppTestBase : TestBase
    {
        //private readonly LoggerFactory loggerFactory;
        protected readonly XUnitWebAppFactory<Service.Startup> webApplicationFactory;

        private readonly ITestOutputHelper output;

        protected IConfiguration configuration => webApplicationFactory.Services.GetRequiredService<IConfiguration>();
        protected IServiceProvider services => new Lazy<IServiceProvider>(() => webApplicationFactory.Services.CreateScope().ServiceProvider).Value;

        protected ILogger testLogger => new XUnitLogger(output, "SUT");

        public WebAppTestBase(ITestOutputHelper output)
        {
            this.webApplicationFactory = new XUnitWebAppFactory<Service.Startup>(output);
            this.output = output;

            var client = webApplicationFactory.CreateDefaultClient();
            _grpcChannel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });
        }

        public GrpcChannel _grpcChannel { get; }
    }
}