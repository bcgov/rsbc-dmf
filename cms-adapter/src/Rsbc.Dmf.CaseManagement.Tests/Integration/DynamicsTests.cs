using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.CaseManagement.Service;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class DynamicsTests : WebAppTestBase
    {
        public DynamicsTests(ITestOutputHelper output, WebApplicationFactory<Startup> webApplicationFactory) : base(output, webApplicationFactory)
        {
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task GetSecurityToken()
        {
            var tokenProvider = services.GetRequiredService<ISecurityTokenProvider>();
            testLogger.LogDebug("Authorization: Bearer {0}", await tokenProvider.AcquireToken());
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanConnectToDynamics()
        {
            var caseManager = services.GetRequiredService<ICaseManager>();
            var result = await caseManager.CaseSearch(new CaseSearchRequest());
            result.ShouldNotBeNull().Items.ShouldBeEmpty();
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task CanSetFlags()
        {
            List<string> flags = new List<string>()
            {
                {"testFlag - 1"},
                {"testFlag - 2"}
            };
            var caseManager = services.GetRequiredService<ICaseManager>();
            var result = await caseManager.SetCaseFlags("111", flags, testLogger);
            result.ShouldNotBeNull().Success.ShouldBe(true);
        }
    }
}