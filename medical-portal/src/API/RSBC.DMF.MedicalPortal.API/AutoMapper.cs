using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using PidpAdapter;
using Pssg.SharedUtils;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.ViewModels;

namespace RSBC.DMF.MedicalPortal.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // TODO Move this to shared folder
            CreateMap<Timestamp, DateTimeOffset>()
                .ConvertUsing(src => src.ToDateTimeOffset());

            CreateMap<Rsbc.Dmf.CaseManagement.Service.Document, ViewModels.CaseDocument>()
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
                .ForMember(dest => dest.SubmittalStatus,
                    opt => opt.MapFrom(src => GroupSubmittalStatusUtil.GroupSubmittalStatus(src.SubmittalStatus)));

            CreateMap<DocumentSubType, ViewModels.DocumentSubTypes>();

            CreateMap<DmerCase, ViewModels.DmerDocument>()
                .ForMember(dest => dest.DmerStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.LoginId, opt => opt.MapFrom(src => src.Provider.Id));

            CreateMap<Document, ViewModels.DmerDocument>()
                .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.LoginId, opt => opt.MapFrom(src => src.Provider.Id))
                .ForMember(dest => dest.IdCode, opt => opt.MapFrom(src => src.Case.CaseNumber))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Case.Person.FullName))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.Case.Person.Birthday))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.ComplianceDate))
                .ForMember(dest => dest.DmerStatus, opt => opt.MapFrom(src => src.DmerStatus))
                .ForMember(dest => dest.DmerType, opt => opt.MapFrom(src => src.Case.DmerType));

            CreateMap<EndorsementDto, Endorsement>();
            CreateMap<PidpAdapter.Licence, ViewModels.Licence>();

            CreateMap<MedicalConditionItem, MedicalCondition>();
            CreateMap<FlagItem, ViewModels.Flag>();
            CreateMap<ViewModels.Flag, FlagItem>();
        }
    }

    public static class AutoMapperExtensions
    {
        public static void AddAutoMapperSingleton(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}