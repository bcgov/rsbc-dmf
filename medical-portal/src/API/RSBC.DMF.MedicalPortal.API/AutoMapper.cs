using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using PidpAdapter;
using Pssg.SharedUtils;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System;
using EnumsNET;

namespace RSBC.DMF.MedicalPortal.API
{
    public class MappingProfile : Profile
    {
        private readonly ILogger<MappingProfile> _logger;
        public MappingProfile()
        {
            //#TODO Move this to shared folder
            CreateMap<Timestamp, DateTimeOffset>()
             .ConvertUsing(src => src.ToDateTimeOffset());

            CreateMap<Document, CaseDocument>()
                .ForMember(dest => dest.DmerType, opt => opt.MapFrom(src => src.DmerType))
                .ForMember(dest => dest.DmerStatus, opt => opt.MapFrom(src => src.DmerStatus))
                // TODO rename to IdCode
                .ForMember(dest => dest.CaseNumber, opt => opt.MapFrom(src => src.Case.CaseNumber))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Case.Person.FullName))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Case.Person.Birthday))
                .ForMember(dest => dest.ComplianceDate, opt => opt.MapFrom(src => src.ComplianceDate));

            CreateMap<LegacyDocument, ViewModels.CaseDocument>()
             .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
             .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreateDate))
             .ForMember(dest => dest.SubmittalStatus, opt => opt.MapFrom(src => GroupSubmittalStatus(src.SubmittalStatus)));

            CreateMap<DocumentSubType, ViewModels.DocumentSubTypes>();

            CreateMap<EndorsementDto, Endorsement>();
            CreateMap<PidpAdapter.Licence, ViewModels.Licence>();
        }

        private string GroupSubmittalStatus(string submittalStatus)
        {
            var canParseEnum = Enums.TryParse<SubmittalStatus>(submittalStatus, true, out var submittalStatusEnum, EnumFormat.Description);
            if (!canParseEnum)
                return submittalStatus;

            switch (submittalStatusEnum)
            {
                case SubmittalStatus.OpenRequired:
                    return SubmittalStatus.OpenRequired.AsString(EnumFormat.Description);
                case SubmittalStatus.Noncomply:
                case SubmittalStatus.ActionedNoncomply:
                    return SubmittalStatus.Noncomply.AsString(EnumFormat.Description);
                case SubmittalStatus.Received:
                case SubmittalStatus.CleanPass:
                case SubmittalStatus.ManualPass:
                case SubmittalStatus.Reviewed:
                case SubmittalStatus.UnderReview:
                    return SubmittalStatus.Received.AsString(EnumFormat.Description);
                case SubmittalStatus.Rejected:
                    return SubmittalStatus.Rejected.AsString(EnumFormat.Description);
                case SubmittalStatus.Uploaded:
                    return SubmittalStatus.Uploaded.AsString(EnumFormat.Description);
                default:
                    _logger.LogError($"Error parsing SubmittalStatus: {submittalStatus}");
                    return submittalStatus;
            }
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
