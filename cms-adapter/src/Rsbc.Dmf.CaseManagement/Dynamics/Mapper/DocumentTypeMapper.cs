using AutoMapper;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public class DocumentTypeMapper
    {
        public class DocumentTypeAutoMapperProfile : Profile
        {
            public DocumentTypeAutoMapperProfile()
            {
                int id = 0;

                CreateMap<dfp_documentsubtype, DocumentSubType>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.dfp_name))
                    .AfterMap((src, dest) => { dest.Id = id++; });
            }
        }
    }
}
