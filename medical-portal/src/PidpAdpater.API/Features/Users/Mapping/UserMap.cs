global using Mapster;
using pdipadapter.Core.Extension;
using pdipadapter.Data.ef;
using pdipadapter.Features.Users.Commands;
using pdipadapter.Features.Users.Models;
using System.Linq;


namespace pdipadapter.Features.Users.Mapping;

public class UserMap : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<JustinUser, CreateUserCommand>()
            .Map(src => src.UserName, dest => dest.UserName)
            .Map(src => src.IsDisable, dest => dest.IsDisabled)
            .Map(src => src.ParticipantId, dest => dest.ParticipantId)
            //.Map(src => src.DigitalIdentifier, dest => dest.DigitalIdentifier)
            .Map(src=>src.FirstName, dest =>dest.Person.FirstName)
            .Map(src => src.LastName, dest => dest.Person.Surname)
            .Map(src => src.MiddleName, dest => dest.Person.MiddleNames)
            .Map(src => src.Email, dest => dest.Person.Email)
            .Map(src => src.BirthDate, dest => dest.Person.BirthDate)
            .Map(src => src.PreferredName, dest => dest.Person.PreferredName)
            .Map(dest => dest.IsDisable, src => src.Person.IsDisabled)
            .Map(src => src.AgencyId, dest => dest.AgencyId)
            .Map(src => src.PhoneNumber, dest => dest.Person.Phone)
            .Map(src => src.PartyTypeCode, dest => dest.PartyType)
            .Map(src => src.Roles, dest => dest.UserRoles.Select(r=>r.Role).ToArray()); 

        config.NewConfig<CreateUserCommand, JustinUser>()
            .Map(dest => dest.UserName, src => src.UserName)
            .Map(dest => dest.IsDisabled, src => src.IsDisable)
            .Map(dest => dest.ParticipantId, src => src.ParticipantId)
           // .Map(src => src.DigitalIdentifier, dest => dest.DigitalIdentifier)
            .Map(src => src.Person.FirstName, dest => dest.FirstName)
            .Map(src => src.Person.Surname, dest => dest.LastName)
            .Map(src => src.Person.MiddleNames, dest => dest.MiddleName)
            .Map(src => src.Person .Email, dest => dest.Email)
            .Map(src => src.Person.BirthDate, dest => dest.BirthDate)
            .Map(src => src.Person.PreferredName, dest => dest.PreferredName)
            .Map(dest => dest.Person.IsDisabled, src => src.IsDisable)
            .Map(dest => dest.AgencyId, src => src.AgencyId)
            .Map(dest => dest.Person.Phone, src => src.PhoneNumber)
            .Map(dest => dest.PartyType.Code, src => src.PartyTypeCode)
            .Map(dest => dest.UserRoles, src => src.Roles)
            .AfterMappingInline((m,e) => UpdateUser(m, e));



    }
    private static void UpdateUser(CreateUserCommand model, JustinUser entity)
    {
        entity.UserRoles.Where(a => a != null).ForEach(a =>
        {
            a.UserId = entity.UserId;
            a.RoleId = model.Roles.FirstOrDefault()?.Id ?? 0;
            a.Role = default;
        });
        
    }
}
