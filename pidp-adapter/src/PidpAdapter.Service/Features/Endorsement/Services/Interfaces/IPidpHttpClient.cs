using PidpAdapter.Endorsement.Model;

namespace PidpAdapter.Endorsement.Services.Interfaces;

public interface IPidpHttpClient
{
    Task<IEnumerable<EndorsementData.Model>> GetEndorsements(string hpidp);
}
