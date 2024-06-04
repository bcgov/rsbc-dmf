namespace OneHealthAdapter.Endorsement.Services.Interfaces;

public interface IEndorsement
{
    Task<IEnumerable<Model.Endorsement>> GetEndorsement(string hpidp);
}
