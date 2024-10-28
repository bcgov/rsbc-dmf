using AutoMapper;
using Rsbc.Dmf.CaseManagement.Dto;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public class DocumentMapper
    {
        public class DocumentAutoMapperProfile : Profile
        {
            public DocumentAutoMapperProfile()
            {
                CreateMap<bcgov_documenturl, LegacyDocument>()
                    .ForMember(dest => dest.IdCode, opt => opt.MapFrom(src => src.bcgov_CaseId.ticketnumber))
                    .ForMember(dest => dest.BatchId, opt => opt.MapFrom(src => src.dfp_batchid))
                    .ForMember(dest => dest.DocumentPages, opt => opt.MapFrom(src => ConvertPagesToInt(src.dfp_documentpages)))
                    .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.bcgov_documenturlid.ToString()))
                    .ForMember(dest => dest.DocumentTypeCode, opt => opt.MapFrom(src => src.dfp_DocumentTypeID.dfp_apidocumenttype))
                    .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.dfp_DocumentTypeID.dfp_name))
                    .ForMember(dest => dest.DocumentSubType, opt => opt.MapFrom(src => src.dfp_DocumentSubType.dfp_name))
                    .AfterMap((src, dest) => dest.CaseType = dest.DocumentType == "DMER" ? "Solicited" : "Unsolicited")
                    .ForMember(dest => dest.BusinessArea, opt => opt.MapFrom(src => ConvertBusinessAreaToString(src.dfp_DocumentTypeID.dfp_businessarea)))
                    .ForMember(dest => dest.DocumentUrl, opt => opt.MapFrom(src => src.bcgov_url))
                    .ForMember(dest => dest.FaxReceivedDate, opt => opt.MapFrom(src => src.dfp_faxreceiveddate.GetValueOrDefault()))
                    .ForMember(dest => dest.ImportDate, opt => opt.MapFrom(src => src.dfp_dpsprocessingdate.GetValueOrDefault()))
                    .ForMember(dest => dest.ImportId, opt => opt.MapFrom(src => src.dfp_importid))
                    .ForMember(dest => dest.OriginatingNumber, opt => opt.MapFrom(src => src.dfp_faxsender))
                    .ForMember(dest => dest.ValidationMethod, opt => opt.MapFrom(src => src.dfp_validationmethod))
                    .ForMember(dest => dest.ValidationPrevious, opt => opt.MapFrom(src => src.dfp_validationprevious))
                    .ForMember(dest => dest.SubmittalStatus, opt => opt.MapFrom(src => TranslateSubmittalStatusInt(src.dfp_submittalstatus)))
                    .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.dfp_compliancedate))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.dfp_DocumentTypeID.dfp_description))
                    .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.createdon.GetValueOrDefault()))
                    .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.dfp_documentorigin))
                    .ForMember(dest => dest.showOnPortals, opt => opt.MapFrom(src => src.dfp_showonportals))
                    .AddTransform(NullStringConverter);

                CreateMap<bcgov_documenturl, Document>()
                    .ForMember(dest => dest.DmerType, opt => opt.MapFrom(src => TranslateDmerType(src.dfp_dmertype)))
                    .ForMember(dest => dest.DmerStatus, opt => opt.MapFrom(src => TranslateSubmittalStatus(src.dfp_submittalstatus)))
                    .ForMember(dest => dest.Case, opt => opt.MapFrom(src => src.bcgov_CaseId))
                    .ForMember(dest => dest.ComplianceDate, opt => opt.MapFrom(src => src.dfp_compliancedate))
                    .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.dfp_DocumentTypeID))
                    .ForMember(dest => dest.DocumentSubType, opt => opt.MapFrom(src => src.dfp_DocumentSubType))
                    .ForMember(dest => dest.SubmittalStatus, opt => opt.MapFrom(src => TranslateSubmittalStatusInt(src.dfp_submittalstatus)))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.dfp_DocumentTypeID.dfp_description))
                    .ForMember(dest => dest.DocumentUrl, opt => opt.MapFrom(src => src.bcgov_url))
                    .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.createdon.GetValueOrDefault()))
                    .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.bcgov_documenturlid.ToString()))
                    .ForMember(dest => dest.IdCode, opt => opt.MapFrom(src => src.bcgov_CaseId.ticketnumber))
                    .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.dfp_LoginId))
                    .ForMember(dest => dest.FaxReceivedDate, opt => opt.MapFrom(src => src.dfp_faxreceiveddate.GetValueOrDefault()))
                    .ForMember(dest => dest.showOnPortals, opt => opt.MapFrom(src => src.dfp_showonportals))
                    .AddTransform(NullStringConverter);
                    
            }

            private Expression<Func<string, string>> NullStringConverter = x => x ?? string.Empty;

            private string TranslateDmerType(int? optionSetValue)
            {
                switch (optionSetValue)
                {
                    case 100000000:
                        return "1 - NSC";
                    case 100000001:
                        return "2 - Age";
                    case 100000002:
                        return "3 - Industrial Road";
                    case 100000003:
                        return "4 - Known Condition";
                    case 100000004:
                        return "5 - Possible Condition";
                    default:
                        return null;
                }
            }

            private string TranslateDmerStatus(int? dmerStatus)
            {
                switch (dmerStatus)
                {
                    case 100000000:
                        return "Adjudicate";
                    case 100000001:
                        return "Reject";
                    case 100000002:
                        return "Clean Pass";
                    case 100000003:
                        return "Manual Pass";
                    default:
                        return null;
                }
            }

            private string TranslateSubmittalStatus(int? submittalStatusCode)
            {
                switch (submittalStatusCode)
                {
                    case 100000000:
                        return "Open-Required";
                    case 100000005:
                        return "Non-Comply";
                    case 100000007:
                        return "Actioned Non-comply";
                    default:
                        return "Submitted";
                }
            }
        }

        /// <summary>
        /// Convert Business Area To String
        /// </summary>
        /// <param name="businessArea"></param>
        /// <returns></returns>
        protected static string ConvertBusinessAreaToString(int? businessArea)
        {
            string result = "";

            if (businessArea != null)
            {
                switch (businessArea)
                {
                    case 100000000:
                        result = "Driver Fitness";
                        break;
                    case 100000001:
                        result = "Remedial";
                        break;
                    case 100000002:
                        result = "Client Services";
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Translate the Dynamics Priority (status reason) field to text
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected static string TranslateSubmittalStatusInt(int? submittalStatusCode)
        {
            var statusMap = new Dictionary<int, string>()
            {
                // TODO update to use shared-utils SubmittalStatus.cs
                { 100000000, "Open-Required"  },
                { 100000001, "Received" }, // Accept
                { 100000002, "Received" }, // 'Under Review' in Dynamics
                { 100000003, "Reviewed" },
                { 100000004, "Rejected" }, // 'Rejected' in Dynamics
                { 100000005, "Non-Comply" },
                { 100000007, "Non-Comply" }, // 'Actioned Non-comply' in Dynamics
                { 100000008, "Sent" },
                { 100000009, "Clean Pass"  },
                { 100000010, "Uploaded" },
                { 100000011, "Issued" },
                { 100000012, "Manual Pass"  },
                // Empty 13, Carry Forward 7
            };

            if (submittalStatusCode != null && statusMap.ContainsKey(submittalStatusCode.Value))
            {
                return statusMap[submittalStatusCode.Value];
            }
            else
            {
                return "Received";
            }
        }

        // NOTE keep in sync with CaseManager.TranslateDocumentOrigin
        protected string TranslateDocumentOrigin(int documentOrigin)
        {
            var statusMap = new Dictionary<int, string>()
            {
                { 100000000, "Practitioner Portal" },
                { 100000001, "Partner Portal" },
                { 100000014, "Mercury Uploaded RSBC" },
                { 100000015, "Migration" },
                { 100000016, "Driver Portal" },
                { 100000017, "DPS/KOFAX" },
            };

            if (statusMap.ContainsKey(documentOrigin))
            {
                return statusMap[documentOrigin];
            }
            else
            {
                return statusMap[100000014];
            }
        }


        /// <summary>
        /// Translate the Dynamics Priority (status reason) field to text
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected string TranslatePriorityCode(int priorityCode)
        {
            var statusMap = new Dictionary<int, string>()
            {
                { 100000000,"Regular" },
                { 100000001,"Urgent / Immediate" },
                { 100000002, "Expedited" },
                { 100000003, "Critical Review" },
            };

            if (priorityCode != null && statusMap.ContainsKey(priorityCode))
            {
                return statusMap[priorityCode];
            }
            else
            {
                return statusMap[100000000];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priorityCode"></param>
        /// <returns></returns>
        protected string TranslateQueueCodeInt(int queueCode)
        {
            var statusMap = new Dictionary<int, string>()
            {
                { 100000002, "Nurse Case Managers" },
                { 100000001, "Adjudicators" },
                { 100000000, "Client Services" },
            };

            if (queueCode != null && statusMap.ContainsKey(queueCode))
            {
                return statusMap[queueCode];
            }
            else
            {
                return statusMap[100000000];
            }
        }

        /// <summary>
        /// Convert Pages To Int
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected static int ConvertPagesToInt(string data)
        {
            int result = 0;
            if (!int.TryParse(data, out result))
            {
                result = 0;
            }
            return result;
        }
    }
}
