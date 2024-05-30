using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System.Linq.Expressions;


namespace RSBC.DMF.MedicalPortal.API
{
    public class MappingProfile : Profile
    {
        private readonly ILogger<MappingProfile> _logger;
        private readonly TimeZoneInfo _pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        public MappingProfile()
        {
            CreateMap<Timestamp, DateTimeOffset>()
             .ConvertUsing(src => src.ToDateTimeOffset());

            CreateMap<Rsbc.Dmf.CaseManagement.Service.Document, CaseDocument>()
                .ForMember(dest => dest.DmerType, opt => opt.MapFrom(src => src.DmerType))
                .ForMember(dest => dest.DmerStatus, opt => opt.MapFrom(src => src.DmerStatus))
                // TODO rename to IdCode
                .ForMember(dest => dest.CaseNumber, opt => opt.MapFrom(src => src.Case.CaseNumber))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Case.Person.FullName))
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Case.Person.Birthday))
                .ForMember(dest => dest.ComplianceDate, opt => opt.MapFrom(src => src.ComplianceDate));


            CreateMap<LegacyDocument, ViewModels.CaseDocument>()
             .ForMember(dest => dest.SubmittalStatus, opt => opt.MapFrom(src => src.SubmittalStatus))
             .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => DueDateConverter(src)))
             .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => CreateDateConverter(src)))
             .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType))
             .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        }


        private Expression<Func<string, string>> NullStringConverter = x => x ?? string.Empty;

        private DateTimeOffset ImportDateConverter(LegacyDocument src)
        {
            var importDate = DateTimeOffset.Now;
            try
            {
                if (src.ImportDate != null)
                {
                    importDate = src.ImportDate.ToDateTimeOffset();
                    if (importDate.Offset == TimeSpan.Zero)
                    {
                        importDate = TimeZoneInfo.ConvertTimeFromUtc(importDate.DateTime, _pacificZone);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error parsing import date");
                importDate = DateTimeOffset.Now;
            }

            return importDate;
        }


        //private Expression<Func<string, string>> NullStringConverter = x => x ?? string.Empty;

        private DateTimeOffset DueDateConverter(LegacyDocument src)
        {
            var dueDate = DateTimeOffset.Now;
            try
            {
                if (src.DueDate != null)
                {
                    dueDate = src.DueDate.ToDateTimeOffset();
                    if (dueDate.Offset == TimeSpan.Zero)
                    {
                        dueDate = TimeZoneInfo.ConvertTimeFromUtc(dueDate.DateTime, _pacificZone);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error parsing import date");
                dueDate = DateTimeOffset.Now;
            }

            return dueDate;
        }

        private DateTimeOffset CreateDateConverter(LegacyDocument src)
        {
            var createdOn = DateTimeOffset.Now;
            try
            {
                if (src.CreateDate != null)
                {
                    createdOn = src.CreateDate.ToDateTimeOffset();
                    if (createdOn.Offset == TimeSpan.Zero)
                    {
                        createdOn = TimeZoneInfo.ConvertTimeFromUtc(createdOn.DateTime, _pacificZone);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error parsing import date");
                createdOn = DateTimeOffset.Now;
            }

            return createdOn;
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
