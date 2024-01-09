using AutoMapper;
using Rsbc.Dmf.CaseManagement.Service;

namespace Rsbc.Dmf.DriverPortal.Api
{
    public class MappingProfile : Profile
    {
        private readonly ILogger<MappingProfile> _logger;
        private TimeZoneInfo _pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        public MappingProfile(ILoggerFactory loggingFactory)
        {
            _logger = loggingFactory.CreateLogger<MappingProfile>();

            CreateMap<LegacyDocument, ViewModels.Document>()
                .ForMember(dest => dest.ImportDate, opt => opt.MapFrom(src => ImportDateConverter(src)))
                .ForMember(dest => dest.BcMailSent, opt => opt.MapFrom(src => src.DocumentType == "Letter Out BCMail" && src.ImportDate != null))
                .ForMember(dest => dest.FaxReceivedDate, opt => opt.MapFrom(src => FaxReceivedDateConverter(src)))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate.ToDateTimeOffset()))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate.ToDateTimeOffset()));
        }

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

        private DateTimeOffset FaxReceivedDateConverter(LegacyDocument src)
        {
            var faxReceivedDate = DateTimeOffset.Now;
            try
            {
                if (src.FaxReceivedDate != null)
                {
                    faxReceivedDate = src.FaxReceivedDate.ToDateTimeOffset();

                    if (faxReceivedDate < new DateTimeOffset(1970, 2, 1, 0, 0, 0, TimeSpan.Zero))
                    {
                        faxReceivedDate = DateTimeOffset.Now;
                    }

                    if (faxReceivedDate.Offset == TimeSpan.Zero)
                    {
                        faxReceivedDate = TimeZoneInfo.ConvertTimeFromUtc(faxReceivedDate.DateTime, _pacificZone);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error parsing faxReceivedDate date");
            }

            return faxReceivedDate;
        }
    }

    public static class AutoMapperEx
    {
        public static void AddAutoMapperSingleton(this IServiceCollection services, ILoggerFactory loggerFactory)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile(loggerFactory));
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
