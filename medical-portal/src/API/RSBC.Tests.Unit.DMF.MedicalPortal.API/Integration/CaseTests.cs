using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.CaseManagement.Service;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

public class CaseTests : ApiIntegrationTestBase
{
    public CaseTests(IConfiguration configuration) : base(configuration) { }

    [Fact]
    public async Task Get_Case()
    {
        var caseId = _configuration["ICBC_TEST_CASEID"];
        if (string.IsNullOrEmpty(caseId))
            return;

        var request = new HttpRequestMessage(HttpMethod.Get, $"{CASE_API_BASE}/{caseId}");
        var clientResult = await HttpClientSendRequest<CaseDetail>(request);

        Assert.Equal(clientResult.CaseId, caseId);
    }
}
