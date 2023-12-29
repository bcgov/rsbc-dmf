using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rsbc.Dmf.CaseManagement.Service
{
    public class MappingProfile : Profile {
        public MappingProfile()
        {
            CreateMap<DateTimeOffset, Timestamp>()
                .ConvertUsing(x => Timestamp.FromDateTimeOffset(x));
            CreateMap<DateTimeOffset?, Timestamp>()
                .ConvertUsing(x => x == null ? null : Timestamp.FromDateTimeOffset(x.Value));
            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(x => Timestamp.FromDateTime(x.ToUniversalTime()));
            // TODO this should only be done on grpc mappings
            CreateMap<string, string>().ConvertUsing<NullStringConverter>();
            CreateMap<CaseManagement.Address, Address>();
            CreateMap<CaseManagement.Driver, Driver>();
            CreateMap<CaseManagement.LegacyDocument, LegacyDocument>();
        }
    }

    public class NullStringConverter : ITypeConverter<string, string>
    {
        public string Convert(string source, string destination, ResolutionContext context)
        {
            return source ?? string.Empty;
        }
    }

    public static class AutoMapper
    {
        public static void AddAutoMapper(this IServiceCollection services)
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