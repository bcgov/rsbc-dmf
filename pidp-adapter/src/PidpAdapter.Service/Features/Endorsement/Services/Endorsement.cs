using PidpAdapter.Infrastructure.HttpClients;
using PidpAdapter.Endorsement.Model;
using PidpAdapter.Endorsement.Services.Interfaces;
using static PidpAdapter.Endorsement.Model.EndorsementData.Model;

namespace PidpAdapter.Endorsement.Services;
public class Endorsement : BaseClient, IEndorsement
{
    public Endorsement(HttpClient client, ILogger<Endorsement> logger) : base(client, logger) { }

    public async Task<IEnumerable<Model.Endorsement>> GetEndorsement(string hpDid)
    public async Task<IEnumerable<EndorsementData.Model>> GetEndorsement(string hpDid)
    {
        hpDid = hpDid.Replace("@bcsc", "");
        var endorsementResult = await this.GetAsync<IEnumerable<EndorsementData.Model>>($"/api/v1/ext/parties/{hpDid}/endorsements").ConfigureAwait(false);

        if (!endorsementResult.IsSuccess || !endorsementResult.Value.Any())
        {
            if (!endorsementResult.IsSuccess)
            {
                // Log error or handle appropriately
                // TODO
            }
            else
            {
                this.Logger.LogNoEndorsementFound(hpDid);
            }
            return null;
        }

        return endorsementResult.Value;
    }
}

public static partial class JustinParticipantClientLoggingExtensions
{
    [LoggerMessage(1, LogLevel.Warning, "No Endorsement found in PiDP with Hpdid = {hpdid}.")]
    public static partial void LogNoEndorsementFound(this ILogger logger, string hpdid);
}
