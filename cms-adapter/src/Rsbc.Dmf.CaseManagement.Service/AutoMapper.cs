using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Rsbc.Dmf.CaseManagement.Dynamics.Mapper;
using Rsbc.Dmf.CaseManagement.Model;
using System;
using System.Linq.Expressions;
using System.Reflection;
using static Rsbc.Dmf.CaseManagement.Dynamics.DocumentMapper;
using static Rsbc.Dmf.CaseManagement.Dynamics.DocumentTypeMapper;
using static Rsbc.Dmf.CaseManagement.Dynamics.Mapper.CallbackMapper;
using static Rsbc.Dmf.CaseManagement.Service.FlagItem.Types;

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

            // to CaseManagement from proto
            CreateMap<Callback, CaseManagement.Callback>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? (Guid?)null : Guid.Parse(src.Id)))
                .AddTransform(NullStringConverter);
            CreateMap<UpdateLoginRequest, CaseManagement.UpdateLoginRequest>();
            CreateMap<FullAddress, CaseManagement.FullAddress>();

            // to Proto from CaseManagement
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
            CreateMap<UpdateDocumentRequest, UpdateDocumentCommand>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? (Guid?)null : Guid.Parse(src.Id)))
              .ForMember(dest => dest.DpsPriority, opt => opt.MapFrom(src => src.DpsPriority))
              .ForMember(dest => dest.Queue, opt => opt.MapFrom(src => src.Queue));

            CreateMap<Dto.Document, Document>()
                .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Login))
                .AddTransform(NullStringConverter);
            CreateMap<Dto.Case, Case>()
                .AddTransform(NullStringConverter);
            CreateMap<Dto.Person, Person>();
            CreateMap<Dto.DocumentType, DocumentType>();
            CreateMap<Dto.Document, DmerCase>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.DmerStatus))
                .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Login))
                .AddTransform(NullStringConverter);

            CreateMap<Dto.Login, Provider>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LoginId))
                .AddTransform(NullStringConverter);

            CreateMap<CaseManagement.Flag, Service.FlagItem>()
                .ForMember(dest => dest.Identifier, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FlagType, opt => opt.MapFrom(src => ConvertFlagType(src.FlagType)))
                .ForMember(dest => dest.FormId, opt => opt.MapFrom(src => src.FormId ?? string.Empty))
                .AddTransform(NullStringConverter);
            CreateMap<CaseManagement.MedicalCondition, Service.MedicalConditionItem>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? "Unknown"))
                .AddTransform(NullStringConverter);

            CreateMap<CaseManagement.MedicalCondition, Service.MedicalConditionItem>();

            CreateMap<IgnitionInterlockDetails, IgnitionInterlock>()
                .ForMember(dest => dest.IiActivity, opt => opt.MapFrom(src => src.IIActivity ?? string.Empty))
                .ForMember(dest => dest.TermMonths, opt => opt.MapFrom(src => src.TermMonths ?? string.Empty))
                .ForMember(dest => dest.InstallDate, opt => opt.MapFrom(src => src.InstallDate != null ? Timestamp.FromDateTimeOffset(src.InstallDate.Value) : null))
                .ForMember(dest => dest.CompletionDate, opt => opt.MapFrom(src => src.CompletionDate != null ? Timestamp.FromDateTimeOffset(src.CompletionDate.Value) : null))
                .ForMember(dest => dest.ClientPaid, opt => opt.MapFrom(src => src.ClientPaid ?? string.Empty));
        }

        // convert null string to empty string (default) for gRPC
        private Expression<Func<string, string>> NullStringConverter = x => x ?? string.Empty;

        FlagTypeOptions ConvertFlagType(FlagTypeOptionSet? value)
        {
            var result = FlagTypeOptions.Unknown;
            switch (value)
            {
                case FlagTypeOptionSet.FollowUp:
                    return FlagTypeOptions.FollowUp;
                case FlagTypeOptionSet.Message:
                    return FlagTypeOptions.Message;
                case FlagTypeOptionSet.Review:
                    return FlagTypeOptions.Review;
                case FlagTypeOptionSet.Submittal:
                    return FlagTypeOptions.Submittal;
            }

            return result;
        }
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
                mc.AddProfile(new FlagAutoMapperProfile());
                mc.AddProfile(new IgnitionInterlockMapperProfile());
                mc.AddProfile(new MedicalConditionAutoMapperProfile());
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}