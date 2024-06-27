using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Rsbc.Dmf.CaseManagement.Dynamics.Mapper;
using System;
using System.Linq.Expressions;
using static Rsbc.Dmf.CaseManagement.Dynamics.DocumentMapper;
using static Rsbc.Dmf.CaseManagement.Dynamics.DocumentTypeMapper;
using static Rsbc.Dmf.CaseManagement.Dynamics.Mapper.CallbackMapper;

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
            CreateMap<DateTime?, Timestamp>()
                .ConvertUsing(x => x == null ? null : Timestamp.FromDateTime(x.Value.ToUniversalTime()));
            CreateMap<Timestamp, DateTimeOffset>()
                .ConvertUsing(x => x.ToDateTimeOffset());
            CreateMap<Timestamp, DateTimeOffset?>()
                .ConvertUsing(x => x == null ? null : x.ToDateTimeOffset());

            CreateMap<CaseManagement.Address, Address>()
                .AddTransform(NullStringConverter);
            CreateMap<CaseManagement.Driver, Driver>()
                .AddTransform(NullStringConverter);
            CreateMap<CaseManagement.LegacyDocument, LegacyDocument>()
                .AddTransform(NullStringConverter);
            CreateMap<CaseManagement.CaseDetail, CaseDetail>()
                .AddTransform(NullStringConverter);
            CreateMap<CaseManagement.DocumentSubType, DocumentSubType>();
            CreateMap<CaseManagement.Callback, Callback>()
                .AddTransform(NullStringConverter);
            CreateMap<Callback, CaseManagement.Callback>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? (Guid?)null : Guid.Parse(src.Id)))
                .AddTransform(NullStringConverter);
            CreateMap<UpdateLoginRequest, CaseManagement.UpdateLoginRequest>();
            CreateMap<FullAddress, CaseManagement.FullAddress>();
            CreateMap<Dto.Document, Document>();
            CreateMap<Dto.Case, Case>();
            CreateMap<Dto.Person, Person>();
            CreateMap<Dto.DocumentType, DocumentType>();
            CreateMap<Dto.Document, DmerCase>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.DmerStatus))
                .AddTransform(NullStringConverter);
            CreateMap<Dto.Login, Provider>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
                .AddTransform(NullStringConverter);
        }

        private Expression<Func<string, string>> NullStringConverter = x => x ?? string.Empty;
    }

    public static class AutoMapperEx
    {
        public static void AddAutoMapperSingleton(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
                mc.AddProfile(new DocumentAutoMapperProfile());
                mc.AddProfile(new DocumentTypeAutoMapperProfile());
                mc.AddProfile(new CallbackMapperProfile());
                mc.AddProfile(new CaseAutoMapperProfile());
                mc.AddProfile(new ContactAutoMapperProfile());
                mc.AddProfile(new DriverAutoMapperProfile());
                mc.AddProfile(new LoginAutoMapperProfile());
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}