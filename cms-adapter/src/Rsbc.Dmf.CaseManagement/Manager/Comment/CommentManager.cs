using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Dynamics;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Rsbc.Dmf.CaseManagement.Manager.Comment;
using System.Linq;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;

namespace Rsbc.Dmf.CaseManagement
{
        internal partial class CommentManager : ICommentManager
        {
            internal readonly DynamicsContext _dynamicsContext;
            private readonly IMapper _mapper;
            private readonly ILogger<CommentManager> _logger;

            public CommentManager(DynamicsContext dynamicsContext, IMapper mapper, ILogger<CommentManager> logger)
            {
                _dynamicsContext = dynamicsContext;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<IEnumerable<Comment>> GetCommentOnDriver(Guid driverId)
            {
                List<Comment> result = new List<Comment>();
            // start by the driver

            var @drivers = _dynamicsContext.dfp_drivers.Where(d => d.dfp_driverid == driverId ).ToList();


            foreach (var driverItem in @drivers)
            {
                if (driverItem != null)
                {
                    await _dynamicsContext.LoadPropertyAsync(driverItem, nameof(dfp_driver.dfp_PersonId));
                    Driver driver = new Driver()
                    {
                        DriverLicenseNumber = driverItem.dfp_licensenumber,
                        Surname = driverItem.dfp_PersonId?.lastname ?? String.Empty,
                        Id = driverItem.dfp_driverid.ToString(),
                    };

                    // get the cases for that driver.
                    var comments = _dynamicsContext.dfp_comments.Where(i => i._dfp_driverid_value == driverItem.dfp_driverid && i.dfp_icbc == true
                    ).OrderByDescending(x => x.dfp_legacyid).OrderByDescending(x => x.createdon).ToList();


                    foreach (var comment in comments)
                    {
                        await _dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.dfp_commentid));
                        await _dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.owninguser));

                        if (comment.dfp_icbc.GetValueOrDefault())
                        {
                            if (comment.statuscode == 1)
                            {
                                int sequenceNumber = 0;
                                int.TryParse(comment.dfp_caseidguid, out sequenceNumber);

                                string caseId = null;
                                Guid? caseGuid = comment._dfp_caseid_value;
                                if (caseGuid != null)
                                {
                                    caseId = caseGuid.ToString();
                                }


                                Comment legacyComment = new Comment
                                {
                                    CaseId = caseId,
                                    CommentDate = comment.createdon.GetValueOrDefault(),
                                    CommentId = comment.dfp_commentid.ToString(),
                                    CommentText = comment.dfp_commentdetails,
                                    CommentTypeCode = TranslateCommentTypeCodeFromInt(comment.dfp_commenttype),
                                    SequenceNumber = sequenceNumber,
                                    UserId = comment.dfp_userid,
                                    Driver = driver,
                                    Origin = TranslateOriginType(comment.dfp_origin),
                                    
                                };

                                if (comment.owninguser != null &&
                                comment.owninguser.dfp_signaturename != null &&
                                !comment.owninguser.dfp_signaturename.Contains("Service Account"))
                                {
                                    legacyComment.SignatureName = comment.owninguser.dfp_signaturename;
                                }

                                result.Add(legacyComment);
                            }

                        }
                    }
                }
            }

            return result;
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

        public string TranslateOriginType(int? originTypeCode)
        {

            string result;
            switch (originTypeCode)
            {
                case 100000000:
                    result = "User";
                    break;
                case 100000001:
                    result = "System";
                    break;
                default:
                    result = "System";
                    break;
            }

            return result;
        }

    }


}
