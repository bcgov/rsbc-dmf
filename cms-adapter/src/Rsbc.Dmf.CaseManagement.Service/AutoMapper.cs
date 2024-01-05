using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;

namespace Rsbc.Dmf.CaseManagement.Service
{

    public class MappingProfile : Profile 
    {
        public MappingProfile()
        {
            CreateMap<DateTimeOffset, Timestamp>()
                .ConvertUsing(x => Timestamp.FromDateTimeOffset(x));
            CreateMap<DateTimeOffset?, Timestamp>()
                .ConvertUsing(x => x == null ? null : Timestamp.FromDateTimeOffset(x.Value));
            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(x => Timestamp.FromDateTime(x.ToUniversalTime()));
            CreateMap<CaseManagement.Address, Address>()
                .AddTransform(NullConverter);
            CreateMap<CaseManagement.Driver, Driver>()
                .AddTransform(NullConverter);
            CreateMap<CaseManagement.LegacyDocument, LegacyDocument>()
                .AddTransform(NullConverter);
            CreateMap<CaseManagement.Callback, Callback>()
                .AddTransform(NullConverter);
    }

        private Expression<Func<string, string>> NullConverter = x => x ?? string.Empty;
    }

    public static class AutoMapperEx
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