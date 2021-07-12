using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RSBC.DMF.CaseManagement.Dynamics;
using RSBC.DMF.CaseManagement.Service;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RSBC.DMF.CaseManagement.Tests.Integration
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
    }
}