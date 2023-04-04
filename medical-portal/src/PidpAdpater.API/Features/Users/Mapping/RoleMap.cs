using pdipadapter.Data.ef;
using pdipadapter.Features.Roles.Models;
using Mapster;

namespace pdipadapter.Features.Users.Mapping;

public class RoleMap : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<JustinRole, RoleModel>()
               .Map(dest => dest.Id, src => src.RoleId)
               .Map(dest => dest.Name, src => src.Name)
               .Map(dest => dest.IsPublic, src => src.IsPublic)
               .Map(dest => dest.IsDisable, src => src.IsDisabled)
               .Map(dest => dest.Description, src => src.Description);

        config.NewConfig<RoleModel, JustinRole>()
       .Map(dest => dest.RoleId, src => src.Id)
       .Map(dest => dest.Name, src => src.Name)
       .Map(dest => dest.IsPublic, src => src.IsPublic)
       .Map(dest => dest.IsDisabled, src => src.IsDisable)
       .Map(dest => dest.Description, src => src.Description);

        config.NewConfig<JustinUserRole, RoleModel>()
            .Map(dest => dest.Id, src => src.RoleId)
            .Map(dest => dest.Name, src => src.Role.Name)
            .Map(dest => dest.Description, src => src.Role.Description);

        config.NewConfig<RoleModel, JustinUserRole>()
    .Map(dest => dest.RoleId, src => src.Id)
    .Map(dest => dest.Role, src => src);
    }
}
