namespace PidpAdapter.Endorsement.Services.Interfaces;

public interface IPidpHttpClient
{
    Task<IEnumerable<Model.EndorsementData.Model>> GetEndorsements(string hpidp);
}
