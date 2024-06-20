using AutoMapper;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public class DriverAutoMapperProfile : Profile
    {
        public DriverAutoMapperProfile()
        {
            // NOTE driver name, and DOB should be coming from ICBC adapter, not Dynamics
            CreateMap<dfp_driver, Dto.Driver>()
                .ForMember(dest => dest.DriverLicenceNumber, opt => opt.MapFrom(src => src.dfp_licensenumber));
        }
    }
}
