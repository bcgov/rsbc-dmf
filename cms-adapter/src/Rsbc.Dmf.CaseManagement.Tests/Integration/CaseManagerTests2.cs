using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Xunit;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    /// <summary>
    /// CaseManagerTests using XUnit dependency injection
    /// </summary>
    public class CaseManagerTests2
    {
        private readonly ICaseManager _caseManager;
        private readonly IConfiguration _configuration;

        public CaseManagerTests2(ICaseManager caseManager, IConfiguration configuration)
        {
            _caseManager = caseManager;
            _configuration = configuration;
        }

        [Fact]
        public async Task CanGetCaseDetails()
        {
            var caseId = _configuration["ICBC_TEST_CASEID"];
            var c = await _caseManager.GetCaseDetail(caseId);
            Assert.NotNull(c);
        }
    }
}