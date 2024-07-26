using Microsoft.Extensions.Configuration;
using PidpAdapter;
using System.Threading.Tasks;
using Xunit;

public class EndorsementTests : ApiIntegrationTestBase
{
    // TODO this won't work on Github Actions until we add a mock service for PidpManagerClient
    //private readonly PidpManager.PidpManagerClient _pidpAdapterClient;

    public EndorsementTests(/*PidpManager.PidpManagerClient pidpAdapterClient, */IConfiguration configuration) : base(configuration)
    {
        //_pidpAdapterClient = pidpAdapterClient;
    }

    [Fact]
    public async Task Get_Endorsements()
    {
        var hpDid = _configuration["TEST_PIDP_USER_ID"];
        if (string.IsNullOrEmpty(hpDid))
            return;

        var getEndorsementsRequest = new GetEndorsementsRequest { UserId = hpDid };
        //var reply = await _pidpAdapterClient.GetEndorsementsAsync(getEndorsementsRequest);

        //Assert.NotNull(reply);
    }
}
