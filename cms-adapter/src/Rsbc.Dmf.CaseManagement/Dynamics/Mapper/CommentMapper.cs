using AutoMapper;
using Rsbc.Dmf.CaseManagement.Manager.Comment;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rsbc.Dmf.CaseManagement.Dynamics.Mapper
{
    public class CommentMapper
    {
        public class CommentAutoMapperProfile : Profile
        {
            public CommentAutoMapperProfile()
            {
                CreateMap<dfp_comment, Comment>()
                .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src._dfp_caseid_value))
                .ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.dfp_commentid))
                .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.dfp_commentdetails))
                .ForMember(dest => dest.CommentTypeCode, opt => opt.MapFrom(src => TranslateCommentTypeCodeFromInt(src.dfp_commenttype)))
                .ForMember(dest => dest.CommentDate, opt => opt.MapFrom(src => src.createdon))
                .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.dfp_DriverId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.dfp_userid));

            }

            private string TranslateCommentTypeCodeFromInt(int? commentTypeCode)
            {
                string result;

                switch (commentTypeCode)
                {
                    // W - Web Comments; D - Decision Notes; I - ICBC Comments; C - File Comments; N - Sticky Notes;

                    case 100000003:
                        result = "W";
                        break;
                    case 100000002:
                        result = "D";
                        break;
                    case 100000005:
                        result = "I";
                        break;
                    case 100000001:
                        result = "C";
                        break;
                    case 100000000:
                        result = "N";
                        break;
                    default:
                        result = "C"; // case comment
                        break;
                }
                return result;
            }
        }
    }
}
