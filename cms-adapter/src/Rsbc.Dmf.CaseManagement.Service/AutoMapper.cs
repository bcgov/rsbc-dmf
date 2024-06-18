﻿using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
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
            CreateMap<DomainModels.Document, Document>()
                .ForMember(dest => dest.PractitionerName, opt => opt.MapFrom(src => src.PractitionerName));
            CreateMap<DomainModels.Case, Case>();
            CreateMap<DomainModels.Person, Person>();
            CreateMap<DomainModels.DocumentType, DocumentType>();
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
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}