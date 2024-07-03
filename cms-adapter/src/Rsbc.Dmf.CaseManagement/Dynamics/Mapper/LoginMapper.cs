using AutoMapper;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public class LoginAutoMapperProfile : Profile
    {
        public LoginAutoMapperProfile()
        {
            CreateMap<dfp_login, Dto.Login>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.dfp_name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.dfp_email))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.dfp_userid))
                .ForMember(dest => dest.LoginId, opt => opt.MapFrom(src => src.dfp_loginid))
                .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.dfp_DriverId));
        }
    }
}