using AutoMapper;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public class FlagAutoMapperProfile : Profile
    {
        public FlagAutoMapperProfile() 
        {
            CreateMap<dfp_flag, CaseManagement.Flag>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.dfp_flagid))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.dfp_description))
                .ForMember(dest => dest.FlagType, opt => opt.MapFrom(src => ConvertFlagType(src.dfp_type)))
                .ForMember(dest => dest.FormId, opt => opt.MapFrom(src => src.dfp_formid));

            CreateMap<dfp_dmerflag, CaseManagement.Flag>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.dfp_FlagId.dfp_flagid))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.dfp_FlagId.dfp_description))
                .ForMember(dest => dest.FlagType, opt => opt.MapFrom(src => ConvertFlagType(src.dfp_FlagId.dfp_type)))
                .ForMember(dest => dest.FormId, opt => opt.MapFrom(src => src.dfp_FlagId.dfp_formid));
        }

        private FlagTypeOptionSet? ConvertFlagType(int? value)
        {
            return value == null ? null : (FlagTypeOptionSet?)value;
        }
    }
}
