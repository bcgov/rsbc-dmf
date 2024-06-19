using AutoMapper;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public class DriverAutoMapperProfile : Profile
    {
        public DriverAutoMapperProfile()
        {
            CreateMap<dfp_driver, Dto.Driver>()
                //.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.dfp_firstname))
                //.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.dfp_surname))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.dfp_dob))
                .ForMember(dest => dest.DriverLicenceNumber, opt => opt.MapFrom(src => src.dfp_licensenumber));
                // get this info from ICBC instead
                //.ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.dfp_PersonId));
        }
    }
}
