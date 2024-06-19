using AutoMapper;
using Rsbc.Dmf.CaseManagement.Dto;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement
{
    public class ContactAutoMapperProfile : Profile
    {
        public ContactAutoMapperProfile()
        {
            CreateMap<contact, Person>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.fullname))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.birthdate));
        }
    }
}
