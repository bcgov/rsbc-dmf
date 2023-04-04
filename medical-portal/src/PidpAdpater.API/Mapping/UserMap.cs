using pdipadapter.Data.ef;
using pdipadapter.Features.Users.Commands;
using pdipadapter.Features.Users.Models;

namespace pdipadapter.Mapping;

public class UserMap: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<JustinUser, CreateUserCommand>()
            .Map(dest => dest.UserName, src => src.UserName)
            .Map(dest => dest.IsDisable, src => src.IsDisabled)
            .Map(dest => dest.ParticipantId, src => src.ParticipantId)
            .Map(dest => dest.FirstName, src => src.Person.FirstName)
           .Map(dest => dest.LastName, src => src.Person.Surname)
           .Map(dest => dest.MiddleName, src => src.Person.MiddleNames)
           .Map(dest => dest.Email, src => src.Person.Email)
           .Map(dest => dest.PhoneNumber, src => src.Person.Phone)
           .Map(dest => dest.BirthDate, src => src.Person.BirthDate)
           .Map(dest => dest.IsDisable, src => src.Person.IsDisabled)
            .Map(dest => dest.AgencyId, src => src.AgencyId)
            .Map(dest => dest.PartyTypeCode, src => src.PartyType.Code)
            .Map(dest => dest.Roles, src => src.UserRoles.Select(r=>r.Role));

        config.NewConfig<CreateUserCommand, JustinUser>()
            .Map(dest => dest.UserName, src => src.UserName)
            .Map(dest => dest.IsDisabled, src => src.IsDisable)
            .Map(dest => dest.ParticipantId, src => src.ParticipantId)
            .Map(dest => dest.Person.FirstName, src => src.FirstName)
            .Map(dest => dest.Person.Surname, src => src.LastName)
            .Map(dest => dest.Person.MiddleNames, src => src.MiddleName)
            .Map(dest => dest.Person.Email, src => src.Email)
            .Map(dest => dest.Person.Phone, src => src.PhoneNumber)
            .Map(dest => dest.Person.BirthDate, src => src.BirthDate)
            .Map(dest => dest.Person.IsDisabled, src => src.IsDisable)
            .Map(dest => dest.AgencyId, src => src.AgencyId)
            .Map(dest => dest.AgencyId, src => src.AgencyId)
            .Map(dest => dest.PartyType.Code, src => src.PartyTypeCode);

        config.NewConfig<CreateUserCommand, JustinUserRole>()
            //.Map(dest => dest.RoleId, src => src.UserId)
            .Map(dest => dest.Role, src => src);


        //config.NewConfig<UserModel, CreateUserCommand>()
        //   .Map(dest => dest.UserId, src => src.UserId)
        //   .Map(dest => dest.UserName, src => src.UserName)
        //   .Map(dest => dest.IsDisable, src => src.IsDisable)
        //   .Map(dest => dest.ParticipantId, src => src.ParticipantId)
        //   .Map(dest => dest.PersonId, src => src.PersonId)
        //   .Map(dest => dest.AgencyId, src => src.AgencyId)
        //   .Map(dest => dest.PartyTypeCode, src => src.PartyTypeCode)
        //   .Map(dest => dest.Roles, src => src.Roles);

        //config.NewConfig<CreateUserCommand, UserModel>()
        //    .Map(dest => dest.UserId, src => src.UserId)
        //    .Map(dest => dest.UserName, src => src.UserName)
        //    .Map(dest => dest.IsDisable, src => src.IsDisable)
        //    .Map(dest => dest.ParticipantId, src => src.ParticipantId)
        //    .Map(dest => dest.PersonId, src => src.PersonId)
        //    .Map(dest => dest.AgencyId, src => src.AgencyId)
        //    .Map(dest => dest.PartyTypeCode, src => src.PartyTypeCode)
        //    .Map(dest => dest.Roles, src => src.Roles); 

    }
}
