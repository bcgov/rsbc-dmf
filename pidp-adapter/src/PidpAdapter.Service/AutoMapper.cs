using AutoMapper;

namespace PidpAdapter
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Endorsement.Model.EndorsementData.Model, EndorsementDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Hpdid))
                .ForMember(dest => dest.Licenses, opt => opt.MapFrom(src => src.Licences));
            CreateMap<Endorsement.Model.EndorsementData.Model.LicenceInformation, LicenseDto>();
        }
    }

    public static class AutoMapperExtensions
    {
        public static void AddAutoMapperSingleton(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
