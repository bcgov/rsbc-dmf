namespace MedicalPortal.API.Features.Endorsement.Services.Interfaces;
public interface IEndorsement
{
    Task<IEnumerable<Model.Endorsement>> GetEndorsement(string hpidp);
}

