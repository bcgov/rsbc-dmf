using AutoMapper;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public class MedicalConditionAutoMapperProfile : Profile
    {
        public MedicalConditionAutoMapperProfile()
        {
            CreateMap<dfp_knownmedicalcondition, CaseManagement.MedicalCondition>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.dfp_MedicalConditionId.dfp_id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.dfp_MedicalConditionId.dfp_description))
                .ForMember(dest => dest.FormId, opt => opt.MapFrom(src => src.dfp_MedicalConditionId.dfp_formid));
        }
    }
}
