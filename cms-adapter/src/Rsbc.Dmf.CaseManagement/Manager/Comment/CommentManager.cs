using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Dynamics;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Rsbc.Dmf.CaseManagement.Manager.Comment;
using System.Linq;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using Microsoft.OData.Client;

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


        /// <summary>
        /// Create Legacy Case Comment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateStatusReply> AddCaseComment(Comment request)
        {
            CreateStatusReply result = new CreateStatusReply()
            {
                Success = false,
                ErrorDetail = "unknown error - AddCaseComment"
            };

            dfp_comment comment = null;

            var driver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();
     
            if (string.IsNullOrEmpty(request.CommentId)) // create
            {
                // create the comment
                comment = new dfp_comment()
                {
                    createdon = request.CommentDate,
                    dfp_commenttype = TranslateCommentTypeCodeToInt(request.CommentTypeCode),
                    dfp_icbc = request.CommentTypeCode == "W" || request.CommentTypeCode == "I",
                    dfp_userid = request.UserId,
                    dfp_commentdetails = request.CommentText,
                    dfp_date = request.CommentDate,
                    statecode = 0,
                    statuscode = 1,
                    overriddencreatedon = request.CommentDate

                };
                int sequenceNumber = 0;
                if (request.SequenceNumber != null)
                {
                    sequenceNumber = request.SequenceNumber.Value;
                }

                comment.dfp_caseidguid = sequenceNumber.ToString();

                try
                {
                    await _dynamicsContext.SaveChangesAsync();
                    _dynamicsContext.AddTodfp_comments(comment);
                    var saveResult = await _dynamicsContext.SaveChangesAsync();
                    var tempId = GetCreatedId(saveResult);
                    if (tempId != null)
                    {
                        comment = _dynamicsContext.dfp_comments.ByKey(tempId).GetValue();
                    }
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "CreateLegacyCaseComment Error adding comment");
                    result.Success = false;
                    result.ErrorDetail = "CreateLegacyCaseComment Error adding comment" + ex.Message;

                }

                if (result.Success == true)
                {
                    try
                    {
                        _dynamicsContext.SetLink(comment, nameof(dfp_comment.dfp_DriverId), driver);

                        await _dynamicsContext.SaveChangesAsync();
                        result.Success = true;
                        result.Id = comment.dfp_commentid.ToString();
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Error(ex, "CreateLegacyCaseComment Set Links Error");
                        result.Success = false;
                        result.ErrorDetail = "CreateLegacyCaseComment Set Links Error" + ex.Message;
                    }
                }
            }

            else // update
            {
                try
                {
                    Guid key = Guid.Parse(request.CommentId);
                    comment = _dynamicsContext.dfp_comments.ByKey(key).GetValue();
                    comment.dfp_commenttype = TranslateCommentTypeCodeToInt(request.CommentTypeCode);
                    comment.dfp_icbc = request.CommentTypeCode == "W" || request.CommentTypeCode == "I";
                    comment.dfp_userid = request.UserId;
                    comment.dfp_commentdetails = request.CommentText;
                    comment.dfp_date = request.CommentDate;
                    comment.overriddencreatedon = request.CommentDate;

                    _dynamicsContext.UpdateObject(comment);
                    _dynamicsContext.SaveChanges();

                    result.Success = true;
                    result.Id = comment.dfp_commentid.ToString();

                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "CreateLegacyCaseComment Update Comment Error");
                    result.Success = false;
                    result.ErrorDetail = "CreateLegacyCaseComment Update Comment Error " + ex.Message;
                }
            }

            if (!string.IsNullOrEmpty(request.CaseId))
            {
                Guid caseId;
                if (Guid.TryParse(request.CaseId, out caseId))
                {
                    if (caseId != Guid.Empty)
                    {
                        try
                        {
                            incident driverCase = _dynamicsContext.incidents.ByKey(Guid.Parse(request.CaseId))
                                .GetValue();
                            _dynamicsContext.AddLink(driverCase, nameof(incident.dfp_incident_dfp_comment), comment);
                            _dynamicsContext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, "Unable to link comment to case");
                        }
                    }
                }
            }

            _dynamicsContext.Detach(comment);
            return result;
        }


        /// <summary>
        /// Translate CommentTypeCode To Int
        /// </summary>
        /// <param name="commentTypeCode"></param>
        /// <returns></returns>
        private int TranslateCommentTypeCodeToInt(string commentTypeCode)
        {
            int result;

            switch (commentTypeCode)
            {
                // W - Web Comments; D - Decision Notes; I - ICBC Comments; C - File Comments; N - Sticky Notes;

                case "W":
                    result = 100000003;
                    break;
                case "D":
                    result = 100000002;
                    break;
                case "I":
                    result = 100000005;
                    break;
                case "C":
                    result = 100000001;
                    break;
                case "N":
                    result = 100000000;
                    break;
                default:
                    result = 100000001;
                    break;
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

        Guid? GetCreatedId(DataServiceResponse saveResult)
        {
            Guid? result = null;
            try
            {
                string returnId = null;

                if (saveResult.Count() > 0)
                {
                    var tempId = saveResult.First().Headers["OData-EntityId"];

                    int bracketLeft = tempId.IndexOf("(");
                    int bracketRight = tempId.IndexOf(")");
                    if (bracketLeft != -1 && bracketRight != -1)
                    {
                        returnId = tempId.Substring(bracketLeft + 1, bracketRight - bracketLeft - 1);
                        result = Guid.Parse(returnId);
                    }
                }

            }
            catch (Exception)
            { }


            return result;
        }

        public IEnumerable<dfp_driver> GetDriverObjects(string licensenumber)
        {
            return _dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d => d.statuscode == 1 && d.dfp_licensenumber == licensenumber).ToList();
        }


    }


}
