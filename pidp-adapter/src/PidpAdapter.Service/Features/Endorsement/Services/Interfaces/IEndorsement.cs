using PidpAdapter.Endorsement.Model;

namespace PidpAdapter.Endorsement.Services.Interfaces;

public interface IEndorsement
{
    Task<IEnumerable<EndorsementData.Model>> GetEndorsement(string hpidp);
}
