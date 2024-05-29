using Mapster;
using MedicalPortal.API.Features.Endorsement.Model;
using Rsbc.Dmf.CaseManagement.Service;

namespace PidpAdapter.API.Features.Users.Mapping;
public class ContactMap : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PractitionerReply, Contact>()
              .Map(dest => dest.Role, src => src.Role)
              .Map(dest => dest.FirstName, src => src.FirstName)
              .Map(dest => dest.LastName, src => src.LastName)
              .Map(dest => dest.BirthDate, src => src.Birthdate)
              .Map(dest => dest.ContactId, src => src.ContactId)
              .Map(dest => dest.Email, src => src.Email);

        //throw new NotImplementedException();
    }
}

