using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;
using static Rsbc.Dmf.CaseManagement.Dynamics.DocumentMapper;
using static Rsbc.Dmf.CaseManagement.Dynamics.DocumentTypeMapper;

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
                .AddTransform(NullStringConverter);
            CreateMap<CaseManagement.Driver, Driver>()
                .AddTransform(NullStringConverter);
            CreateMap<CaseManagement.LegacyDocument, LegacyDocument>()
                .AddTransform(NullStringConverter);
            CreateMap<CaseManagement.CaseDetail, CaseDetail>()
                .AddTransform(NullStringConverter);
            CreateMap<CaseManagement.DocumentSubType, DocumentSubType>();
        }

        private Expression<Func<string, string>> NullStringConverter = x => x ?? string.Empty;
    }

    public static class AutoMapper
    {
        public static void AddAutoMapperSingleton(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
                mc.AddProfile(new DocumentAutoMapperProfile());
                mc.AddProfile(new DocumentTypeAutoMapperProfile());
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}