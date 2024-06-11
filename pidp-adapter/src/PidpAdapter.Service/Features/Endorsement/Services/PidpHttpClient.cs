using PidpAdapter.Endorsement.Model;
using PidpAdapter.Endorsement.Services.Interfaces;

namespace PidpAdapter.Endorsement.Services;

public class PidpHttpClient : IPidpHttpClient
{
    private readonly HttpClient _httpClient;

    public PidpHttpClient(HttpClient httpClient, ILogger<PidpHttpClient> logger) 
    { 
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Model.EndorsementData.Model>> GetEndorsements(string hpDid)
    {
        var response = await _httpClient.GetAsync($"/api/v1/ext/parties/{hpDid}/endorsements").ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<IEnumerable<Model.EndorsementData.Model>>();
    }
}
