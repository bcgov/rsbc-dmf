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
                // NOTE somehow the id keeps incrementing on each test run, not sure how that is possible
                int id = 0;

                CreateMap<dfp_documentsubtype, DocumentSubType>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.dfp_name))
                    .AfterMap((src, dest) => { dest.Id = id++; });
            }
        }
    }
}
