using AutoMapper;
using Rsbc.Dmf.CaseManagement.Model;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public class IgnitionInterlockMapperProfile : Profile
    {
        public IgnitionInterlockMapperProfile()
        {

            CreateMap<dfp_ignitioninterlock, IgnitionInterlockDetails>()
           .ForMember(dest => dest.IgnitionId, opt => opt.MapFrom(src => src.dfp_ignitioninterlockid))
           .ForMember(dest => dest.IIActivity, opt => opt.MapFrom(src => TranslateIIActivityStatus(src.dfp_iiactivity)))
           .ForMember(dest => dest.TermMonths, opt => opt.MapFrom(src => src.dfp_termmonths.ToString() ?? string.Empty))
           .ForMember(dest => dest.InstallDate, opt => opt.MapFrom(src => src.dfp_installdate))
           .ForMember(dest => dest.CompletionDate, opt => opt.MapFrom(src => src.dfp_completiondate))
           .ForMember(dest => dest.ClientPaid, opt => opt.MapFrom(src => TranslateClientPaidStatus(src.dfp_clientpaid) ?? string.Empty));
        }

        private string TranslateIIActivityStatus(int? iiActivityCode)
        {
            switch (iiActivityCode)
            {
                case 100000000:
                    return "Interlock - Notice Sent";
                case 100000001:
                    return "Interlock - Registered";
                case 100000002:
                    return "Interlock - Term Extended";
                case 100000003:
                    return "Interlock - Report Received";
                case 100000004:
                    return "Interlock - Reconsideration";
                case 100000005:
                    return "Interlock - Complete";
                case 100000006:
                    return "Interlock - Cancelled Non-Comply";
                case 100000007:
                   return "Interlock - Withdrawn";
                case 100000008:
                    return "Interlock - Cancelled";
                case 100000009:
                    return "Interlock - Withdrawn MIA";
                case 100000010:
                    return "Interlock - Withdrawn EOP Voluntary";
                default:
                    return null;
            }
        }

        private string TranslateClientPaidStatus(int? clientPaidStatusCode)
        {
            switch (clientPaidStatusCode)
            {
                case 100000000:
                    return "Yes";
                case 100000001:
                    return "No";
                case 100000002:
                    return "NSF";
                default:
                    return null;
            }
        }
    }
}
