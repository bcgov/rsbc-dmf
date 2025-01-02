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
                PreferredTime
            }

            public CallbackMapperProfile()
            {
                CreateMap<task, Callback>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.activityid ?? Guid.NewGuid()))
                    .ForMember(dest => dest.RequestCallback, opt => opt.MapFrom(src => src.scheduledend.GetValueOrDefault()))
                    .ForMember(dest => dest.CallStatus, opt => opt.MapFrom(src => src.statecode))
                    .ForMember(dest => dest.Closed, opt => opt.MapFrom(src => src.actualend.GetValueOrDefault()))
                    .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.subject))
                    //.ForMember(dest => dest.Phone, opt => opt.MapFrom(src => ParseSeparatedValue(SeparatedValueIndex.Phone, src.description, string.Empty)))
                    .ForMember(dest => dest.PreferredTime, opt => opt.MapFrom(src => ParseSeparatedValue(SeparatedValueIndex.PreferredTime, src.description, string.Empty)))
                    .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.dfp_origin))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                    .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.prioritycode));

                CreateMap<Callback, task>()
                    .ForMember(dest => dest.activityid, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.scheduledend, opt => opt.MapFrom(src => src.RequestCallback))
                    .ForMember(dest => dest.statecode, opt => opt.MapFrom(src => src.CallStatus))
                    .ForMember(dest => dest.actualend, opt => opt.MapFrom(src => src.Closed))
                    .ForMember(dest => dest.subject, opt => opt.MapFrom(src => src.Subject))
                    .ForMember(dest => dest.description, opt => opt.MapFrom(src => SerializeValues(src.Phone, src.PreferredTime)))
                    .ForMember(dest => dest.dfp_origin, opt => opt.MapFrom(src => src.Origin))
                    .ForMember(dest => dest.prioritycode, opt => opt.MapFrom(src => src.Priority));
            }

            private const char _delimiter = ',';

            // TODO can optimize by caching the split values
            // or only using automapper to parse the description once per row instead of everytime you need any value
            private string ParseSeparatedValue(SeparatedValueIndex indexEnum, string value, string defaultValue)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return defaultValue;
                }

                var values = value.Split(_delimiter);
                var index = (int)indexEnum;
                if (values.Length > index)
                {
                    return values[index];
                }

                return defaultValue;
            }

            private string SerializeValues(params object[] values)
            {
                return string.Join(_delimiter, values);
            }
        }
    }
}