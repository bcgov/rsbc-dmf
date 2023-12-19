using Mapster;
using MedicalPortal.API.Features.Endorsement.Model;
using Rsbc.Dmf.CaseManagement.Service;

namespace pdipadapter.Mapping;

public class ContactMap : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Contact, PractitionerReply>()
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Role, src => src.Role)
           .Map(dest => dest.ContactId, src => src.ContactId)
            //.Map(dest => dest.Birthdate!.ToDateTime()!.ToString()!, src => src.BirthDate)
        .IgnoreNullValues(true);

    }
}
