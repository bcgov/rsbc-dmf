using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Dynamics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class DynamicsTests : WebAppTestBase
    {
        public DynamicsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task GetSecurityToken()
        {
            var tokenProvider = services.GetRequiredService<ISecurityTokenProvider>();
            testLogger.LogDebug("Authorization: Bearer {0}", await tokenProvider.AcquireToken());
        }
    }
}