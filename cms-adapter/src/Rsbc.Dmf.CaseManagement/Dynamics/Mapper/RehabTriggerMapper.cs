using AutoMapper;
using Rsbc.Dmf.CaseManagement.Model;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public class RehabTriggerMapperProfile : Profile
    {
        public RehabTriggerMapperProfile()
        {
            CreateMap<dfp_rehabtrigger, RehabTriggerDetails>()
                .ForMember(dest => dest.RehabId, opt => opt.MapFrom(src => src.dfp_rehabtriggerid))
                .ForMember(dest => dest.AssignmentDate, opt => opt.MapFrom(src => src.dfp_assignmentdate))
                .ForMember(dest => dest.DecisionDate, opt => opt.MapFrom(src => src.dfp_decisiondate))
                .ForMember(dest => dest.ClientType, opt => opt.MapFrom(src => TranslateClientType(src.dfp_clienttype)))
                .ForMember(dest => dest.RehabActivity, opt => opt.MapFrom(src => TranslateRehabActivity(src.dfp_rehabactivity)))
                .ForMember(dest => dest.ClientPaid, opt => opt.MapFrom(src => TranslateClientPaidStatus(src.dfp_clientpaid)))
                .ForMember(dest => dest.Stream, opt => opt.MapFrom(src => TranslateStream(src.dfp_stream)))
                .ForMember(dest => dest.Decision, opt => opt.MapFrom(src => TranslateDecision(src.dfp_decision)));
        }

        private string TranslateClientType(int? clientTypeCode)
        {
            switch (clientTypeCode)
            {
                case 100000000:
                    return "License - Allowed";
                case 100000001:
                    return "License - Not allowed";
                default:
                    return null;
            }
        }

        private string TranslateRehabActivity(int? rehabActivityCode)
        {
            switch (rehabActivityCode)
            {
                case 100000000:
                    return "Registration Received";
                case 100000001:
                    return "Reconsideration Received";
                case 100000002:
                    return "Counselling Result Received";
                case 100000003:
                    return "High Risk Results Received";
                case 100000004:
                    return "Initial Assessment Received";
                case 100000005:
                    return "Notice Sent";
                case 100000006:
                    return "Registered";
                case 100000007:
                    return "Educ Results Received";
                case 100000008:
                    return "PIA Results Received";
                case 100000009:
                    return "Priority Result Received";
                case 100000010:
                    return "Complete";
                case 100000011:
                    return "Incomplete";
                case 100000012:
                    return "Non Comply";
                case 100000013:
                    return "IA reviewed IIP Required";
                case 100000014:
                    return "IA reviewed IIP  not Required";
                case 100000015:
                    return "Initial Assessment Received";
                case 100000016:
                    return "OOP CERT";
                case 100000017:
                    return "OOP Result Received";
                case 100000018:
                    return "Cancelled";
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

        private string TranslateStream(int? streamCode)
        {
            switch (streamCode)
            {
                case 100000000:
                    return "Education";
                case 100000001:
                    return "Counselling";
                case 100000002:
                    return "Treatment";
                default:
                    return null;
            }
        }

        private string TranslateDecision(int? decisionCode)
        {
            switch (decisionCode)
            {
                case 100000000:
                    return "Fit";
                case 100000001:
                    return "Fit with Interlock";
                case 100000002:
                    return "Unfit";
                case 100000003:
                    return "Cancelled";
                default:
                    return null;
            }
        }
    }
}