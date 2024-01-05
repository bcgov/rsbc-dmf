using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Rsbc.Dmf.DriverPortal.ViewModels;

namespace Rsbc.Dmf.DriverPortal.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Timestamp, DateTimeOffset>()
                .ConvertUsing(x => x.ToDateTimeOffset());
            CreateMap<CaseManagement.Service.Callback, Callback>();
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
