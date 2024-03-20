using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.CaseManagement.Service;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class CallbackServiceTests : WebAppTestBase
    {
        private readonly CallbackService _callbackService;
        private readonly IConfiguration _configuration;

        public CallbackServiceTests(ITestOutputHelper output) : base(output)
        {
            var logger = services.GetRequiredService<ILogger<CallbackService>>();
            var callbackManager = services.GetRequiredService<ICallbackManager>();
            var mapper = services.GetRequiredService<IMapper>();
            _configuration = services.GetRequiredService<IConfiguration>();
            _callbackService = new CallbackService(callbackManager, logger, mapper);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task GetDriverCallbacks()
        {
            var request = new DriverIdRequest();
            request.Id = _configuration["DRIVER_WITH_CALLBACKS"];
            var response = await _callbackService.GetDriverCallbacks(request, null);

            Assert.NotNull(response);
        }
    }
}
