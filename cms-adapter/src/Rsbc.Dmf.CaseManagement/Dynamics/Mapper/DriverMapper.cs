using AutoMapper;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System.Collections.Generic;

namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public class DriverAutoMapperProfile : Profile
    {
        public DriverAutoMapperProfile()
        {
            // NOTE driver name, and DOB should be coming from ICBC adapter, not Dynamics
            CreateMap<dfp_driver, Dto.Driver>()
                .ForMember(dest => dest.DriverLicenceNumber, opt => opt.MapFrom(src => src.dfp_licensenumber));

            CreateMap<dfp_driver, DriverInfo>()
                .ForMember(dest => dest.DriverId, opt => opt.MapFrom(src => src.dfp_driverid.ToString()))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.dfp_firstname))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.dfp_surname))
                .ForMember(dest => dest.LicenceNumber, opt => opt.MapFrom(src => src.dfp_licensenumber))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => new List<DriverDocument>()));
        }
    }
}
