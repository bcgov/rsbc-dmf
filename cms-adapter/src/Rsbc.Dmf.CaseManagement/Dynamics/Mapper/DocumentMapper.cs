using AutoMapper;
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
                    .ForMember(dest => dest.BatchId, opt => opt.MapFrom(src => src.dfp_batchid))
                    .ForMember(dest => dest.DocumentPages, opt => opt.MapFrom(src => ConvertPagesToInt(src.dfp_documentpages)))
                    .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.bcgov_documenturlid.ToString()))
                    .ForMember(dest => dest.DocumentTypeCode, opt => opt.MapFrom(src => src.dfp_DocumentTypeID.dfp_apidocumenttype))
                    .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.dfp_DocumentTypeID.dfp_name))
                    .ForMember(dest => dest.BusinessArea, opt => opt.MapFrom(src => ConvertBusinessAreaToString(src.dfp_DocumentTypeID.dfp_businessarea)))
                    .ForMember(dest => dest.DocumentUrl, opt => opt.MapFrom(src => src.bcgov_url))
                    .ForMember(dest => dest.FaxReceivedDate, opt => opt.MapFrom(src => src.dfp_faxreceiveddate.GetValueOrDefault()))
                    .ForMember(dest => dest.ImportDate, opt => opt.MapFrom(src => src.dfp_dpsprocessingdate.GetValueOrDefault()))
                    .ForMember(dest => dest.ImportId, opt => opt.MapFrom(src => src.dfp_importid))
                    .ForMember(dest => dest.OriginatingNumber, opt => opt.MapFrom(src => src.dfp_faxsender))
                    .ForMember(dest => dest.ValidationMethod, opt => opt.MapFrom(src => src.dfp_validationmethod))
                    .ForMember(dest => dest.ValidationPrevious, opt => opt.MapFrom(src => src.dfp_validationprevious))
                    .ForMember(dest => dest.SubmittalStatus, opt => opt.MapFrom(src => TranslateSubmittalStatusInt(src.dfp_submittalstatus)))
                    .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.dfp_duedate))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.dfp_description))
                    .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.createdon.GetValueOrDefault()))
                    .AddTransform(NullStringConverter);
            }

            private Expression<Func<string, string>> NullStringConverter = x => x ?? string.Empty;
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
                { 100000001, "Received" }, // Received
                { 100000004, "Reject" }, // Rejected
                { 100000009, "Clean Pass"  }, // Clean Pass
                { 100000012, "Manual Pass"  }, // Manual Pass
                { 100000000, "Open-Required"  }, // Open required
                { 100000010, "Uploaded" }, // Uploaded
                { 100000008, "Sent" }
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

        protected string TranslateDocumentOrigin(int documentOrigin)
        {
            var statusMap = new Dictionary<int, string>()
            {
                { 100000014, "Mercury Uploaded RSBC" },
                { 100000015, "Migration" },
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
