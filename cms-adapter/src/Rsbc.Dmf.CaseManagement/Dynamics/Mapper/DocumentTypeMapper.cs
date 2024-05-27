using AutoMapper;
using Rsbc.Dmf.CaseManagement.Model.Dto;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public class DocumentTypeMapper
    {
        public class DocumentTypeAutoMapperProfile : Profile
        {
            public DocumentTypeAutoMapperProfile()
            {
                CreateMap<dfp_documentsubtype, DocumentSubType>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.dfp_name));

                CreateMap<dfp_submittaltype, DocumentType>()
                    .ForMember(dest => dest.DocumentName, opt => opt.MapFrom(src => src.dfp_name));
                   


            }
        }
    }
}
