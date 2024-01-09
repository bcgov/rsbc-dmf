using AutoMapper;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Linq.Expressions;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<bcgov_documenturl, LegacyDocument>()
                .ForMember(dest => dest.BatchId, opt => opt.MapFrom(src => src.dfp_batchid))
                .ForMember(dest => dest.DocumentPages, opt => opt.MapFrom(src => CaseManager.ConvertPagesToInt(src.dfp_documentpages)))
                .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.bcgov_documenturlid.ToString()))
                .ForMember(dest => dest.DocumentTypeCode, opt => opt.MapFrom(src => src.dfp_DocumentTypeID.dfp_apidocumenttype))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.dfp_DocumentTypeID.dfp_name))
                .ForMember(dest => dest.BusinessArea, opt => opt.MapFrom(src => CaseManager.ConvertBusinessAreaToString(src.dfp_DocumentTypeID.dfp_businessarea)))
                .ForMember(dest => dest.DocumentUrl, opt => opt.MapFrom(src => src.bcgov_url))
                .ForMember(dest => dest.FaxReceivedDate, opt => opt.MapFrom(src => src.dfp_faxreceiveddate.GetValueOrDefault()))
                .ForMember(dest => dest.ImportDate, opt => opt.MapFrom(src => src.dfp_dpsprocessingdate.GetValueOrDefault()))
                .ForMember(dest => dest.ImportId, opt => opt.MapFrom(src => src.dfp_importid))
                .ForMember(dest => dest.OriginatingNumber, opt => opt.MapFrom(src => src.dfp_faxsender))
                .ForMember(dest => dest.ValidationMethod, opt => opt.MapFrom(src => src.dfp_validationmethod))
                .ForMember(dest => dest.ValidationPrevious, opt => opt.MapFrom(src => src.dfp_validationprevious))
                .ForMember(dest => dest.SubmittalStatus, opt => opt.MapFrom(src => CaseManager.TranslateSubmittalStatusInt(src.dfp_submittalstatus)))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.dfp_duedate))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.dfp_description))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.createdon.GetValueOrDefault()))
                .AddTransform(NullStringConverter);
        }

        private Expression<Func<string, string>> NullStringConverter = x => x ?? string.Empty;
    }
}
