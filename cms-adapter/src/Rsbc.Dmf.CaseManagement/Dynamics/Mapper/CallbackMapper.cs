using AutoMapper;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;

namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public class CallbackMapper
    {
        public class CallbackMapperProfile : Profile
        {
            private enum SeparatedValueIndex
            {
                Phone,
                PreferredTime,
                NotifyByMail,
                NotifyByEmail
            }

            public CallbackMapperProfile()
            {
                CreateMap<task, Callback>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.activityid ?? Guid.NewGuid()))
                    .ForMember(dest => dest.RequestCallback, opt => opt.MapFrom(src => src.scheduledend.GetValueOrDefault()))
                    .ForMember(dest => dest.CallStatus, opt => opt.MapFrom(src => src.statecode))
                    .ForMember(dest => dest.Closed, opt => opt.MapFrom(src => src.actualend.GetValueOrDefault()))
                    .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.subject))
                    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => ParseSeparatedValue(SeparatedValueIndex.Phone, src.description)))
                    .ForMember(dest => dest.PreferredTime, opt => opt.MapFrom(src => ParseSeparatedValue(SeparatedValueIndex.PreferredTime, src.description)))
                    .ForMember(dest => dest.NotifyByMail, opt => opt.MapFrom(src => ParseSeparatedValue(SeparatedValueIndex.NotifyByMail, src.description)))
                    .ForMember(dest => dest.NotifyByEmail, opt => opt.MapFrom(src => ParseSeparatedValue(SeparatedValueIndex.NotifyByEmail, src.description)))
                    .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.dfp_origin))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                    .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.prioritycode));
            }

            // TODO can optimize by caching the split values
            // or only using automapper to parse the description once per row instead of everytime you need any value
            private string ParseSeparatedValue(SeparatedValueIndex indexEnum, string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }

                var values = value.Split(',');
                var index = (int)indexEnum;
                if (values.Length > index)
                {
                    return values[index];
                }

                return string.Empty;
            }
        }
    }
}