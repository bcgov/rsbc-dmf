using PidpAdapter.Infrastructure.HttpClients;
using PidpAdapter.Endorsement.Model;
using PidpAdapter.Endorsement.Services.Interfaces;

namespace PidpAdapter.Endorsement.Services;

{
    private readonly HttpClient _httpClient;

    public Endorsement(HttpClient httpClient, ILogger<Endorsement> logger) 
    { 
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<EndorsementData.Model>> GetEndorsement(string hpDid)
    {
        hpDid = hpDid.Replace("@bcsc", "");
        var response = await _httpClient.GetAsync($"/api/v1/ext/parties/{hpDid}/endorsements").ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            {
                // TODO
            }
        return await response.Content.ReadFromJsonAsync<IEnumerable<EndorsementData.Model>>();
    }
}

public static partial class JustinParticipantClientLoggingExtensions
{
    [LoggerMessage(1, LogLevel.Warning, "No Endorsement found in PiDP with Hpdid = {hpdid}.")]
    public static partial void LogNoEndorsementFound(this ILogger logger, string hpdid);
}
