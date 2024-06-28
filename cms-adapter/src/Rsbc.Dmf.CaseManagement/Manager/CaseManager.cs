using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    #region Model

    public class CaseSearchRequest
    {
        public string CaseId { get; set; }
        public string Title { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string ClinicId { get; set; }
    }

    public class LegacyCandidateSearchRequest
    {
        public string DriverLicenseNumber { get; set; }
        public string Surname { get; set; }

        public int? SequenceNumber { get; set; }
    }

    public class LegacyComment
    {
        public int? SequenceNumber { get; set; }
        public string CommentTypeCode { get; set; }
        public string CommentText { get; set; }
        public string UserId { get; set; }
        public string CaseId { get; set; }
        public DateTimeOffset CommentDate { get; set; }
        public string CommentId { get; set; }
        public Driver Driver { get; set; }
        public string Assignee { get; set; }
        public string SignatureName { get; set; }
    }


    public class LegacyDocument
    {
        public int? SequenceNumber { get; set; }
        public string IdCode { get; set; }
        public string DocumentTypeCode { get; set; }
        public string DocumentType { get; set; }
        public string CaseType { get; set; }
        public string BusinessArea { get; set; }

        public string DocumentUrl { get; set; }
        public long FileSize { get; set; }
        public string UserId { get; set; }
        public string CaseId { get; set; }
        public DateTimeOffset? FaxReceivedDate { get; set; }
        public DateTimeOffset? ImportDate { get; set; }
        public string DocumentId { get; set; }
        public string ImportId { get; set; }
        public Driver Driver { get; set; }

        public string BatchId { get; set; }
        public string OriginatingNumber { get; set; }
        public int DocumentPages { get; set; }
        public string ValidationMethod { get; set; }
        public string ValidationPrevious { get; set; }
        public string Priority { get; set; }
        public string Owner { get; set; }
        public string SubmittalStatus { get; set; }
        public string FilenameOverride { get; set; }
        public string Queue { get; set; }
        public long DpsDocumentId { get; set; }
        public string Origin { get; set; }
        public bool Solicited { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public string Description { get; set; }
        public string DocumentSubType { get; set; }
        public string DocumentSubTypeId { get; set; }
    }

    public class CreateStatusReply
    {
        public string Id;
        public bool Success { get; set; }
        public string ErrorDetail { get; set; }
    }

    public class CaseSearchReply
    {
        public IEnumerable<Case> Items { get; set; }
    }

    public class SetCaseFlagsReply
    {
        public bool Success { get; set; }
    }

    [Obsolete("Use UserManager.cs FullAddress instead")]
    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postal { get; set; }
    }

    public abstract class Case
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsCommercial { get; set; }
        public string Status { get; set; }
        public bool CleanPass { get; set; }
    }

    public class Driver
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }

        public string Middlename { get; set; }

        public double Weight { get; set; }
        public string Sex { get; set; }
        public DateTime BirthDate { get; set; }
        public double Height { get; set; }
        public Address Address { get; set; }
        public string DriverLicenseNumber { get; set; }
    }

    public class Provider
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string ProviderDisplayId { get; set; }
        public string ProviderDisplayIdType { get; set; }
        public string ProviderRole { get; set; }
        public string ProviderSpecialty { get; set; }
        public string PhoneUseType { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneExtension { get; set; }
        public string FaxUseType { get; set; }
        public string FaxNumber { get; set; }
        public Address Address { get; set; }
    }

    public class DmerCase : Case
    {
        public Driver Driver { get; set; }
        public Provider Provider { get; set; }
        public IEnumerable<Flag> Flags { get; set; }

        public IEnumerable<Decision> Decisions { get; set; }

        public string ClinicId { get; set; }

        public string ClinicName { get; set; }

        public string DmerType { get; set; }

        public int CaseSequence { get; set; }

        public IEnumerable<MedicalCondition> MedicalConditions { get; set; }
    }

    public class Flag
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public FlagTypeOptionSet? FlagType { get; set; }
        public string FormId { get; set; }
    }

    public class MedicalCondition
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string FormId { get; set; }
    }

    public enum DecisionOutcome
    {
        FitToDrive = 1,
        NonComply = 2,
        UnfitToDrive = 3
    }


    public class Decision
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DecisionOutcome? Outcome { get; set; }
    }


    public class IcbcErrorRequest
    {
        public string CaseId { get; set; }
        public string ErrorMessage { get; set; }
    }

    [Obsolete("use shared-utils SubmittalStatus.cs")]
    public enum submittalStatusOptionSet
    {
        Accept = 100000001,
        Reject = 100000004,
        CleanPass = 100000009,
        ManualPass = 100000012,
        Empty = 100000013,
        OpenRequired = 100000000,
        Reviewed = 100000003,
        Uploaded = 100000010
    }

    // TODO move to CallbackModel.cs
    public enum CallbackPriority
    {
        Low = 0,
        Normal = 1,
        High = 2
    }

    public class ResultStatusReply
    {
        public string Id;

        public bool Success { get; set; }

        public string ErrorDetail { get; set; }
    }

    public class UpdateDriverRequest
    {
        public string DriverLicenseNumber { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class CreateCaseRequest
    {
        public string CaseId { get; set; }

        public int? SequenceNumber { get; set; }

        public string DriverLicenseNumber { get; set; }

        public string DocumentType { get; set; }

        public string CaseTypeCode { get; set; }
    }


    public class CreateDecisionRequest
    {
        public string CaseId { get; set; }
        public string DriverId { get; set; }
        public string OutcomeText { get; set; }
        public string SubOutcomeText { get; set; }
        public DateTimeOffset StatusDate { get; set; }
    }

    public class CreateDriverRequest
    {
        public string DriverLicenseNumber { get; set; }

        public string Surname { get; set; }

        public string GivenName { get; set; }

        public DateTimeOffset? BirthDate { get; set; }

        public int? SequenceNumber { get; set; }
    }

    public enum StatusCodeOptionSet
    {
        SendToBCMail = 100000002,
        Sent = 100000005,
        FailedToSend = 100000006
    }


    public class PdfDocument
    {
        public string PdfDocumentId { get; set; }
        public StatusCodeOptionSet StatusCode { get; set; }
        public int? StateCode { get; set; }
        public string Filename { get; set; }
        public string ServerUrl { get; set; }
    }

    public class PdfDocumentReply
    {
        public PdfDocument Document { get; set; }
        public bool Success { get; set; }
    }

    #endregion Model

    internal partial class CaseManager : ICaseManager
    {
        internal readonly DynamicsContext dynamicsContext;
        private readonly ILogger<CaseManager> logger;
        private readonly IMapper _mapper;
        private readonly IMapperAsync<incident, CaseDetail> _caseMapper;
        private readonly Dictionary<string, Guid> OutcomeStatusTypes;
        private readonly Dictionary<Guid, Dictionary<string, Guid>> OutcomeSubStatusTypes;

        public CaseManager(DynamicsContext dynamicsContext, ILogger<CaseManager> logger, IMapper mapper,
            IMapperAsync<incident, CaseDetail> caseMapper)
        {
            this.dynamicsContext = dynamicsContext;
            this.logger = logger;
            _mapper = mapper;
            _caseMapper = caseMapper;
            OutcomeStatusTypes = GetOutcomeStatusTypes();
            OutcomeSubStatusTypes = GetOutcomeSubStatusTypes();
        }

        private Dictionary<string, Guid> GetOutcomeStatusTypes()
        {
            Dictionary<string, Guid> result = new Dictionary<string, Guid>();

            var outcomestatuses = dynamicsContext.dfp_outcomestatuses.Where(x => x.statecode == 0).ToList();

            foreach (var item in outcomestatuses)
            {
                string code = item.dfp_name;
                Guid id = item.dfp_outcomestatusid.Value;

                if (code != null && !result.ContainsKey(code))
                {
                    result.Add(code, id);
                }

                dynamicsContext.Detach(item);
            }

            return result;
        }

        private Dictionary<Guid, Dictionary<string, Guid>> GetOutcomeSubStatusTypes()
        {
            Dictionary<Guid, Dictionary<string, Guid>> result = new Dictionary<Guid, Dictionary<string, Guid>>();

            var outcomesubstatuses = dynamicsContext.dfp_outcomesubstatuses.Where(x => x.statecode == 0).ToList();

            foreach (var item in outcomesubstatuses)
            {
                string code = item.dfp_name;
                Guid id = item.dfp_outcomesubstatusid.Value;
                if (item._dfp_outcomestatusid_value != null)
                {
                    Guid parent = item._dfp_outcomestatusid_value.Value;

                    if (!result.ContainsKey(parent))
                    {
                        result.Add(parent, new Dictionary<string, Guid>());
                    }

                    var subresult = result[parent];
                    if (code != null && !subresult.ContainsKey(code))
                    {
                        subresult.Add(code, id);
                    }
                }

                dynamicsContext.Detach(item);
            }

            return result;
        }

        /// <summary>
        /// Lazy Load Properties
        /// </summary>
        /// <param name="case"></param>
        /// <returns></returns>
        private async Task LazyLoadProperties(incident @case)
        {
            //load clinic details (assuming customer as clinic for now)
            if (@case.customerid_contact == null)
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.customerid_contact));

            if (@case._dfp_driverid_value.HasValue)
            {
                //load driver info
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_DriverId));
                if (@case.dfp_DriverId != null)
                    await dynamicsContext.LoadPropertyAsync(@case.dfp_DriverId,
                        nameof(incident.dfp_DriverId.dfp_PersonId));
            }

            if (@case._dfp_medicalpractitionerid_value.HasValue)
            {
                //load driver info
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_MedicalPractitionerId));
                if (@case.dfp_MedicalPractitionerId != null)
                    await dynamicsContext.LoadPropertyAsync(@case.dfp_MedicalPractitionerId,
                        nameof(incident.dfp_MedicalPractitionerId.dfp_PersonId));
            }

            //load case's flags
            await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_dmerflag));
            foreach (var flag in @case.dfp_incident_dfp_dmerflag)
            {
                await dynamicsContext.LoadPropertyAsync(flag, nameof(dfp_dmerflag.dfp_FlagId));
            }

            //load case's medical conditions (KMCs)
            await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_knownmedicalcondition));
            foreach (var medicalcondition in @case.dfp_incident_dfp_knownmedicalcondition)
            {
                await dynamicsContext.LoadPropertyAsync(medicalcondition,
                    nameof(dfp_knownmedicalcondition.dfp_MedicalConditionId));
            }

            //load decisions
            await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_decision));
            foreach (var decision in @case.dfp_incident_dfp_decision)
            {
                await dynamicsContext.LoadPropertyAsync(decision, nameof(dfp_decision.dfp_decisionid));
                if (decision.dfp_OutcomeStatus != null)
                    await dynamicsContext.LoadPropertyAsync(decision.dfp_OutcomeStatus,
                        nameof(dfp_decision.dfp_OutcomeStatus));
            }

            // load owner
            //if (@case._ownerid_value.HasValue)
            //{
            //    //load driver info
            //    await dynamicsContext.LoadPro pertyAsync(@case, nameof(incident.ownerid));

            //}
        }

        public async Task<CaseSearchReply> CaseSearch(CaseSearchRequest request)
        {
            //search matching cases
            var cases = (await SearchCases(dynamicsContext, request)).Concat(
                await SearchDriverCases(dynamicsContext, request));

            //lazy load case related properties
            foreach (var @case in cases)
            {
                await LazyLoadProperties(@case);
            }

            dynamicsContext.DetachAll();

            //map cases from query results (TODO: consider replacing with AutoMapper)
            return MapCases(cases);
        }

        /// <summary>
        /// Get Case Legacy Comments
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="allComments"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyComment>> GetCaseLegacyComments(string caseId, bool allComments,
            OriginRestrictions originRestrictions)
        {
            List<LegacyComment> result = new List<LegacyComment>();
            // start by the driver


            var @cases = dynamicsContext.incidents.Expand(x => x.dfp_incident_dfp_comment)
                .Where(i => i.incidentid == Guid.Parse(caseId))
                .ToList();

            foreach (var @case in @cases)
            {
                // ensure related data is loaded.

                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_comment));
                foreach (var comment in @case.dfp_incident_dfp_comment)
                {
                    // determine if there is a match.

                    bool originMatch = false;

                    switch (originRestrictions)
                    {
                        case OriginRestrictions.None:
                            originMatch = true;
                            break;
                        case OriginRestrictions.UserOnly:
                            if (comment.dfp_origin != null && comment.dfp_origin == (int?)OriginTypes.User)
                            {
                                originMatch = true;
                            }

                            break;
                        case OriginRestrictions.SystemOnly:
                            if (comment.dfp_origin != null && comment.dfp_origin == (int?)OriginTypes.System)
                            {
                                originMatch = true;
                            }

                            break;
                    }

                    // ignore inactive and system generated
                    if ((comment.statecode != null && comment.statecode == 0) && originMatch)
                    {
                        await dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.dfp_commentid));
                        await dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.owninguser));
                        if (allComments || comment.dfp_icbc.GetValueOrDefault())
                        {
                            int sequenceNumber = 1;
                            int.TryParse(comment.dfp_caseidguid, out sequenceNumber);

                            LegacyComment legacyComment = new LegacyComment
                            {
                                CaseId = @case.incidentid.ToString(),
                                CommentDate = comment.createdon.GetValueOrDefault(),
                                CommentId = comment.dfp_commentid.ToString(),
                                CommentText = comment.dfp_commentdetails,
                                CommentTypeCode = TranslateCommentTypeCodeFromInt(comment.dfp_commenttype),
                                SequenceNumber = sequenceNumber,
                                UserId = comment.dfp_userid
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

            return result;
        }


        /// <summary>
        /// Get Case Legacy Comments
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="allComments"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyComment>> GetDriverLegacyComments(string driverId, bool allComments,
            OriginRestrictions originRestrictions)
        {
            List<LegacyComment> result = new List<LegacyComment>();
            // start by the driver


            var comments = dynamicsContext.dfp_comments.Where(i => i._dfp_driverid_value == Guid.Parse(driverId))
                .ToList();

            foreach (var comment in comments)
            {
                // ensure related data is loaded.


                // determine if there is a match.

                bool originMatch = false;

                switch (originRestrictions)
                {
                    case OriginRestrictions.None:
                        originMatch = true;
                        break;
                    case OriginRestrictions.UserOnly:
                        if (comment.dfp_origin != null && comment.dfp_origin == (int?)OriginTypes.User)
                        {
                            originMatch = true;
                        }

                        break;
                    case OriginRestrictions.SystemOnly:
                        if (comment.dfp_origin != null && comment.dfp_origin == (int?)OriginTypes.System)
                        {
                            originMatch = true;
                        }

                        break;
                }

                // ignore inactive and system generated
                if ((comment.statecode != null && comment.statecode == 0) && originMatch)
                {
                    await dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.dfp_commentid));
                    await dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.owninguser));
                    if (allComments || comment.dfp_icbc.GetValueOrDefault())
                    {
                        int sequenceNumber = 0;
                        int.TryParse(comment.dfp_caseidguid, out sequenceNumber);

                        LegacyComment legacyComment = new LegacyComment
                        {
                            CommentDate = comment.createdon.GetValueOrDefault(),
                            CommentId = comment.dfp_commentid.ToString(),
                            CommentText = comment.dfp_commentdetails,
                            CommentTypeCode = TranslateCommentTypeCodeFromInt(comment.dfp_commenttype),
                            SequenceNumber = sequenceNumber,
                            UserId = comment.dfp_userid
                        };

                        if (comment._dfp_caseid_value != null)
                        {
                            legacyComment.CaseId = comment._dfp_caseid_value.ToString();
                        }

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

            return result;
        }

        /// <summary>
        /// Get Driver Legacy Comments
        /// </summary>
        /// <param name="driverLicenceNumber"></param>
        /// <param name="allComments"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyComment>> GetDriverLegacyComments(string driverLicenceNumber,
            bool allComments)
        {
            List<LegacyComment> result = new List<LegacyComment>();
            // start by the driver

            var @drivers = dynamicsContext.dfp_drivers
                .Where(d => d.dfp_licensenumber == driverLicenceNumber && d.statuscode == 1).ToList();

            foreach (var driverItem in @drivers)
            {
                if (driverItem != null)
                {
                    await dynamicsContext.LoadPropertyAsync(driverItem, nameof(dfp_driver.dfp_PersonId));
                    Driver driver = new Driver()
                    {
                        DriverLicenseNumber = driverItem.dfp_licensenumber,
                        Surname = driverItem.dfp_PersonId?.lastname ?? String.Empty
                    };


                    // get the cases for that driver.
                    var comments = dynamicsContext.dfp_comments.Where(
                        i => i._dfp_driverid_value == driverItem.dfp_driverid && i.dfp_origin == (int?)OriginTypes.User
                    ).OrderByDescending(x => x.dfp_legacyid).OrderByDescending(x => x.createdon).ToList();


                    foreach (var comment in comments)
                    {
                        await dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.dfp_commentid));
                        await dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.owninguser));

                        if (allComments || comment.dfp_icbc.GetValueOrDefault())
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


                                LegacyComment legacyComment = new LegacyComment
                                {
                                    CaseId = caseId,
                                    CommentDate = comment.createdon.GetValueOrDefault(),
                                    CommentId = comment.dfp_commentid.ToString(),
                                    CommentText = comment.dfp_commentdetails,
                                    CommentTypeCode = TranslateCommentTypeCodeFromInt(comment.dfp_commenttype),
                                    SequenceNumber = sequenceNumber,
                                    UserId = comment.dfp_userid,
                                    Driver = driver
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
        /// Get Driver Legacy Documents by driver id
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyDocument>> GetDriverLegacyDocuments(Guid driverId)
        {
            var driverDocuments = dynamicsContext.bcgov_documenturls
                .Expand(d => d.dfp_DocumentTypeID)
                .Expand(sd => sd.dfp_DocumentSubType)
                .Expand(d => d.bcgov_CaseId)
                .Where(d => d._dfp_driverid_value == driverId && d.statecode == (int)EntityState.Active)
                .ToList();

            return _mapper.Map<IEnumerable<LegacyDocument>>(driverDocuments);
        }

        public IEnumerable<dfp_driver> GetDriverObjects(string licensenumber)
        {
            return dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId)
                .Where(d => d.statuscode == 1 && d.dfp_licensenumber == licensenumber).ToList();
        }

        public IEnumerable<dfp_driver> GetDriverObjectsById(string id)
        {
            return dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId)
                .Where(d => d.statuscode == 1 && d.dfp_driverid == Guid.Parse(id)).ToList();
        }

        public async Task<IEnumerable<Driver>> GetDriverByLicenseNumber(string licensenumber)
        {
            List<Driver> result = new List<Driver>();

            var @drivers = GetDriverObjects(licensenumber);

            foreach (var item in @drivers)
            {
                Driver d = new Driver()
                {
                    Id = item.dfp_driverid.ToString(),
                    DriverLicenseNumber = item.dfp_licensenumber,
                    Surname = item.dfp_PersonId?.lastname,
                    BirthDate = item.dfp_PersonId?.birthdate ?? default(DateTime),
                };
                result.Add(d);
            }

            return result;
        }

        public async Task<IEnumerable<Driver>> GetDriverById(string id)
        {
            List<Driver> result = new List<Driver>();

            var @drivers = GetDriverObjectsById(id);

            foreach (var item in @drivers)
            {
                Driver d = new Driver()
                {
                    Id = item.dfp_driverid.ToString(),
                    DriverLicenseNumber = item.dfp_licensenumber,
                    Surname = item.dfp_PersonId?.lastname,
                    GivenName = item.dfp_PersonId?.firstname
                };
                result.Add(d);
            }

            return result;
        }


        /// <summary>
        /// Get Drivers
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Driver>> GetDrivers()
        {
            List<Driver> result = new List<Driver>();

            var @drivers = dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d => d.statuscode == 1)
                .ToList();

            foreach (var item in @drivers)
            {
                Driver d = new Driver()
                {
                    DriverLicenseNumber = item.dfp_licensenumber,
                    Surname = item.dfp_PersonId?.lastname,
                };
                result.Add(d);
            }

            return result;
        }


        public Driver GetDriverById(Guid id)
        {
            Driver result = null;

            var driver = dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d => d.dfp_driverid == id)
                .FirstOrDefault();

            if (driver != null)
            {
                result = new Driver()
                {
                    DriverLicenseNumber = driver.dfp_licensenumber,
                    Surname = driver.dfp_PersonId?.lastname
                };
            }

            return result;
        }

        /// <summary>
        /// Get Cases by driver
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="activeStatus"></param>
        /// <returns>IEnumerable<CaseDetail></returns>
        public async Task<IEnumerable<CaseDetail>> GetCases(Guid driverId, EntityState activeStatus)
        {
            var result = new List<CaseDetail>();

            try
            {
                var cases = dynamicsContext.incidents.Where(d =>
                    d._dfp_driverid_value == driverId && d.statecode == (int)activeStatus);
                if (cases != null)
                {
                    var mostRecentProcessingDate = GetDpsProcessingDate();

                    foreach (var @case in cases)
                    {
                        var mappedCase = await _caseMapper.Map(@case);
                        mappedCase.DpsProcessingDate = mostRecentProcessingDate;
                        result.Add(mappedCase);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Error getting cases for driver {driverId}");
            }

            return result;
        }

        /// <summary>
        /// GetCaseDetail
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        public async Task<CaseDetail> GetCaseDetail(string caseId)
        {
            CaseDetail result = null;

            try
            {
                var fetchedCase = dynamicsContext.incidents
                    .Expand(c => c.dfp_DriverId)
                    .Expand(c => c.dfp_DriverId.dfp_PersonId)
                    .Expand(c => c.dfp_incident_dfp_knownmedicalcondition)
                    .Where(d => d.incidentid == Guid.Parse(caseId))
                    .FirstOrDefault();
                if (fetchedCase != null)
                {
                    result = await _caseMapper.Map(fetchedCase);
                    result.DpsProcessingDate = GetDpsProcessingDate();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Error getting case {caseId}");
            }

            return result;
        }

        public async Task<CaseDetail> GetCaseByIdCode(string idCode)
        {
            CaseDetail result = null;

            try
            {
                var @case = dynamicsContext.incidents
                    .Expand(i => i.dfp_DriverId)
                    .Expand(i => i.dfp_incident_dfp_knownmedicalcondition)
                    .Where(i => i.ticketnumber == idCode)
                    .Where(i => i.statuscode != (int)StatusCodeOptionSet.Sent)
                    .FirstOrDefault();

                if (@case != null)
                {
                    return await _caseMapper.Map(@case);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Error getting case {idCode}");
            }

            return result;
        }

        /// <summary>
        /// GetMostRecentCaseDetail
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        public async Task<CaseDetail> GetMostRecentCaseDetail(Guid driverId)
        {
            CaseDetail result = null;

            try
            {
                var fetchedCase = dynamicsContext.incidents.Where(i => i.dfp_DriverId.dfp_driverid == driverId)
                    .OrderByDescending(x => x.createdon).FirstOrDefault();

                if (fetchedCase != null)
                {
                    result = await _caseMapper.Map(fetchedCase);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Error getting driver {driverId}");
            }

            return result;
        }

        /// <summary>
        /// Get Legacy Document
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<LegacyComment> GetComment(string commentId)
        {
            LegacyComment legacyComment = null;
            try
            {
                var comment = dynamicsContext.dfp_comments.Where(d => d.dfp_commentid == Guid.Parse(commentId))
                    .FirstOrDefault();
                await dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.owninguser));

                if (comment != null)
                {
                    // fetch the driver
                    Driver driver = null;
                    if (comment._dfp_driverid_value != null)
                    {
                        GetDriverById(comment._dfp_driverid_value.Value);
                    }

                    int sequenceNumber = 0;
                    int.TryParse(comment.dfp_caseidguid, out sequenceNumber);

                    legacyComment = new LegacyComment
                    {
                        CaseId = comment._dfp_caseid_value.ToString(),
                        CommentDate = comment.createdon.GetValueOrDefault(),
                        CommentId = comment.dfp_commentid.ToString(),
                        CommentText = comment.dfp_commentdetails,
                        CommentTypeCode = TranslateCommentTypeCodeFromInt(comment.dfp_commenttype),
                        SequenceNumber = sequenceNumber,
                        UserId = comment.dfp_userid,
                        Driver = driver
                    };

                    if (comment.owninguser != null &&
                        comment.owninguser.dfp_signaturename != null &&
                        !comment.owninguser.dfp_signaturename.Contains("Service Account"))
                    {
                        legacyComment.SignatureName = comment.owninguser.dfp_signaturename;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Error getting comment {commentId}");
            }

            return legacyComment;
        }

        [Obsolete("Use CallbackManager.Create instead.")]
        public async Task<ResultStatusReply> CreateBringForward(BringForwardRequest request)
        {
            ResultStatusReply result = new ResultStatusReply()
            {
                Success = false
            };

            string caseId = request.CaseId;

            if (!string.IsNullOrEmpty(caseId))
            {
                task newTask = new task()
                {
                    description = request.Description,
                    subject = request.Subject,
                    scheduledend = DateTimeOffset.UtcNow,
                    prioritycode = (int?)request.Priority,
                    //ownerid = request.Assignee
                };

                // Get the case
                var @case = GetIncidentById(caseId);

                // load owner
                if (string.IsNullOrEmpty(request.Assignee))
                {
                    if (@case._owningteam_value != null)
                    {
                        // create a reference to team
                        var caseTeam = dynamicsContext.teams.ByKey(@case._owningteam_value.Value).GetValue();
                        dynamicsContext.AddTotasks(newTask);
                        dynamicsContext.SetLink(newTask, nameof(task.ownerid), caseTeam);
                    }
                    else
                    {
                        // create a reference to system user
                        var caseUser = dynamicsContext.systemusers.ByKey(@case._owninguser_value).GetValue();
                        dynamicsContext.AddTotasks(newTask);
                        dynamicsContext.SetLink(newTask, nameof(task.ownerid), caseUser);
                    }
                }
                else
                {
                    // set the BF owner to request assignee
                    if (@case._ownerid_value != null)
                    {
                        dynamicsContext.AddTotasks(newTask);
                        dynamicsContext.SetLink(newTask, nameof(task.ownerid), @case._ownerid_value);
                    }

                    ;
                }

                // Create a bring Forward
                try
                {
                    // set Case Id
                    dynamicsContext.SetLink(newTask, nameof(task.regardingobjectid_incident), @case);

                    await dynamicsContext.SaveChangesAsync();
                    result.Success = true;
                    dynamicsContext.DetachAll();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    Log.Logger.Error(ex.Message);
                    result.ErrorDetail = ex.Message;
                }
            }

            return result;
        }


        /// <summary>
        /// Get Incident By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected incident GetIncidentById(string id)
        {
            incident result = null;
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    Guid idGuid = Guid.Parse(id);

                    if (idGuid != Guid.Empty)
                    {
                        result = dynamicsContext.incidents.Where(d => d.incidentid == Guid.Parse(id)).FirstOrDefault();
                        // ensure the driver is fetched.
                        LazyLoadProperties(result).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    result = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Create Legacy Case Comment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateStatusReply> CreateLegacyCaseComment(LegacyComment request)
        {
            CreateStatusReply result = new CreateStatusReply()
            {
                Success = false,
                ErrorDetail = "unknown error - CreateLegacyCaseComment"
            };

            dfp_comment comment = null;

            var driver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();
            if (driver == null)
            {
                var driverResult = await CreateDriver(new CreateDriverRequest()
                {
                    DriverLicenseNumber = request.Driver.DriverLicenseNumber,
                    BirthDate = request.Driver.BirthDate,
                    SequenceNumber = request.SequenceNumber,
                    Surname = request.Driver.Surname
                });

                driver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();
            }

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
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.AddTodfp_comments(comment);
                    var saveResult = await dynamicsContext.SaveChangesAsync();
                    var tempId = GetCreatedId(saveResult);
                    if (tempId != null)
                    {
                        comment = dynamicsContext.dfp_comments.ByKey(tempId).GetValue();
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
                        dynamicsContext.SetLink(comment, nameof(dfp_comment.dfp_DriverId), driver);

                        await dynamicsContext.SaveChangesAsync();
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
                    comment = dynamicsContext.dfp_comments.ByKey(key).GetValue();
                    comment.dfp_commenttype = TranslateCommentTypeCodeToInt(request.CommentTypeCode);
                    comment.dfp_icbc = request.CommentTypeCode == "W" || request.CommentTypeCode == "I";
                    comment.dfp_userid = request.UserId;
                    comment.dfp_commentdetails = request.CommentText;
                    comment.dfp_date = request.CommentDate;
                    comment.overriddencreatedon = request.CommentDate;

                    dynamicsContext.UpdateObject(comment);
                    dynamicsContext.SaveChanges();

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
                            incident driverCase = dynamicsContext.incidents.ByKey(Guid.Parse(request.CaseId))
                                .GetValue();
                            dynamicsContext.AddLink(driverCase, nameof(incident.dfp_incident_dfp_comment), comment);
                            dynamicsContext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Warning(ex, "Unable to link comment to case");
                        }
                    }
                }
            }

            dynamicsContext.Detach(comment);
            return result;
        }

        /// <summary>
        /// Create ICBC Medical Candidate Comment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateStatusReply> CreateICBCMedicalCandidateComment(LegacyComment request)
        {
            CreateStatusReply result = new CreateStatusReply()
            {
                Success = false,
                ErrorDetail = "unknown error - CreateICBCMedicalCandidateComment"
            };

            dfp_comment comment = null;

            var driver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();
            if (driver == null)
            {
                var newDriver = new CreateDriverRequest()
                {
                    DriverLicenseNumber = request.Driver.DriverLicenseNumber,
                    Surname = request.Driver.Surname ?? string.Empty,
                    SequenceNumber = request.SequenceNumber
                };

                await CreateDriver(newDriver);

                driver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();
            }


            if (string.IsNullOrEmpty(request.CommentId)) // create
            {
                // create the comment
                comment = new dfp_comment()
                {
                    createdon = request.CommentDate,
                    dfp_commenttype = TranslateCommentTypeCodeToInt(request.CommentTypeCode),
                    dfp_icbc = request.CommentTypeCode == "C",
                    dfp_userid = request.UserId,
                    dfp_commentdetails = request.CommentText,
                    dfp_date = request.CommentDate,
                    statecode = 0,
                    statuscode = 1,
                    overriddencreatedon = request.CommentDate,
                    dfp_origin = 100000001
                };


                int sequenceNumber = 0;
                if (request.SequenceNumber != null)
                {
                    sequenceNumber = request.SequenceNumber.Value;
                }

                comment.dfp_caseidguid = sequenceNumber.ToString();

                // Get owner for the comment

                var getCase = GetIncidentById(request.CaseId);

                if (string.IsNullOrEmpty(request.Assignee))
                {
                    principal assignee = null;

                    string assigneeDisplayName;

                    if (getCase._owningteam_value != null)
                    {
                        // create a reference to team
                        var caseTeam = dynamicsContext.teams.ByKey(getCase._owningteam_value.Value).GetValue();

                        assignee = (principal)caseTeam;

                        assigneeDisplayName = caseTeam.name;
                    }
                    else
                    {
                        //create a reference to system user

                        var caseUser = dynamicsContext.systemusers.ByKey(getCase._owninguser_value).GetValue();

                        assignee = (principal)caseUser;

                        assigneeDisplayName = caseUser.dfp_signaturename;
                    }

                    comment.dfp_commentdetails = comment.dfp_commentdetails.Replace("{assignee}", assigneeDisplayName);

                    dynamicsContext.AddTodfp_comments(comment);
                    dynamicsContext.SetLink(comment, nameof(dfp_comment.ownerid), assignee);
                }

                try
                {
                    await dynamicsContext.SaveChangesAsync();
                    //dynamicsContext.AddTodfp_comments(comment);
                    var saveResult = await dynamicsContext.SaveChangesAsync();
                    var tempId = GetCreatedId(saveResult);
                    if (tempId != null)
                    {
                        comment = dynamicsContext.dfp_comments.ByKey(tempId).GetValue();
                    }

                    result.Success = true;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "CreateICBCMedicalCandidateComment Error adding comment");
                    result.Success = false;
                    result.ErrorDetail = "CreateICBCMedicalCandidateComment Error adding comment" + ex.Message;
                }

                if (result.Success == true)
                {
                    try
                    {
                        dynamicsContext.SetLink(comment, nameof(dfp_comment.dfp_DriverId), driver);

                        await dynamicsContext.SaveChangesAsync();
                        result.Success = true;
                        result.Id = comment.dfp_commentid.ToString();
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Error(ex, "CreateICBCMedicalCandidateComment Set Links Error");
                        result.Success = false;
                        result.ErrorDetail = "CreateICBCMedicalCandidateComment Set Links Error" + ex.Message;
                    }
                }
            }

            // Check if the CaseId is null
            if (!string.IsNullOrEmpty(request.CaseId))
            {
                try
                {
                    incident driverCase = dynamicsContext.incidents.ByKey(Guid.Parse(request.CaseId)).GetValue();
                    dynamicsContext.AddLink(driverCase, nameof(incident.dfp_incident_dfp_comment), comment);
                    await dynamicsContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, "Unable to link comment to case");
                }
            }

            dynamicsContext.DetachAll();

            return result;
        }

        /// <summary>
        /// Get Document Type
        /// </summary>
        /// <param name="documentTypeCode"></param>
        /// <param name="documentType"></param>
        /// <param name="businessArea"></param>
        /// <returns></returns>
        private dfp_submittaltype GetDocumentType(string documentTypeCodeInput, string documentType,
            string businessArea)
        {
            dfp_submittaltype result = null;
            string documentTypeCode = null;
            if (!string.IsNullOrEmpty(documentTypeCodeInput))
            {
                documentTypeCode = documentTypeCodeInput;
            }
            else
            {
                if (!string.IsNullOrEmpty(documentType))
                {
                    documentTypeCode = documentType.Replace(" ", "");
                }
            }
            // lookup the document Type Code

            if (!string.IsNullOrEmpty(documentTypeCode))
            {
                try
                {
                    result = dynamicsContext.dfp_submittaltypes.Where(d => d.dfp_apidocumenttype == documentTypeCode)
                        .FirstOrDefault();
                    if (result == null)
                    {
                        result = dynamicsContext.dfp_submittaltypes.Where(d => d.dfp_code == documentTypeCode)
                            .FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, $"Error searching for document Type Code {documentTypeCode}");
                    result = null;
                }
            }

            if (result == null)
            {
                Serilog.Log.Information($"Attempting to add {documentTypeCode} {documentType}");
                // try to create.
                var newRecord = new dfp_submittaltype()
                {
                    dfp_apidocumenttype = documentTypeCode,
                    dfp_code = documentTypeCode,
                    dfp_name = documentType ?? documentTypeCode
                };

                if (businessArea != null)
                {
                    newRecord.dfp_businessarea = ConvertStringToBusinessArea(businessArea);
                }

                dynamicsContext.AddTodfp_submittaltypes(newRecord);
                dynamicsContext.SaveChanges();

                result = dynamicsContext.dfp_submittaltypes.Where(d => d.dfp_apidocumenttype == documentTypeCode)
                    .FirstOrDefault();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateStatusReply> CreateUnsolicitedCaseDocument(LegacyDocument request)
        {
            CreateStatusReply result = new CreateStatusReply();

            // Step 1: Check the driver if cannot find create the driver

            bool secondCandidateCreate = true;

            var searchdriver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();

            if (searchdriver == null)
            {
                var newDriver = new CreateDriverRequest()
                {
                    DriverLicenseNumber = request.Driver.DriverLicenseNumber,
                    Surname = request.Driver.Surname ?? string.Empty,
                    SequenceNumber = request.SequenceNumber
                };

                await CreateDriver(newDriver);

                secondCandidateCreate = false;
                searchdriver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();
            }

            // Step 2: Check for case if cannot find create the case

            incident searchcase = GetIncidentById(request.CaseId);


            if (searchcase == null)
            {
                var newCase = new CreateCaseRequest()
                {
                    DriverLicenseNumber = request.Driver.DriverLicenseNumber,
                    SequenceNumber = request.SequenceNumber
                };

                if (secondCandidateCreate)
                {
                    // create the case                   
                    await CreateCase(newCase);
                }
            }

            // Create the unsolicitated document


            result = await CreateCaseDocument(request);

            return result;
        }


        /// <summary>
        /// CreateICBCDocumentEnvelope
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateStatusReply> CreateICBCDocumentEnvelope(LegacyDocument request)
        {
            CreateStatusReply result = new CreateStatusReply();


            // Create a document enevelope
            var documentEnvelope = new LegacyDocument()
            {
                SubmittalStatus = request.SubmittalStatus,
                DocumentType = request.DocumentType,
                DocumentTypeCode = request.DocumentTypeCode,
                Driver = new Driver()
                {
                    DriverLicenseNumber = request.Driver.DriverLicenseNumber
                },
                CaseId = request.CaseId,


                ImportDate = request.ImportDate,
                DocumentId = request.DocumentId,
                SequenceNumber = request.SequenceNumber,
                Solicited = true,
                Owner = request.Owner,


                //DueDate = request.DueDate
            };

            if (request.FaxReceivedDate != null)
            {
                documentEnvelope.FaxReceivedDate = request.FaxReceivedDate;
            }

            result = await CreateCaseDocument(documentEnvelope);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateStatusReply> CreateDocumentOnDriver(LegacyDocument request)
        {
            CreateStatusReply result = new CreateStatusReply();
            // Search for triage driver
            //bool secondCandidateCreate = true;

            dfp_driver searchdriver;

            if (!string.IsNullOrEmpty(request.Driver.Id))
            {
                searchdriver = GetDriverObjectsById(request.Driver.Id).FirstOrDefault();
            }
            else
            {
                searchdriver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();
            }

            if (searchdriver == null)
            {
                var newDriver = new CreateDriverRequest()
                {
                    DriverLicenseNumber = request.Driver.DriverLicenseNumber,
                    Surname = request.Driver.Surname ?? string.Empty,
                    SequenceNumber = request.SequenceNumber,
                    //BirthDate = request.Driver.BirthDate,
                    GivenName = request.Driver.GivenName ?? string.Empty,
                };

                var birthDate = request.Driver.BirthDate;

                // Check DOB is null and also verify if DOB is not less 1753 and greater than today

                if (birthDate != null && !(birthDate.Year < 1753 || birthDate > DateTime.Now.AddHours(-1)))
                {
                    newDriver.BirthDate = request.Driver.BirthDate;
                }

                await CreateDriver(newDriver);
                searchdriver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();
            }

            // Get Document Type
            var documentTypeId = GetDocumentType(request.DocumentTypeCode, request.DocumentType, request.BusinessArea);

            if (searchdriver != null)
            {
                bool found = false;

                bcgov_documenturl bcgovDocumentUrl = null;

                if (request.SubmittalStatus == "Reject")
                {
                    //Create a new document with the reject status attached to the driver

                    found = false;
                }

                else
                {
                    if (request.Driver.DriverLicenseNumber != "00000000")
                    {
                        // scan through the documents to see if there is document in open pending status
                        // ensure we have the documents.
                        await dynamicsContext.LoadPropertyAsync(searchdriver,
                            nameof(dfp_driver.dfp_driver_bcgov_documenturl));


                        if (documentTypeId != null && request.DocumentType != "Unclassified")
                        {
                            // Query the documents entity to get the open required envelope for the given driver
                            var docs = dynamicsContext.bcgov_documenturls.Where(x => x.statecode == 0
                                && x._dfp_driverid_value == searchdriver.dfp_driverid
                                && x._dfp_documenttypeid_value == documentTypeId.dfp_submittaltypeid
                                && x.dfp_submittalstatus == 100000000).FirstOrDefault();

                            // check the result and set found to true if there is document
                            if (docs != null)
                            {
                                bcgovDocumentUrl = docs;
                                found = true;
                            }
                        }
                    }
                }


                // Load Driver lookup from the documents entity
                // await dynamicsContext.LoadPropertyAsync(searchdriver, nameof(dfp_driver.dfp_driver_bcgov_documenturl));

                principal newOwner = LookupTeam(request.Owner, request.ValidationPrevious);


                // Create the document 

                if (bcgovDocumentUrl == null)
                {
                    bcgovDocumentUrl = new bcgov_documenturl();
                }

                bcgovDocumentUrl.dfp_attachmentnumber = (int)request.DpsDocumentId;

                bcgovDocumentUrl.dfp_batchid = request.BatchId;
                bcgovDocumentUrl.dfp_documentpages = request.DocumentPages.ToString();
                bcgovDocumentUrl.bcgov_url = request.DocumentUrl;
                bcgovDocumentUrl.bcgov_receiveddate = request.FaxReceivedDate;
                bcgovDocumentUrl.dfp_faxreceiveddate = request.FaxReceivedDate;

                if (request.ImportDate != null && request.ImportDate.Value.Year > 1)
                {
                    bcgovDocumentUrl.dfp_uploadeddate = request.ImportDate;
                    bcgovDocumentUrl.dfp_dpsprocessingdate = request.ImportDate;
                }
                else
                {
                    bcgovDocumentUrl.dfp_uploadeddate = DateTimeOffset.Now;
                    bcgovDocumentUrl.dfp_dpsprocessingdate = DateTimeOffset.Now;
                }


                bcgovDocumentUrl.dfp_importid = request.ImportId;
                bcgovDocumentUrl.dfp_faxnumber = request.OriginatingNumber;
                bcgovDocumentUrl.dfp_validationmethod = request.ValidationMethod;
                bcgovDocumentUrl.dfp_validationprevious = request.ValidationPrevious ?? request.UserId;
                bcgovDocumentUrl.dfp_submittalstatus = TranslateSubmittalStatusString(request.SubmittalStatus);
                bcgovDocumentUrl.dfp_priority = TranslatePriorityCode(request.Priority);
                bcgovDocumentUrl.dfp_issuedate = DateTimeOffset.Now;

                bcgovDocumentUrl.dfp_dpspriority = TranslatePriorityCode(request.Priority);
                bcgovDocumentUrl.dfp_documentorigin = TranslateDocumentOrigin(request.Origin);
                bcgovDocumentUrl.dfp_queue = TranslateQueueCode(request.Queue);
                if (!string.IsNullOrEmpty(request.FilenameOverride))
                {
                    bcgovDocumentUrl.bcgov_fileextension = Path.GetExtension(request.FilenameOverride);
                    bcgovDocumentUrl.bcgov_filename = request.FilenameOverride;
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.DocumentUrl))
                    {
                        bcgovDocumentUrl.bcgov_fileextension = Path.GetExtension(request.DocumentUrl);
                        bcgovDocumentUrl.bcgov_filename = Path.GetFileName(request.DocumentUrl);
                    }
                }

                bcgovDocumentUrl.dfp_solicited = request.Solicited;

                if (!string.IsNullOrEmpty(request.Origin) && request.Origin == "DPS/KOFAX")
                {
                    if (!found) // only clear the solicited flag if this is a new document; do not clear for "existing envelope"
                    {
                        bcgovDocumentUrl.dfp_solicited = false; // non-user documents default to not solicited
                    }
                }

                if (found) // update
                {
                    // 12-20-2023 - Removed code to set solicited on a found document envelope; as it will be set by the ICBC service receiving candidates
                    try
                    {
                        dynamicsContext.UpdateObject(bcgovDocumentUrl);
                        dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgov_documenturl.dfp_DocumentTypeID),
                            documentTypeId);
                        dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgov_documenturl.dfp_DriverId), searchdriver);
                        dynamicsContext.SaveChanges();
                        if (!string.IsNullOrEmpty(request.DocumentSubTypeId) &&
                            request.DocumentSubTypeId != Guid.Empty.ToString())
                        {
                            var documentSubType = dynamicsContext.dfp_documentsubtypes.Where(d =>
                                    d.dfp_documentsubtypeid == Guid.Parse(request.DocumentSubTypeId))
                                .FirstOrDefault();
                            if (documentSubType != null)
                            {
                                dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgov_documenturl.dfp_DocumentSubType),
                                    documentSubType);
                            }
                        }

                        dynamicsContext.SaveChanges();
                        result.Success = true;
                        result.Id = bcgovDocumentUrl.bcgov_documenturlid.ToString();
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Error(ex, "CreateLegacyCaseDocument");
                        result.Success = false;
                    }
                }
                else // insert
                {
                    try
                    {
                        await dynamicsContext.SaveChangesAsync();
                        dynamicsContext.AddTobcgov_documenturls(bcgovDocumentUrl);
                        var saveResult = await dynamicsContext.SaveChangesAsync();
                        var tempId = GetCreatedId(saveResult);
                        if (tempId != null)
                        {
                            bcgovDocumentUrl = dynamicsContext.bcgov_documenturls.ByKey(tempId).GetValue();
                        }

                        if (documentTypeId != null)
                        {
                            dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgov_documenturl.dfp_DocumentTypeID),
                                documentTypeId);
                        }

                        dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgovDocumentUrl.dfp_DriverId), searchdriver);

                        if (newOwner != null)
                        {
                            dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgovDocumentUrl.ownerid), newOwner);
                        }

                        if (!string.IsNullOrEmpty(request.DocumentSubTypeId))
                        {
                            var documentSubType = dynamicsContext.dfp_documentsubtypes.Where(d =>
                                    d.dfp_documentsubtypeid == Guid.Parse(request.DocumentSubTypeId))
                                .FirstOrDefault();
                            if (documentSubType != null)
                            {
                                dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgov_documenturl.dfp_DocumentSubType),
                                    documentSubType);
                            }
                        }

                        dynamicsContext.SaveChanges();

                        if (!string.IsNullOrEmpty(request.CaseId) && request.CaseId != Guid.Empty.ToString() &&
                            request.CaseId.ToLower() != "null" && request.CaseId.ToLower() != "error")
                        {
                            try
                            {
                                Guid caseId = Guid.Parse(request.CaseId);
                                if (caseId != Guid.Empty)
                                {
                                    var theCase = dynamicsContext.incidents.Where(d => d.incidentid == caseId)
                                        .FirstOrDefault();
                                    ;
                                    if (theCase != null)
                                    {
                                        dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgovDocumentUrl.bcgov_CaseId),
                                            theCase);
                                        dynamicsContext.SaveChanges();
                                    }

                                    dynamicsContext.Detach(theCase);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex,
                                    $"CreateDocumentOnDriver Error linking Case {request.CaseId} to Document.");
                            }
                        }

                        result.Success = true;
                        result.Id = bcgovDocumentUrl.bcgov_documenturlid.ToString();
                        dynamicsContext.Detach(bcgovDocumentUrl);
                        if (newOwner != null)
                        {
                            dynamicsContext.Detach(newOwner);
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Error(ex, "Cannot create a Document");
                        result.Success = false;
                    }
                }
            }

            return result;
        }

        // NOTE keep in sync with DocumentMapper.TranslateDocumentOrigin
        protected int TranslateDocumentOrigin(string documentOrigin)
        {
            var statusMap = new Dictionary<string, int>()
            {
                { "Practitioner Portal", 100000000 },
                { "Partner Portal", 100000001 },
                { "Mercury Uploaded RSBC", 100000014 },
                { "Migration", 100000015 },
                { "Driver Portal", 100000016 },
                { "DPS/KOFAX", 100000017 },
            };

            if (statusMap.ContainsKey(documentOrigin))
            {
                return statusMap[documentOrigin];
            }
            else
            {
                return statusMap["Mercury Uploaded RSBC"];
            }
        }

        /// <summary>
        /// Create Case Document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateStatusReply> CreateCaseDocument(LegacyDocument request)
        {
            CreateStatusReply result = new CreateStatusReply();

            // Search for driver
            var searchDriver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();

            // Search for case
            incident searchcase = GetIncidentById(request.CaseId);

            var documentTypeId = GetDocumentType(request.DocumentTypeCode, request.DocumentType, request.BusinessArea);

            if (searchcase != null && searchDriver != null)
            {
                // Create the case document 


                bcgov_documenturl bcgovDocumentUrl = null;

                await dynamicsContext.LoadPropertyAsync(searchcase, nameof(incident.bcgov_incident_bcgov_documenturl));


                var newOwner = LookupTeam(request.Owner, request.ValidationPrevious);

                // Create the document 

                if (bcgovDocumentUrl == null)
                {
                    bcgovDocumentUrl = new bcgov_documenturl();
                }

                bcgovDocumentUrl.dfp_batchid = request.BatchId;
                bcgovDocumentUrl.dfp_documentpages = request.DocumentPages.ToString();
                bcgovDocumentUrl.bcgov_url = request.DocumentUrl;
                bcgovDocumentUrl.bcgov_receiveddate = DateTimeOffset.Now;
                bcgovDocumentUrl.dfp_faxreceiveddate = request.FaxReceivedDate;
                bcgovDocumentUrl.dfp_uploadeddate = request.ImportDate;
                bcgovDocumentUrl.dfp_dpsprocessingdate = request.ImportDate;
                bcgovDocumentUrl.dfp_importid = request.ImportId;
                bcgovDocumentUrl.dfp_faxnumber = request.OriginatingNumber;
                bcgovDocumentUrl.dfp_validationmethod = request.ValidationMethod;
                bcgovDocumentUrl.dfp_validationprevious = request.ValidationPrevious ?? request.UserId;
                bcgovDocumentUrl.dfp_submittalstatus = TranslateSubmittalStatusString(request.SubmittalStatus);
                //bcgovDocumentUrl.dfp_submittalstatus = 100000000; // Open Required
                bcgovDocumentUrl.dfp_priority = TranslatePriorityCode(request.Priority);
                bcgovDocumentUrl.dfp_issuedate = DateTimeOffset.Now;
                bcgovDocumentUrl.dfp_solicited = request.Solicited;

                if (!string.IsNullOrEmpty(request.DocumentUrl))
                {
                    bcgovDocumentUrl.bcgov_fileextension = Path.GetExtension(request.DocumentUrl);
                    bcgovDocumentUrl.bcgov_filename = Path.GetFileName(request.DocumentUrl);
                }


                try
                {
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.AddTobcgov_documenturls(bcgovDocumentUrl);
                    var saveResult = await dynamicsContext.SaveChangesAsync();
                    var tempId = GetCreatedId(saveResult);
                    if (tempId != null)
                    {
                        bcgovDocumentUrl = dynamicsContext.bcgov_documenturls.ByKey(tempId).GetValue();
                    }

                    if (documentTypeId != null)
                    {
                        dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgov_documenturl.dfp_DocumentTypeID),
                            documentTypeId);
                    }

                    if (searchcase != null)
                    {
                        dynamicsContext.AddLink(searchcase, nameof(incident.bcgov_incident_bcgov_documenturl),
                            bcgovDocumentUrl);
                    }

                    dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgovDocumentUrl.dfp_DriverId), searchDriver);


                    if (newOwner != null)
                    {
                        dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgovDocumentUrl.ownerid), newOwner);
                    }

                    await dynamicsContext.SaveChangesAsync();
                    result.Success = true;
                    result.Id = bcgovDocumentUrl.bcgov_documenturlid.ToString();
                    dynamicsContext.DetachAll();
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "CreateCaseDocument");
                    result.Success = false;
                }
            }

            return result;
        }


        /// <summary>
        /// Create Legacy Case Document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateStatusReply> CreateLegacyCaseDocument(LegacyDocument request)
        {
            CreateStatusReply result = new CreateStatusReply();
            // get the driver
            bool secondCandidateCreate = true;

            var driver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();
            if (driver == null)
            {
                var driverResult = await CreateDriver(new CreateDriverRequest()
                {
                    DriverLicenseNumber = request.Driver.DriverLicenseNumber,
                    BirthDate = request.Driver.BirthDate,
                    SequenceNumber = request.SequenceNumber,
                    Surname = request.Driver.Surname
                });

                secondCandidateCreate = false;
                driver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();
            }

            if (driver != null && driver.dfp_licensenumber == "00000000") // bypass normal logic
            {
                // document type ID
                var documentTypeId =
                    GetDocumentType(request.DocumentTypeCode, request.DocumentType, request.BusinessArea);

                // find the owner.
                var newOwner = LookupTeam(request.Owner, request.ValidationPrevious);

                var bcgovDocumentUrl = new bcgov_documenturl();
                bcgovDocumentUrl.dfp_batchid = request.BatchId;
                bcgovDocumentUrl.dfp_documentpages = request.DocumentPages.ToString();
                bcgovDocumentUrl.bcgov_url = request.DocumentUrl;
                bcgovDocumentUrl.bcgov_receiveddate = DateTimeOffset.Now;
                bcgovDocumentUrl.dfp_faxreceiveddate = request.FaxReceivedDate;
                bcgovDocumentUrl.dfp_uploadeddate = request.ImportDate;
                bcgovDocumentUrl.dfp_dpsprocessingdate = request.ImportDate;
                bcgovDocumentUrl.dfp_importid = request.ImportId;
                bcgovDocumentUrl.dfp_faxnumber = request.OriginatingNumber;
                bcgovDocumentUrl.dfp_validationmethod = request.ValidationMethod;
                bcgovDocumentUrl.dfp_validationprevious = request.ValidationPrevious ?? request.UserId;
                bcgovDocumentUrl.dfp_submittalstatus = TranslateSubmittalStatusString(request.SubmittalStatus);
                bcgovDocumentUrl.dfp_priority = TranslatePriorityCode(request.Priority);
                bcgovDocumentUrl.dfp_issuedate = DateTimeOffset.Now;
                // bcgovDocumentUrl.dpsQueue
                bcgovDocumentUrl.dfp_dpspriority = TranslatePriorityCode(request.Priority);
                bcgovDocumentUrl.dfp_documentorigin = 100000014;
                bcgovDocumentUrl.dfp_queue = TranslateQueueCode(request.Queue);

                if (!string.IsNullOrEmpty(request.DocumentUrl))
                {
                    bcgovDocumentUrl.bcgov_fileextension = Path.GetExtension(request.DocumentUrl);
                    bcgovDocumentUrl.bcgov_filename = Path.GetFileName(request.DocumentUrl);
                }

                await dynamicsContext.SaveChangesAsync();
                dynamicsContext.AddTobcgov_documenturls(bcgovDocumentUrl);
                var saveResult = await dynamicsContext.SaveChangesAsync();
                var tempId = GetCreatedId(saveResult);
                if (tempId != null)
                {
                    bcgovDocumentUrl = dynamicsContext.bcgov_documenturls.ByKey(tempId).GetValue();
                }

                if (documentTypeId != null)
                {
                    dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgov_documenturl.dfp_DocumentTypeID),
                        documentTypeId);
                }

                dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgov_documenturl.dfp_DriverId), driver);

                await dynamicsContext.SaveChangesAsync();
                result.Success = true;
                result.Id = tempId.ToString();
            }
            else
            {
                incident driverCase = GetIncidentById(request.CaseId);

                bool driverMismatch = false;

                if (driverCase != null && driver.dfp_driverid != driverCase._dfp_driverid_value)
                {
                    // driver mismatch
                    driverMismatch = true;
                }


                // document type ID
                var documentTypeId =
                    GetDocumentType(request.DocumentTypeCode, request.DocumentType, request.BusinessArea);


                if (driverCase == null)
                {
                    var newDriver = new LegacyCandidateSearchRequest()
                    {
                        DriverLicenseNumber = request.Driver.DriverLicenseNumber,
                        Surname = request.Driver.Surname ?? string.Empty, SequenceNumber = request.SequenceNumber
                    };
                    if (secondCandidateCreate)
                    {
                        // create it.                    
                        await LegacyCandidateCreate(newDriver, request.Driver.BirthDate, DateTime.Now,
                            "CreateLegacyCaseDocument-2");
                    }

                    var newCaseId = await GetNewestCaseIdForDriver(newDriver);

                    if (newCaseId != null)
                    {
                        driverCase = GetIncidentById(newCaseId.Value.ToString());
                    }
                }

                if (driverCase != null)
                {
                    bool found = false;
                    bcgov_documenturl newDocument = null;

                    if (request.SubmittalStatus == "Reject")
                    {
                        // Create a new document with the reject status attached to the driver

                        found = false;
                    }

                    else
                    {
                        // scan through the documents to see if there is document in open pending status
                        // ensure we have the documents.
                        await dynamicsContext.LoadPropertyAsync(driverCase,
                            nameof(incident.bcgov_incident_bcgov_documenturl));

                        if (documentTypeId != null)
                        {
                            foreach (var doc in driverCase.bcgov_incident_bcgov_documenturl)
                            {
                                await dynamicsContext.LoadPropertyAsync(doc, nameof(newDocument.dfp_DocumentTypeID));
                                if (doc.statecode == 0 // active
                                    && doc.dfp_DocumentTypeID?.dfp_submittaltypeid == documentTypeId.dfp_submittaltypeid
                                    && doc.dfp_submittalstatus == 100000000)
                                {
                                    newDocument = doc;
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    // find the owner.
                    var newOwner = LookupTeam(request.Owner, request.ValidationPrevious);


                    if (newDocument == null)
                    {
                        newDocument = new bcgov_documenturl();
                    }

                    newDocument.dfp_batchid = request.BatchId;
                    newDocument.dfp_documentpages = request.DocumentPages.ToString();
                    newDocument.bcgov_url = request.DocumentUrl;
                    newDocument.bcgov_receiveddate = DateTimeOffset.Now;
                    newDocument.dfp_faxreceiveddate = request.FaxReceivedDate;
                    newDocument.dfp_uploadeddate = request.ImportDate;
                    newDocument.dfp_dpsprocessingdate = request.ImportDate;
                    newDocument.dfp_importid = request.ImportId;
                    newDocument.dfp_faxnumber = request.OriginatingNumber;
                    newDocument.dfp_validationmethod = request.ValidationMethod;
                    newDocument.dfp_validationprevious = request.ValidationPrevious ?? request.UserId;
                    newDocument.dfp_submittalstatus = TranslateSubmittalStatusString(request.SubmittalStatus);
                    newDocument.dfp_priority = TranslatePriorityCode(request.Priority);
                    newDocument.dfp_issuedate = DateTimeOffset.Now;
                    newDocument.dfp_dpspriority = TranslatePriorityCode(request.Priority);
                    newDocument.dfp_documentorigin = 100000014;
                    newDocument.dfp_queue = TranslateQueueCode(request.Queue);

                    if (!string.IsNullOrEmpty(request.DocumentUrl))
                    {
                        newDocument.bcgov_fileextension = Path.GetExtension(request.DocumentUrl);
                        newDocument.bcgov_filename = Path.GetFileName(request.DocumentUrl);
                    }

                    // assign the owner to DPSR if the docuemnttype = "DMER" and submittalStatus = "ManualPass"


                    if (found) // update
                    {
                        try
                        {
                            dynamicsContext.UpdateObject(newDocument);

                            dynamicsContext.SetLink(newDocument, nameof(bcgov_documenturl.dfp_DocumentTypeID),
                                documentTypeId);
                            dynamicsContext.SetLink(newDocument, nameof(bcgov_documenturl.dfp_DriverId), driver);


                            await dynamicsContext.SaveChangesAsync();
                            result.Success = true;
                            result.Id = newDocument.bcgov_documenturlid.ToString();
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Error(ex, "CreateLegacyCaseDocument");
                            result.Success = false;
                        }
                    }
                    else // insert
                    {
                        try
                        {
                            await dynamicsContext.SaveChangesAsync();
                            dynamicsContext.AddTobcgov_documenturls(newDocument);
                            /*
                            if (driverCase != null)
                            {
                                // ensure the incident is tracked.
                                driverCase = dynamicsContext.incidents.Where(x => x.incidentid == driverCase.incidentid).FirstOrDefault();
                                if (driverCase != null)
                                {
                                    dynamicsContext.AddLink(driverCase, nameof(incident.bcgov_incident_bcgov_documenturl), newDocument);
                                }

                            }
                            */

                            if (documentTypeId != null)
                            {
                                dynamicsContext.SetLink(newDocument, nameof(bcgov_documenturl.dfp_DocumentTypeID),
                                    documentTypeId);
                            }

                            dynamicsContext.SetLink(newDocument, nameof(bcgov_documenturl.dfp_DriverId), driver);

                            var saveResult = dynamicsContext.SaveChanges();
                            var tempId = GetCreatedId(saveResult);
                            if (tempId != null)
                            {
                                newDocument = dynamicsContext.bcgov_documenturls.ByKey(tempId).GetValue();
                            }


                            result.Success = true;
                            result.Id = tempId.ToString();
                        }


                        catch (Exception ex)
                        {
                            Serilog.Log.Error(ex, "CreateLegacyCaseDocument");
                            result.Success = false;
                        }
                    }

                    if (request.SubmittalStatus == "Manual Pass" || request.SubmittalStatus == "Clean Pass")
                    {
                        try
                        {
                            var validationPrevious = request.ValidationPrevious;
                            if (!validationPrevious.StartsWith("IDIR\\"))
                            {
                                validationPrevious = $"IDIR\\" + validationPrevious;
                            }

                            systemuser manualPassOwner = dynamicsContext.systemusers
                                .Where(x => x.domainname == validationPrevious).FirstOrDefault();


                            if (manualPassOwner != null)
                            {
                                // set the owner to DPSR
                                //var manualPassOwner = dynamicsContext.systemusers.FirstOrDefault(u => u.domainname == request.ValidationPrevious);
                                dynamicsContext.SetLink(newDocument, nameof(bcgov_documenturl.ownerid),
                                    manualPassOwner);
                            }
                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Error(ex, "Failed to get the user value");
                        }
                    }
                    else
                    {
                        if (newOwner != null)
                        {
                            dynamicsContext.SetLink(newDocument, nameof(bcgov_documenturl.ownerid), newOwner);
                        }
                    }
                }

                await dynamicsContext.SaveChangesAsync();
            }

            dynamicsContext.DetachAll();

            return result;
        }


        /// <summary>
        /// Delete Comment
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteComment(string documentId)
        {
            bool result = false;

            var comment = dynamicsContext.dfp_comments.ByKey(Guid.Parse(documentId)).GetValue();
            if (comment != null)
            {
                dynamicsContext.DeactivateObject(comment, 2);
                // set to inactive.                
                await dynamicsContext.SaveChangesAsync();
                dynamicsContext.DetachAll();
                result = true;
            }
            else
            {
                Log.Error($"Could not find comment {comment}");
            }

            return result;
        }

        /// <summary>
        /// Delete Legacy Document
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<bool> DeactivateLegacyDocument(string documentId)
        {
            bool result = false;

            var document = dynamicsContext.bcgov_documenturls
                .Where(x => x.bcgov_documenturlid == Guid.Parse(documentId)).FirstOrDefault();
            if (document != null)
            {
                dynamicsContext.DeactivateObject(document, 2);
                // set to inactive.                
                await dynamicsContext.SaveChangesAsync();
                result = true;
            }
            else
            {
                Log.Error($"Could not find document {documentId}");
            }

            return result;
        }

        public async Task<bool> DeleteLegacyDocument(string documentId)
        {
            bool result = false;

            try
            {
                var document = dynamicsContext.bcgov_documenturls
                    .Where(x => x.bcgov_documenturlid == Guid.Parse(documentId)).FirstOrDefault();
                if (document != null)
                {
                    dynamicsContext.DeleteObject(document);
                    // set to inactive.                
                    await dynamicsContext.SaveChangesAsync();
                    result = true;
                }
                else
                {
                    Log.Error($"Could not find document {documentId}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error deleting document {documentId}");
            }

            return result;
        }

        /// <summary>
        /// Translate Decision Outcome
        /// </summary>
        /// <param name="decisionId"></param>
        /// <returns></returns>
        private DecisionOutcome? TranslateDecisionOutcome(Guid? decisionId)
        {
            DecisionOutcome? result = null;
            if (decisionId != null)
            {
                // get the decision record.
                var d = dynamicsContext.dfp_decisions.Expand(x => x.dfp_OutcomeStatus)
                    .Where(x => x.dfp_decisionid == decisionId).FirstOrDefault();
                if (d != null && d.dfp_OutcomeStatus != null)
                {
                    // ensure the decision has data.
                    dynamicsContext.LoadPropertyAsync(d, nameof(dfp_decision.dfp_OutcomeStatus)).GetAwaiter()
                        .GetResult();
                    switch (d.dfp_OutcomeStatus.dfp_name)
                    {
                        case "Fit to Drive":
                            result = DecisionOutcome.FitToDrive;
                            break;
                        case "Non - Comply":
                            result = DecisionOutcome.NonComply;
                            break;
                        case "Unfit to Drive":
                            result = DecisionOutcome.UnfitToDrive;
                            break;
                    }
                }
            }

            return result;
        }


        /// <summary>
        ///  Combine First Name and Last Name
        /// //map cases from query results (TODO: consider replacing with AutoMapper)
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <returns></returns>
        private string CombineName(string firstname, string lastname)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(lastname))
            {
                result = lastname.ToUpper();
            }

            if (!string.IsNullOrEmpty(firstname))
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ", ";
                }

                result += firstname.ToUpper();
            }

            return result;
        }

        /// <summary>
        /// Convert String To Business Area
        /// </summary>
        /// <param name="businessArea"></param>
        /// <returns></returns>
        protected int? ConvertStringToBusinessArea(string businessArea)
        {
            int? result = null;
            if (businessArea != null)
            {
                switch (businessArea)
                {
                    case "Driver Fitness":
                        result = 100000000;
                        break;
                    case "Remedial":
                        result = 100000001;
                        break;
                    case "Client Services":
                        result = 100000002;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Map Cases
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
        [Obsolete]
        private CaseSearchReply MapCases(IEnumerable<incident> cases)
        {
            return new CaseSearchReply
            {
                Items = cases.Select(c =>
                {
                    Provider provider;
                    if (c.dfp_MedicalPractitionerId != null)
                    {
                        provider = new CaseManagement.Provider()
                        {
                            Id = c.dfp_MedicalPractitionerId.dfp_medicalpractitionerid.ToString(),
                            Address = new Address()
                            {
                                City = c.dfp_MedicalPractitionerId.dfp_PersonId?.address1_city,
                                Postal = c.dfp_MedicalPractitionerId.dfp_PersonId?.address1_postalcode,
                                Line1 = c.dfp_MedicalPractitionerId.dfp_PersonId?.address1_line1,
                                Line2 = c.dfp_MedicalPractitionerId.dfp_PersonId?.address1_line2,
                            },
                            FaxNumber = c.dfp_MedicalPractitionerId.dfp_PersonId?.fax,
                            GivenName = $"{c.dfp_MedicalPractitionerId.dfp_PersonId?.firstname}",
                            Name =
                                $"{c.dfp_MedicalPractitionerId.dfp_PersonId?.firstname} {c.dfp_MedicalPractitionerId.dfp_PersonId?.lastname}",
                            Surname = $"{c.dfp_MedicalPractitionerId.dfp_PersonId?.lastname}",
                            FaxUseType = "work",
                            PhoneNumber = $"{c.dfp_MedicalPractitionerId.dfp_PersonId?.telephone1}",
                            PhoneExtension = $"{c.dfp_MedicalPractitionerId.dfp_PersonId?.telephone2}",
                            PhoneUseType = "work",
                            ProviderDisplayId = c.dfp_MedicalPractitionerId.dfp_providerid ?? "000000000",
                            ProviderDisplayIdType = "PHID",
                            ProviderRole = "physician",
                            ProviderSpecialty = "cardiology"
                        };
                    }
                    else
                    {
                        provider = null;
                    }


                    CaseManagement.Driver driver = new CaseManagement.Driver()
                    {
                        Id = c.dfp_DriverId?.dfp_driverid.ToString(),
                        Address = new Address()
                        {
                            City = c.dfp_DriverId?.dfp_PersonId?.address1_city ?? string.Empty,
                            Postal = c.dfp_DriverId?.dfp_PersonId?.address1_postalcode ?? string.Empty,
                            Line1 = c.dfp_DriverId?.dfp_PersonId?.address1_line1 ?? string.Empty,
                            Line2 = c.dfp_DriverId?.dfp_PersonId?.address1_line2 ?? string.Empty,
                        },
                        BirthDate = c.dfp_DriverId?.dfp_PersonId?.birthdate ?? default(DateTime),
                        DriverLicenseNumber = c.dfp_DriverId?.dfp_licensenumber,
                        GivenName = c.dfp_DriverId?.dfp_PersonId?.firstname ?? string.Empty,
                        Middlename = c.dfp_DriverId?.dfp_PersonId?.middlename ?? string.Empty,
                        Sex = TranslateGenderCode(c.dfp_DriverId?.dfp_PersonId?.gendercode),
                        Surname = c.dfp_DriverId?.dfp_PersonId?.lastname ?? string.Empty,
                        Name = CombineName(c.dfp_DriverId?.dfp_PersonId?.lastname,
                            c.dfp_DriverId?.dfp_PersonId?.firstname)
                    };


                    return new DmerCase
                    {
                        Id = c.incidentid.ToString(),
                        Title = c.ticketnumber ?? string.Empty,
                        CreatedBy = $"{c.customerid_contact?.lastname?.ToUpper()}, {c.customerid_contact?.firstname}",
                        CreatedOn = c.createdon.Value.DateTime,
                        ModifiedBy = $"{c.customerid_contact?.lastname?.ToUpper()}, {c.customerid_contact?.firstname}",
                        ModifiedOn = FilterLastCaseModified(c.dfp_lastmodifiedcasestatus, c.createdon.Value.DateTime),
                        ClinicId = c.dfp_ClinicId?.accountid.ToString(),
                        ClinicName = c.dfp_ClinicId?.name ?? string.Empty,
                        DmerType = TranslateDmerType(c.dfp_dmertype),
                        Driver = driver,
                        Provider = provider,
                        IsCommercial =
                            c.dfp_iscommercial != null &&
                            c.dfp_iscommercial == 100000000, // convert the optionset to a bool.
                        Flags = c.dfp_incident_dfp_dmerflag
                            .Where(f => f.dfp_FlagId != null) //temp defense against deleted flags
                            .Select(f => new Flag
                            {
                                Id = f.dfp_FlagId?.dfp_id,
                                Description = f.dfp_FlagId?.dfp_description,
                                FormId = f.dfp_FlagId?.dfp_formid
                            }).ToArray(),
                        MedicalConditions = c.dfp_incident_dfp_knownmedicalcondition
                            .Where(f => f.dfp_MedicalConditionId != null &&
                                        f.statecode == 0 &&
                                        (f.statuscode == 100000003 ||
                                         f.statuscode == 1))
                            .Select(f => new MedicalCondition
                            {
                                Id = f.dfp_MedicalConditionId?.dfp_id,
                                Description = f.dfp_MedicalConditionId?.dfp_description,
                                FormId = f.dfp_MedicalConditionId?.dfp_formid
                            }).ToArray(),
                        Decisions = c.dfp_incident_dfp_decision
                            .Select(d => new Decision
                            {
                                Id = d.dfp_decisionid.ToString(),
                                Outcome = TranslateDecisionOutcome(d.dfp_decisionid),
                                CreatedOn = d.createdon ?? default
                            }),
                        Status = TranslateStatus(c.statuscode),
                        CaseSequence = c.dfp_dfcmscasesequencenumber ?? -1
                    };
                }).ToArray()
            };
        }


        /// <summary>
        /// Filter Last Case Modified
        /// </summary>
        /// <param name="value"></param>
        /// <param name="created"></param>
        /// <returns></returns>
        private DateTime FilterLastCaseModified(DateTimeOffset? value, DateTime created)
        {
            DateTime result;
            if (value == null)
            {
                result = created;
            }
            else
            {
                result = value.Value.DateTime;
            }

            return result;
        }


        /// <summary>
        /// Legacy Candidate Create
        /// </summary>
        /// <param name="request"></param>
        /// <param name="birthDate"></param>
        /// <param name="effectiveDate"></param>
        /// <returns></returns>
        public async Task<Guid?> GetNewestCaseIdForDriver(LegacyCandidateSearchRequest request)
        {
            Guid? result = null;

            dfp_driver driver;
            contact driverContact;
            Guid? driverContactId;

            var driverQuery = dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d =>
                d.dfp_licensenumber == request.DriverLicenseNumber && d.statuscode == 1);
            var driverList = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToList();

            driver = driverList.FirstOrDefault();

            if (driver != null)
            {
                var driverCase = dynamicsContext.incidents.OrderByDescending(x => x.createdon)
                    .Where(x => x._dfp_driverid_value == driver.dfp_driverid).FirstOrDefault();
                if (driverCase != null)
                {
                    result = driverCase.incidentid;
                }
            }

            return result;
        }

        public async Task LegacyCandidateCreate(LegacyCandidateSearchRequest request, DateTimeOffset? birthDate,
            DateTimeOffset? effectiveDate, string MedicalType)
        {
            await LegacyCandidateCreate(request, birthDate, effectiveDate, "Unknown", MedicalType);
        }

        /// <summary>
        /// Legacy Candidate Create
        /// </summary>
        /// <param name="request"></param>
        /// <param name="birthDate"></param>
        /// <param name="effectiveDate"></param>
        /// <returns></returns>
        public async Task LegacyCandidateCreate(LegacyCandidateSearchRequest request, DateTimeOffset? birthDate,
            DateTimeOffset? effectiveDate, string source, string medicalType)
        {
            dfp_driver driver = null;
            contact driverContact;
            Guid? driverContactId;

            string randomDriverId = string.Format("e27d7c69-3913-4116-a360-f5e99972b7e8");
            string driverSubId =
                randomDriverId.Substring(0, randomDriverId.Length - request.DriverLicenseNumber.Length);
            Guid driverId = Guid.Parse(driverSubId + request.DriverLicenseNumber);

            // attempt to get the driver by guid.


            if (driver == null) // get by DL 
            {
                var driverQuery = dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d =>
                    d.dfp_licensenumber == request.DriverLicenseNumber && d.statecode == 0); // active
                var data = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToList();

                if (birthDate != null && birthDate.Value.Year < 1753)
                {
                    birthDate = new DateTime(1753, 1, 1);
                }

                if (data != null && data.Count > 0)
                {
                    dfp_driver[] driverResults = data.ToArray();
                    /*
                    if (!string.IsNullOrEmpty(request?.Surname))
                    {
                        driverResults = data.Where(x => x?.dfp_PersonId?.lastname != null && (bool)(x?.dfp_PersonId?.lastname.ToUpper().StartsWith(request?.Surname.ToUpper()))).ToArray();
                    }
                    else
                    {
                        driverResults = data.ToArray();
                    }
                    */
                    if (driverResults.Length > 0)
                    {
                        driver = driverResults[0];
                    }
                }
            }


            if (driver != null)
            {
                await dynamicsContext.LoadPropertyAsync(driver, nameof(dfp_driver.dfp_PersonId));
                // check contact
                if (driver.dfp_PersonId != null)
                {
                    driverContactId = driver.dfp_PersonId.contactid;
                    driverContact = driver.dfp_PersonId;
                }
                else
                {
                    string contactIdString = string.Format("FCBCE0AC-82EF-411D-BD95-DB84D5E3D927");
                    string contactSubId =
                        contactIdString.Substring(0, contactIdString.Length - request.DriverLicenseNumber.Length);

                    var contactId = new Guid(contactSubId + request.DriverLicenseNumber);

                    try
                    {
                        driverContact = dynamicsContext.contacts.ByKey(contactId).GetValue();
                    }
                    catch (Exception)
                    {
                        driverContact = null;
                    }

                    if (driverContact != null)
                    {
                        try
                        {
                            if (driver._dfp_personid_value == null)
                            {
                                dynamicsContext.SetLink(driver, nameof(dfp_driver.dfp_PersonId), driverContact);
                                await dynamicsContext.SaveChangesAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "LegacyCandidateCreate ERROR linking Driver to Contact " + e.Message);
                        }
                    }
                    else
                    {
                        driverContact = new contact()
                        {
                            contactid = contactId,
                            lastname = request.Surname
                        };

                        if (birthDate != null)
                        {
                            driverContact.birthdate = new Microsoft.OData.Edm.Date(birthDate.Value.Year,
                                birthDate.Value.Month, birthDate.Value.Day);
                        }

                        try
                        {
                            dynamicsContext.AddTocontacts(driverContact);
                            var saveResult = await dynamicsContext.SaveChangesAsync();
                            var tempId = GetCreatedId(saveResult);
                            if (tempId != null)
                            {
                                contactId = tempId.Value;
                                driverContact = dynamicsContext.contacts.ByKey(tempId).GetValue();
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "LegacyCandidateCreate ERROR CREATING Contact Null Person - " + e.Message);
                        }

                        driver.dfp_PersonId = driverContact;
                        try
                        {
                            dynamicsContext.SetLink(driver, nameof(dfp_driver.dfp_PersonId), driverContact);
                            await dynamicsContext.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "LegacyCandidateCreate ERROR Linking Driver Null Person - " + e.Message);
                        }
                    }
                }
            }
            else // create the driver.
            {
                string contactIdString = string.Format("FCBCE0AC-82EF-411D-BD95-DB84D5E3D927");
                string contactSubId =
                    contactIdString.Substring(0, contactIdString.Length - request.DriverLicenseNumber.Length);

                var contactId = new Guid(contactSubId + request.DriverLicenseNumber);

                try
                {
                    driverContact = dynamicsContext.contacts.ByKey(contactId).GetValue();
                }
                catch (Exception)
                {
                    driverContact = null;
                }

                if (driverContact == null)
                {
                    driverContact = new contact()
                    {
                        contactid = contactId,
                        lastname = request.Surname
                    };

                    if (birthDate != null)
                    {
                        driverContact.birthdate = new Microsoft.OData.Edm.Date(birthDate.Value.Year,
                            birthDate.Value.Month, birthDate.Value.Day);
                    }

                    try
                    {
                        await dynamicsContext.SaveChangesAsync();
                        dynamicsContext.AddTocontacts(driverContact);
                        var saveResult2 = await dynamicsContext.SaveChangesAsync();
                        var tempId2 = GetCreatedId(saveResult2);
                        if (tempId2 != null)
                        {
                            contactId = tempId2.Value;
                            driverContact = dynamicsContext.contacts.ByKey(tempId2).GetValue();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "LegacyCandidateCreate ERROR CREATING Contact - " + e.Message);
                    }
                }

                driver = new dfp_driver()
                {
                    dfp_driverid = driverId,
                    dfp_licensenumber = request.DriverLicenseNumber,
                    dfp_PersonId = driverContact,
                    statuscode = 1,
                };
                if (birthDate != null)
                {
                    driver.dfp_dob = birthDate.Value;
                }

                await dynamicsContext.SaveChangesAsync();
                dynamicsContext.AddTodfp_drivers(driver);
                var saveResult = await dynamicsContext.SaveChangesAsync();
                var tempId = GetCreatedId(saveResult);
                if (tempId != null)
                {
                    driverId = tempId.Value;
                    driver = dynamicsContext.dfp_drivers.ByKey(tempId).GetValue();
                }

                dynamicsContext.SetLink(driver, nameof(dfp_driver.dfp_PersonId), driverContact);

                // this save is necessary so that the create incident will work.
                await dynamicsContext.SaveChangesAsync();
            }


            // create the case.
            incident newIncident = new incident()
            {
                // set status to Open Pending for Submission
                statuscode = 100000000,
                casetypecode = 2, // DMER
                // set progress status to in queue, ready for review
                dfp_progressstatus = 100000000,
            };

            if (medicalType != null)
            {
                /*
                   1             Class
                   2             Age
                   3             Industrial Road Endorsement
                   4             Known Condition
                   5             Suspected Condition
                 */
                int? dmerType = null;
                switch (medicalType)
                {
                    case "1": // Class
                        dmerType = 100000000;
                        break;
                    case "2": // Age
                        dmerType = 100000001;
                        break;
                    case "3": // Industrial Road
                        dmerType = 100000002;
                        break;
                    case "4": // Known Condition
                        dmerType = 100000003;
                        break;
                    case "5":
                        dmerType = 100000006; // Suspected
                        break;
                }

                if (dmerType != null)
                {
                    newIncident.dfp_dmertype = dmerType;
                }
            }

            int? sequenceNumber = request.SequenceNumber;

            if (request.SequenceNumber != null)
            {
                newIncident.dfp_dfcmscasesequencenumber = sequenceNumber;
            }
            else
            {
                // get the number of cases that the driver has.

                await dynamicsContext.LoadPropertyAsync(driver, nameof(dfp_driver.dfp_driver_incident_DriverId));

                sequenceNumber = driver.dfp_driver_incident_DriverId.Count + 1;
            }

            if (sequenceNumber == 0)
            {
                sequenceNumber = 1;
            }

            // Check sequence number on case 


            /*
            newIncident.incidentid = CreateIncidentGuid(request.DriverLicenseNumber, sequenceNumber.Value);

            // Check sequence number on case



            int incidentLoop = 0;
            while (IncidentExists(newIncident.incidentid.Value))
            {
                sequenceNumber++;
                newIncident.incidentid = CreateIncidentGuid(request.DriverLicenseNumber, sequenceNumber.Value);

                incidentLoop++;


                if (incidentLoop > 1000)
                {
                    throw new Exception("IncidentLoop count exceeded");
                }
            }

            */


            newIncident.dfp_dfcmscasesequencenumber = sequenceNumber;

            try
            {
                dynamicsContext.AddToincidents(newIncident);

                if (driverContact != null && newIncident._customerid_value != driverContact.contactid)
                {
                    dynamicsContext.SetLink(newIncident, nameof(incident.customerid_contact), driverContact);
                }

                var saveResult = dynamicsContext.SaveChanges();

                var tempId = GetCreatedId(saveResult);
                /*
                if (tempId != null)
                {
                    dynamicsContext.Detach(newIncident);
                    newIncident = dynamicsContext.incidents.ByKey(tempId).GetValue();
                }
                */

                if (newIncident._dfp_driverid_value == null || newIncident._dfp_driverid_value != driver.dfp_driverid)
                {
                    dynamicsContext.SetLink(newIncident, nameof(incident.dfp_DriverId), driver);
                    dynamicsContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"LegacyCandidateCreate {source} ERROR CREATING INCIDENT - " + e.Message);
            }

            dynamicsContext.Detach(newIncident);
        }


        /// <summary>
        /// Legacy Candidate Search
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CaseSearchReply> LegacyCandidateSearch(LegacyCandidateSearchRequest request)
        {
            //search matching cases
            var cases = await SearchLegacyCandidate(dynamicsContext, request);

            //lazy load case related properties
            foreach (var @case in cases)
            {
                await LazyLoadProperties(@case);
            }

            dynamicsContext.DetachAll();

            return MapCases(cases);
        }

        public void DeleteDecision(Guid id)
        {
            dfp_decision itemToDelete =
                dynamicsContext.dfp_decisions.Where(x => x.dfp_decisionid == id).FirstOrDefault();
            if (itemToDelete != null)
            {
                dynamicsContext.DeleteObject(itemToDelete);
                dynamicsContext.SaveChanges();
            }
        }

        public async Task<ResultStatusReply> CreateDecision(CreateDecisionRequest request)
        {
            ResultStatusReply result = new ResultStatusReply();
            result.Success = false;

            var driver = dynamicsContext.dfp_drivers.Where(x => x.dfp_driverid == Guid.Parse(request.DriverId))
                .FirstOrDefault();
            var sparseCase = dynamicsContext.incidents.Where(x => x.incidentid == Guid.Parse(request.CaseId))
                .FirstOrDefault();

            bool reOpenIncident = sparseCase.statecode != null && sparseCase.statecode == 1; // inactive
            int? currentStatus = sparseCase.statuscode;
            if (reOpenIncident)
            {
                dynamicsContext.ActivateObject(sparseCase, 2);
                dynamicsContext.SaveChanges();
                sparseCase = dynamicsContext.incidents.Where(x => x.incidentid == Guid.Parse(request.CaseId))
                    .FirstOrDefault();
            }

            //try
            //{                
            if (request.OutcomeText != null) // only create if there was a decision.
            {
                dfp_decision newDecision = new dfp_decision
                {
                    //createdon = statusDateTime,

                    statecode = 0,
                    statuscode = 1, //100000000, //Final
                    dfp_decisiontype = 100000000, // Original
                    dfp_eligibledriverlicenceclass = null, // NONE
                    // do not set dfp_name as there is a workflow that sets it.
                    dfp_legacydecision = true,
                };

                if (request.StatusDate != null)
                {
                    DateTimeOffset statusDateTime = request.StatusDate.ToUniversalTime();
                    newDecision.overriddencreatedon = statusDateTime;
                }

                Guid outcomeStatusTypeId = Guid.Empty;
                Guid outcomeSubStatusTypeId = Guid.Empty;

                if (request.OutcomeText != null && OutcomeStatusTypes.ContainsKey(request.OutcomeText))
                {
                    outcomeStatusTypeId = OutcomeStatusTypes[request.OutcomeText];
                }


                // now check the decisions.


                bool createDecision = false;
                if (sparseCase.dfp_incident_dfp_decision == null || sparseCase.dfp_incident_dfp_decision.Count == 0
                   )

                {
                    createDecision = true;
                }

                if (!createDecision)
                {
                    bool firstMatch = true;
                    List<Guid> itemsToDelete = new List<Guid>();

                    foreach (var item in sparseCase.dfp_incident_dfp_decision)
                    {
                        if (item.dfp_decisionid != null)
                        {
                            if (item.createdon == newDecision.overriddencreatedon &&
                                item.statecode == newDecision.statecode &&
                                item.statuscode == newDecision.statuscode &&
                                item.dfp_decisiontype == newDecision.dfp_decisiontype &&
                                item.dfp_eligibledriverlicenceclass == newDecision.dfp_eligibledriverlicenceclass &&
                                item._dfp_outcomestatus_value == outcomeStatusTypeId
                               )
                            {
                                if (firstMatch) // keep the first match
                                {
                                    firstMatch = false;
                                }
                                else
                                {
                                    if (!itemsToDelete.Contains(item.dfp_decisionid.Value))
                                    {
                                        itemsToDelete.Add(item.dfp_decisionid.Value);
                                    }
                                }
                            }
                            else // remove it, not a match
                            {
                                if (!itemsToDelete.Contains(item.dfp_decisionid.Value))
                                {
                                    itemsToDelete.Add(item.dfp_decisionid.Value);
                                }
                            }
                        }

                        dynamicsContext.Detach(item);
                    }

                    bool dirty = false;
                    foreach (var deleteDecision in itemsToDelete)
                    {
                        DeleteDecision(deleteDecision);
                        dirty = true;
                    }

                    if (firstMatch)
                    {
                        createDecision = true;
                    }

                    if (dirty)
                    {
                        dynamicsContext.SaveChanges();
                    }
                }

                if (createDecision)
                {
                    newDecision = new dfp_decision
                    {
                        //createdon = statusDateTime,                        
                        statecode = 0,
                        statuscode = 1, //100000000, //Final
                        dfp_decisiontype = 100000000, // Original
                        //dfp_eligibledlclass = "100000000", // NONE
                        // do not set dfp_name as there is a workflow that sets it.
                        dfp_legacydecision = true,
                        //dfp_DriverId = driver
                    };
                    if (request.StatusDate != null)
                    {
                        newDecision.overriddencreatedon = request.StatusDate;
                    }

                    dfp_outcomestatus outcomeStatus = new dfp_outcomestatus();
                    dfp_outcomesubstatus outcomeSubStatus = new dfp_outcomesubstatus();

                    dynamicsContext.AddTodfp_decisions(newDecision);
                    dynamicsContext.SetLink(newDecision, nameof(dfp_decision.dfp_CaseId), sparseCase);
                    dynamicsContext.SetLink(newDecision, nameof(dfp_decision.dfp_DriverId), driver);

                    bool outcomeStatusSet = false;
                    if (request.OutcomeText != null && OutcomeStatusTypes.ContainsKey(request.OutcomeText))
                    {
                        outcomeStatusTypeId = OutcomeStatusTypes[request.OutcomeText];
                        outcomeStatus = dynamicsContext.dfp_outcomestatuses
                            .Where(x => x.dfp_outcomestatusid == outcomeStatusTypeId).FirstOrDefault();

                        dynamicsContext.SetLink(newDecision, nameof(dfp_decision.dfp_OutcomeStatus), outcomeStatus);

                        outcomeStatusSet = true;
                    }

                    bool outcomeSubStatusSet = false;
                    if (request.SubOutcomeText != null && OutcomeStatusTypes.ContainsKey(request.OutcomeText)
                                                       && OutcomeSubStatusTypes.ContainsKey(outcomeStatusTypeId)
                                                       && OutcomeSubStatusTypes[outcomeStatusTypeId]
                                                           .ContainsKey(request.SubOutcomeText))
                    {
                        outcomeSubStatusTypeId = OutcomeSubStatusTypes[outcomeStatusTypeId][request.SubOutcomeText];

                        outcomeSubStatus = dynamicsContext.dfp_outcomesubstatuses
                            .Where(x => x.dfp_outcomesubstatusid == outcomeSubStatusTypeId).FirstOrDefault();

                        dynamicsContext.SetLink(newDecision, nameof(dfp_decision.dfp_OutcomeSubStatus),
                            outcomeSubStatus);
                        //entity.dfp_OutcomeSubStatus = outcomeSubStatus;
                        dynamicsContext.SaveChanges();
                        outcomeSubStatusSet = true;
                    }

                    try
                    {
                        var saveResult = dynamicsContext.SaveChanges();
                        var decisionId = GetCreatedId(saveResult);
                        result.Id = decisionId.ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error adding decision");
                        Console.WriteLine(ex.ToString());
                    }

                    // only close the case if it is open.
                    if (sparseCase.statecode != 1)
                    {
                        // close the incident.
                        var theIncidentResolution = new incidentresolution
                        {
                            incidentid = sparseCase, subject = "Decision Rendered", description = "Decision Rendered"
                        };
                        theIncidentResolution._incidentid_value = sparseCase.incidentid;
                        var closeIncident =
                            dynamicsContext.CloseIncident(theIncidentResolution,
                                5); // no execute so it does not send the request.

                        // construct a special request to close the incident - required because the OData Client is not sending the right parameters.

                        var closeRequest = new HttpRequestMessage(HttpMethod.Post, closeIncident.RequestUri);

                        closeRequest.Headers.Add("OData-MaxVersion", "4.0");
                        closeRequest.Headers.Add("OData-Version", "4.0");
                        closeRequest.Headers.Add("Accept", "application/json");

                        closeRequest.Content = new StringContent(
                            @"{ ""IncidentResolution"": { ""subject"": ""Closed Case - Migration"",
    ""incidentid@odata.bind"": ""/incidents(" + sparseCase.incidentid.Value.ToString() + @")""}, ""Status"": 5 }",
                            Encoding.UTF8, "application/json");

                        HttpClient httpClient = new HttpClient();

                        httpClient.DefaultRequestHeaders.Add("Authorization",
                            $"Bearer {dynamicsContext._tokenFactory().GetAwaiter().GetResult()}");

                        var response = httpClient.SendAsync(closeRequest).GetAwaiter().GetResult();
                    }

                    if (outcomeStatusSet)
                    {
                        dynamicsContext.Detach(outcomeStatus);
                    }

                    if (outcomeSubStatusSet)
                    {
                        dynamicsContext.Detach(outcomeSubStatus);
                    }

                    dynamicsContext.Detach(sparseCase);
                }
            }

            return result;
        }

        public async Task<ResultStatusReply> CreateDriver(CreateDriverRequest request)
        {
            var result = new ResultStatusReply();
            result.Success = false;

            contact driverContact;

            try
            {
                // step 1 : Create driver contact 
                string contactIdString = string.Format("FCBCE0AC-82EF-411D-BD95-DB84D5E3D927");
                string contactSubId =
                    contactIdString.Substring(0, contactIdString.Length - request.DriverLicenseNumber.Length);
                var contactId = new Guid(contactSubId + request.DriverLicenseNumber);

                try
                {
                    driverContact = dynamicsContext.contacts.Where(x => x.contactid == contactId).FirstOrDefault();
                }
                catch (Exception)
                {
                    driverContact = null;
                }

                if (driverContact == null)
                {
                    driverContact = new contact()
                    {
                        contactid = contactId,
                        lastname = request.Surname,
                        firstname = request.GivenName,
                    };

                    if (request.BirthDate != null &&
                        request.BirthDate > new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.Zero))
                    {
                        driverContact.birthdate = new Microsoft.OData.Edm.Date(request.BirthDate.Value.Year,
                            request.BirthDate.Value.Month, request.BirthDate.Value.Day);
                    }

                    dynamicsContext.AddTocontacts(driverContact);
                    var saveResult2 = await dynamicsContext.SaveChangesAsync();
                    var tempId2 = GetCreatedId(saveResult2);
                    if (tempId2 != null)
                    {
                        contactId = tempId2.Value;
                        driverContact = dynamicsContext.contacts.ByKey(tempId2).GetValue();
                    }
                }

                // Step 2 : Create Driver
                string newDriverId = string.Format("e27d7c69-3913-4116-a360-f5e99972b7e8");
                string driverSubId = newDriverId.Substring(0, newDriverId.Length - request.DriverLicenseNumber.Length);
                Guid driverId = Guid.Parse(driverSubId + request.DriverLicenseNumber);

                dfp_driver driver = new dfp_driver()
                {
                    dfp_driverid = driverId,
                    dfp_licensenumber = request.DriverLicenseNumber,
                    statuscode = 1,
                };

                if (request.BirthDate != null &&
                    request.BirthDate > new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.Zero))
                {
                    driver.dfp_dob = request.BirthDate.Value;
                }

                await dynamicsContext.SaveChangesAsync();
                dynamicsContext.AddTodfp_drivers(driver);
                dynamicsContext.SetLink(driver, nameof(dfp_driver.dfp_PersonId), driverContact);

                var saveResult = await dynamicsContext.SaveChangesAsync();
                var tempId = GetCreatedId(saveResult);
                if (tempId != null)
                {
                    driverId = tempId.Value;
                    driver = dynamicsContext.dfp_drivers.ByKey(tempId).GetValue();
                }
                //dynamicsContext.SetLink(driver, nameof(dfp_driver.dfp_PersonId), driverContact);

                // this save is necessary so that the create incident will work.
                await dynamicsContext.SaveChangesAsync();
                result.Success = true;
                result.Id = driverId.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create driver.");
                result.Success = false;
                result.ErrorDetail = "Failed to create driver.";
            }

            return result;
        }

        public async Task<ResultStatusReply> LinkLoginToDriver(Guid loginId, Guid driverId)
        {
            var reply = new ResultStatusReply();

            try
            {
                var driver = dynamicsContext.dfp_drivers
                    .Where(d => d.dfp_driverid == driverId)
                    .First();

                var login = dynamicsContext.dfp_logins
                    .Where(l => l.dfp_loginid == loginId)
                    .Single();

                dynamicsContext.SetLink(login, nameof(dfp_login.dfp_DriverId), driver);
                await dynamicsContext.SaveChangesAsync();

                reply.Success = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to link login to driver.");
                reply.Success = false;
                reply.ErrorDetail = "Failed to link login to driver.";
            }

            return reply;
        }

        private bool IncidentExists(Guid id)
        {
            bool found = false;
            try
            {
                var temp = dynamicsContext.incidents.Where(x => x.incidentid == id).FirstOrDefault();
                if (temp != null)
                {
                    found = true;
                }
            }
            catch (Exception ex)
            {
                found = false;
            }

            return found;
        }

        private Guid CreateIncidentGuid(string driverLicenseNumber, int sequenceNumber)
        {
            string baseGuid = "407f23fb5500ec11b82bfbf5fbf5fbf5";

            int paddedSize = driverLicenseNumber.Length;

            string sequenceString = sequenceNumber.ToString();
            paddedSize += sequenceString.Length;

            string incidentIdString = baseGuid.Substring(0, baseGuid.Length - paddedSize) + driverLicenseNumber +
                                      sequenceString;

            return Guid.Parse(incidentIdString);
        }

        /// <summary>
        /// Create Case
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultStatusReply> CreateCase(CreateCaseRequest request)
        {
            //return case Id in response
            ResultStatusReply result = null;

            dfp_driver driver;

            contact driverContact;

            // Query the driver

            var driverQuery = dynamicsContext.dfp_drivers
                .Where(d => d.dfp_licensenumber == request.DriverLicenseNumber && d.statecode == 0).FirstOrDefault();


            if (driverQuery != null)
            {
                await dynamicsContext.LoadPropertyAsync(driverQuery, nameof(dfp_driver.dfp_PersonId));

                int sequenceNumber;

                if (request.SequenceNumber != null)
                {
                    sequenceNumber = request.SequenceNumber.Value;
                }
                else
                {
                    // get the number of cases that the driver has.

                    await dynamicsContext.LoadPropertyAsync(driverQuery,
                        nameof(dfp_driver.dfp_driver_incident_DriverId));

                    sequenceNumber = driverQuery.dfp_driver_incident_DriverId.Count + 1;
                }

                if (sequenceNumber == 0)
                {
                    sequenceNumber = 1;
                }


                incident newIncident = new incident()
                {
                    // Check the 

                    customerid_contact = driverQuery.dfp_PersonId,
                    // set status to Open Pending for Submission
                    statuscode = 100000000,
                    // use dictionary to translate the codes
                    casetypecode = TranslateCaseType(request.CaseTypeCode ?? request.DocumentType),
                    dfp_progressstatus = 100000000,
                    dfp_dfcmscasesequencenumber = request.SequenceNumber,
                };

                // Check sequence number on case 


                newIncident.incidentid = CreateIncidentGuid(request.DriverLicenseNumber, sequenceNumber);


                bool found = false;
                int incidentLoop = 0;
                while (IncidentExists(newIncident.incidentid.Value))
                {
                    sequenceNumber++;
                    newIncident.incidentid = CreateIncidentGuid(request.DriverLicenseNumber, sequenceNumber);

                    incidentLoop++;


                    if (incidentLoop > 1000)
                    {
                        throw new Exception("IncidentLoop count exceeded");
                    }
                }

                newIncident.dfp_dfcmscasesequencenumber = sequenceNumber;

                try
                {
                    dynamicsContext.AddToincidents(newIncident);

                    if (driverQuery.dfp_PersonId != null &&
                        newIncident._customerid_value != driverQuery.dfp_PersonId.contactid)
                    {
                        dynamicsContext.SetLink(newIncident, nameof(incident.customerid_contact),
                            driverQuery.dfp_PersonId);
                    }

                    var saveResult = await dynamicsContext.SaveChangesAsync();

                    var tempId = GetCreatedId(saveResult);

                    if (tempId != null)
                    {
                        dynamicsContext.Detach(newIncident);
                        newIncident = dynamicsContext.incidents.ByKey(tempId).GetValue();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, $"CandidateCreate ERROR CREATING INCIDENT - " + e.Message);
                }


                try
                {
                    // first check to see that the driver is not already linked.

                    //await dynamicsContext.LoadPropertyAsync(newIncident, nameof(incident.dfp_DriverId));

                    if (newIncident._dfp_driverid_value == null ||
                        newIncident._dfp_driverid_value != driverQuery.dfp_driverid)
                    {
                        dynamicsContext.SetLink(newIncident, nameof(incident.dfp_DriverId), driverQuery);

                        // add a link from driver to incident
                        // dynamicsContext.SetLink(driverQuery, nameof(dfp_driver.dfp_driver_incident_DriverId), newIncident);
                        await dynamicsContext.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, " Candidate Create  {source} ERROR set link incident - driver  " + e.Message);
                }


                dynamicsContext.DetachAll();
            }


            return result;
        }

        /// <summary>
        /// Returns a Dynamics Principal that can be used in a set Owner call.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public principal LookupTeam(string name, string validationPrevious)
        {
            // name can be username IDIR\name or a Team Name like "Adjudicators"
            // this would need to check team first and then the systemuser entity
            principal result = null;

            try
            {
                string translatedOwner = TranslateOwner(name);

                team lookupTeam = dynamicsContext.teams.Where(x => x.name == translatedOwner).FirstOrDefault();

                if (lookupTeam != null)
                {
                    result = lookupTeam;
                }
                else
                {
                    if (validationPrevious != null)
                    {
                        if (!validationPrevious.StartsWith("IDIR\\"))
                        {
                            validationPrevious = $"IDIR\\" + validationPrevious;
                        }

                        systemuser lookupUser = dynamicsContext.systemusers
                            .Where(x => x.domainname == validationPrevious).FirstOrDefault();
                        if (lookupUser != null)
                        {
                            result = lookupUser;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"LookupOwner {name}");
                result = null;
            }

            return result;
        }


        /// <summary>
        /// Translate the Dynamics statuscode (status reason) field to text
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private string TranslateStatus(int? statusCode)
        {
            var statusMap = new Dictionary<int, string>()
            {
                { 1, "In Progress" },
                { 2, "On Hold" },
                { 3, "Waiting on Details" },
                { 4, "Case Created" },
                { 100000000, "Open - Pending Submission" },
                { 5, "Decision Rendered" },
                { 1000, "RSBC Received" },
                { 6, "Canceled" },
                { 2000, "Merged" },
            };

            if (statusCode != null && statusMap.ContainsKey(statusCode.Value))
            {
                return statusMap[statusCode.Value];
            }
            else
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Translate the Dynamics DMER Type field to text
        /// </summary>
        /// <param name="dmerType"></param>
        /// <returns></returns>
        private string TranslateDmerType(int? dmerType)
        {
            var statusMap = new Dictionary<int, string>()
            {
                { 100000000, "Commercial" },
                { 100000001, "Scheduled Age" },
                { 100000002, "Commercial" },
                { 100000003, "Known/Suspected Condition" },
                { 100000004, "Scheduled Routine" },
                { 100000005, "No DMER" }
            };

            if (dmerType != null && statusMap.ContainsKey(dmerType.Value))
            {
                return statusMap[dmerType.Value];
            }
            else
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Translate the Dynamics Priority (status reason) field to text
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private int TranslatePriorityCode(string priorityCode)
        {
            var statusMap = new Dictionary<string, int>()
            {
                { "Regular", 100000000 },
                { "Urgent / Immediate", 100000001 },
                { "Expedited", 100000002 },
                { "Critical Review", 100000003 },
            };

            if (priorityCode != null && statusMap.ContainsKey(priorityCode))
            {
                return statusMap[priorityCode];
            }
            else
            {
                return 100000000;
            }
        }

        /// <summary>
        /// Translate the Submittal Status field to text
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private int TranslateSubmittalStatusString(string submittalStatusCode)
        {
            var statusMap = new Dictionary<string, int>()
            {
                { "Actioned Non-comply", 100000007 },
                { "Clean Pass", 100000009 },
                { "Issued", 100000011 },
                { "Manual Pass", 100000012 },
                { "Non-comply", 100000005 },
                { "Open-Required", 100000000 },
                { "Received", 100000001 },
                { "Rejected", 100000004 },
                { "Reviewed", 100000003 },
                { "Sent", 100000008 },
                { "Uploaded", 100000010 },
            };

            if (submittalStatusCode != null && statusMap.ContainsKey(submittalStatusCode))
            {
                return statusMap[submittalStatusCode];
            }
            else
            {
                return 100000001;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priorityCode"></param>
        /// <returns></returns>
        private int TranslateQueueCode(string queueCode)
        {
            var statusMap = new Dictionary<string, int>()
            {
                { "Team - Intake", 100000000 },
                { "Team - Adjudicators", 100000001 },
                { "Team - Case Managers", 100000002 },
                { "Team Leads Review", 100000000 },
                { "Nurse Case Managers", 100000002 },
                { "Adjudicators", 100000001 },
                { "Client Services", 100000000 },
            };

            if (queueCode != null && statusMap.ContainsKey(queueCode))
            {
                return statusMap[queueCode];
            }
            else
            {
                return 100000000;
            }
        }


        /// <summary>
        ///  convert the owner from the value provided by the legacy system to Dynamics CRM
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        private string TranslateOwner(string owner)
        {
            var statusMap = new Dictionary<string, string>()
            {
                { "Team Leads Review", "Team - Team Lead/Manager" },
                { "Nurse Case Managers", "Team - Nurse Case Manager" },
                { "Adjudicators", "Team - Adjudicator" },
                { "Client Services", "Team - Intake" },
            };

            if (owner != null && statusMap.ContainsKey(owner))
            {
                return statusMap[owner];
            }
            else
            {
                return owner;
            }
        }


        /// <summary>
        /// Translate the Dynamics Priority (status reason) field to text
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private int TranslateCaseType(string documentStatusCode)
        {
            var statusMap = new Dictionary<string, int>()
            {
                { "DMER", 2 }, // DMER
                { "PDR", 3 }, // PDR document
                { "(PDR)PriorityDoctorsReport", 3 },
                { "UNSL", 100000005 }, // Unsolisitated Document
                { "UnsolicitedReportofConcern", 100000005 },
                { "POL", 100000002 }, //Police Report
                { "Police Report", 100000002 },
                { "(POL)-PoliceReport", 100000002 },
                { "OTHR", 100000004 },
            };

            if (documentStatusCode != null && statusMap.ContainsKey(documentStatusCode))
            {
                return statusMap[documentStatusCode];
            }
            else
            {
                // default value is OTHER 
                return statusMap["OTHR"];
            }
        }

        /// <summary>
        /// Search Cases
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<incident>> SearchCases(DynamicsContext ctx, CaseSearchRequest criteria)
        {
            bool notJustDriverLicenseSearch = !string.IsNullOrEmpty(criteria.CaseId) ||
                                              !string.IsNullOrEmpty(criteria.Title) ||
                                              !string.IsNullOrEmpty(criteria.ClinicId);
            bool shouldSearchCases =
                !string.IsNullOrEmpty(criteria.DriverLicenseNumber) ||
                notJustDriverLicenseSearch;

            Guid? driverId = Guid.Empty;
            // pre-search if the driver licence number is a parameter.
            if (!string.IsNullOrEmpty(criteria.DriverLicenseNumber))
            {
                var driverQuery = ctx.dfp_drivers.Where(d => d.dfp_licensenumber == criteria.DriverLicenseNumber);
                var driverResults = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToArray();
                if (driverResults.Length > 0)
                {
                    driverId = driverResults[0].dfp_driverid;
                }
                else if (!notJustDriverLicenseSearch)
                {
                    shouldSearchCases = false;
                }
            }


            if (!shouldSearchCases) return Array.Empty<incident>();

            var caseQuery = ctx.incidents
                .Expand(i => i.dfp_DriverId)
                .Expand(i => i.customerid_contact)
                .Expand(i => i.dfp_ClinicId)
                .Expand(i => i.dfp_MedicalPractitionerId)
                .Expand(i => i.bcgov_incident_bcgov_documenturl)
                .Expand(i => i.dfp_incident_dfp_knownmedicalcondition)
                .Where(i => i.statecode == 1);

            if (!string.IsNullOrEmpty(criteria.CaseId))
            {
                caseQuery = caseQuery.Where(i => i.incidentid == Guid.Parse(criteria.CaseId));
            }
            else
            {
                caseQuery = caseQuery.Where(i => i.casetypecode == (int)CaseTypeOptionSet.DMER);
            }

            if (!string.IsNullOrEmpty(criteria.Title))
                caseQuery = caseQuery.Where(i => i.ticketnumber == criteria.Title);
            if (!string.IsNullOrEmpty(criteria.ClinicId))
                caseQuery = caseQuery.Where(i => i._dfp_clinicid_value == Guid.Parse(criteria.ClinicId));
            if (!string.IsNullOrEmpty(criteria.DriverLicenseNumber))
            {
                // abort the search if there is not an exact match for driver's licence number
                // as we do not want the results to include all records...
                if (driverId != null && driverId != Guid.Empty)
                {
                    return Array.Empty<incident>();
                }
                else
                {
                    caseQuery = caseQuery.Where(i => i._dfp_driverid_value == driverId);
                }
            }

            return (await ((DataServiceQuery<incident>)caseQuery).GetAllPagesAsync()).ToArray();
        }

        /// <summary>
        /// Search Legacy Candidate
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<incident>> SearchLegacyCandidate(DynamicsContext ctx,
            LegacyCandidateSearchRequest criteria)
        {
            var shouldSearchCases =
                !string.IsNullOrEmpty(criteria.DriverLicenseNumber) ||
                !string.IsNullOrEmpty(criteria.Surname);

            if (!shouldSearchCases) return Array.Empty<incident>();

            Guid? driverId = Guid.Empty;
            var driverQuery = ctx.dfp_drivers.Expand(x => x.dfp_PersonId)
                .Where(d => d.dfp_licensenumber == criteria.DriverLicenseNumber).ToList();

            dfp_driver driver = null;

            foreach (var item in driverQuery)
            {
                // ensure the person is loaded.
                await ctx.LoadPropertyAsync(item, nameof(dfp_driver.dfp_PersonId));
                if (item.dfp_PersonId?.lastname != null &&
                    (bool)(item.dfp_PersonId?.lastname.ToUpper().StartsWith(criteria?.Surname.ToUpper())))
                {
                    driver = item;
                    break;
                }
            }


            if (driver != null)
            {
                driverId = driver.dfp_driverid;
                var caseQuery = ctx.incidents
                    .Expand(i => i.dfp_DriverId)
                    .Expand(i => i.customerid_contact)
                    .Expand(i => i.dfp_ClinicId)
                    .Expand(i => i.dfp_MedicalPractitionerId)
                    .Expand(i => i.dfp_incident_dfp_decision)
                    .Where(i => i.casetypecode == (int)CaseTypeOptionSet.DMER && i._dfp_driverid_value == driverId);

                return (await ((DataServiceQuery<incident>)caseQuery).GetAllPagesAsync()).ToArray();
            }
            else
            {
                return Array.Empty<incident>();
            }
        }

        /// <summary>
        /// Search Driver Cases
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<incident>> SearchDriverCases(DynamicsContext ctx,
            CaseSearchRequest criteria)
        {
            var shouldSearchDrivers = !string.IsNullOrEmpty(criteria.DriverLicenseNumber);

            if (!shouldSearchDrivers) return Array.Empty<incident>();

            var driverQuery = ctx.dfp_drivers
                .Expand(d => d.dfp_PersonId)
                .Where(d => d.statecode == (int)EntityState.Active);
            if (!string.IsNullOrEmpty(criteria.DriverLicenseNumber))
                driverQuery = driverQuery.Where(i => i.dfp_licensenumber == criteria.DriverLicenseNumber);
            var drivers = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToArray();
            foreach (var driver in drivers)
            {
                await ctx.LoadPropertyAsync(driver, nameof(dfp_driver.dfp_driver_incident_DriverId));
            }

            return drivers.SelectMany(d => d.dfp_driver_incident_DriverId).ToArray();
        }

        /// <summary>
        /// Get All Flags
        /// </summary>
        /// <returns></returns>
        public async Task<List<Flag>> GetAllFlags()
        {
            List<Flag> result = new List<Flag>();

            var flags = dynamicsContext.dfp_flags.Execute();
            foreach (var flag in flags)
            {
                var newFlag = new Flag()
                {
                    Id = flag.dfp_id,
                    Description = flag.dfp_description,
                    FormId = flag.dfp_formid,
                };
                if (flag.dfp_type != null)
                {
                    newFlag.FlagType = (FlagTypeOptionSet)flag.dfp_type;
                }

                result.Add(newFlag);
            }

            return result;
        }

        /// <summary>
        /// Get All Known Medical Conditions (KMCs)
        /// </summary>
        /// <returns></returns>
        public async Task<List<MedicalCondition>> GetAllMedicalConditions()
        {
            List<MedicalCondition> result = new List<MedicalCondition>();

            var medicalConditions = dynamicsContext.dfp_medicalconditions.Execute();
            foreach (var medicalCondition in medicalConditions)
            {
                var newMedicalCondition = new MedicalCondition()
                {
                    Id = medicalCondition.dfp_id,
                    Description = medicalCondition.dfp_description,
                    FormId = medicalCondition.dfp_formid
                };

                result.Add(newMedicalCondition);
            }

            return result;
        }

        /// <summary>
        /// Get Unsent Medical Updates for clean pass and manual pass
        /// </summary>
        /// <returns></returns>
        public async Task<CaseSearchReply> GetUnsentMedicalPass()
        {
            Log.Information("Start GetUnsentMedicalPass");
            var outputArray = new List<incident>();

            DataServiceQueryContinuation<incident> nextLink = null;

            var caseQuery = (DataServiceQuery<incident>)dynamicsContext.incidents
                .Where(i => i.statecode == 0 // Active
                            && i.dfp_datesenttoicbc == null
                            && i.dfp_bpfstage == 100000003
                            && i._dfp_driverid_value != null
                ); // Case should be in File End Tasks (FET)


            var response = caseQuery.Execute() as QueryOperationResponse<incident>;

            Log.Information($"Response : {response != null}");

            do
            {
                if (nextLink != null)
                {
                    response = dynamicsContext.Execute<incident>(nextLink) as QueryOperationResponse<incident>;
                }

                if (response != null)
                {
                    // You must enumerate the response before calling GetContinuation below.
                    foreach (var item in response)
                    {
                        // Log.Information(JsonConvert.SerializeObject(item));

                        if (item._dfp_driverid_value.HasValue)
                        {
                            //load driver info
                            await dynamicsContext.LoadPropertyAsync(item, nameof(incident.dfp_DriverId));
                            if (item.dfp_DriverId != null)
                                await dynamicsContext.LoadPropertyAsync(item.dfp_DriverId,
                                    nameof(incident.dfp_DriverId.dfp_PersonId));
                        }

                        // Load documents
                        await dynamicsContext.LoadPropertyAsync(item,
                            nameof(incident.bcgov_incident_bcgov_documenturl));

                        // Check for manual pass document if it exists break

                        bool hasManualPassDocument = false;

                        bool addItem = false;

                        foreach (var document in item.bcgov_incident_bcgov_documenturl)
                        {
                            await dynamicsContext.LoadPropertyAsync(document, nameof(document.dfp_DocumentTypeID));

                            Log.Information(
                                $"{item.dfp_DriverId.dfp_licensenumber}, {item.incidentid.Value}, {item.ticketnumber} {document.bcgov_documenturlid.Value}, {document.dfp_submittalstatus}");

                            if (document.dfp_submittalstatus == (int)submittalStatusOptionSet.ManualPass)
                            {
                                hasManualPassDocument = true;
                                break;
                            }

                            // Condition Check if the document is DMER and not manual pass
                            //Updating code on 2 / 29 / 2024 removed the check for manual pass
                            if (
                                document.dfp_DocumentTypeID != null
                                && document.dfp_DocumentTypeID.dfp_name == "DMER"
                                && document.dfp_submittalstatus == (int)submittalStatusOptionSet.CleanPass
                            )
                            {
                                addItem = true;
                            }
                        }

                        if (hasManualPassDocument == false)
                        {
                            //load decisions
                            await dynamicsContext.LoadPropertyAsync(item, nameof(incident.dfp_incident_dfp_decision));
                            if (item.dfp_incident_dfp_decision.Count > 0)
                            {
                                foreach (var decision in item.dfp_incident_dfp_decision)
                                {
                                    //await dynamicsContext.LoadPropertyAsync(decision, nameof(dfp_decision.dfp_decisionid));
                                    if (decision._dfp_outcomestatus_value != null)
                                        await dynamicsContext.LoadPropertyAsync(decision,
                                            nameof(dfp_decision.dfp_OutcomeStatus));
                                }

                                addItem = true;
                            }

                            if (addItem)
                            {
                                outputArray.Add(item);
                            }
                        }
                    }
                }
            }
            // Loop if there is a next link
            while ((nextLink = response.GetContinuation()) != null);

            dynamicsContext.DetachAll();
            Log.Information("End GetUnsentMedicalPass");

            return MapCases(outputArray);
        }

        /// <summary>
        /// Get Unsent Medical Updates for Adjudication
        /// </summary>
        /// <returns></returns>
        public async Task<CaseSearchReply> GetUnsentMedicalAdjudication()
        {
            Log.Information("Start GetUnsentMedicalAdjudication");
            var outputArray = new List<incident>();

            DataServiceQueryContinuation<incident> nextLink = null;

            var caseQuery = (DataServiceQuery<incident>)dynamicsContext.incidents
                .Where(i => i.statecode == 0 // Active
                            && i.dfp_datesenttoicbc == null
                            && i.dfp_bpfstage == 100000002
                            && i._dfp_driverid_value != null
                ); // case is in under review);

            // Get the document of type DMER from documents entity and not in Rejected, Clean Pass, or Manual Pass(Query document entity)
            // Add condition to check document is in review (Document entity)
            //

            var response = caseQuery.Execute() as QueryOperationResponse<incident>;

            do
            {
                if (nextLink != null)
                {
                    response = dynamicsContext.Execute<incident>(nextLink) as QueryOperationResponse<incident>;
                }

                if (response != null)
                {
                    // You must enumerate the response before calling GetContinuation below.
                    foreach (var item in response)
                    {
                        if (item._dfp_driverid_value.HasValue)
                        {
                            //load driver info
                            await dynamicsContext.LoadPropertyAsync(item, nameof(incident.dfp_DriverId));
                            if (item.dfp_DriverId != null)
                                await dynamicsContext.LoadPropertyAsync(item.dfp_DriverId,
                                    nameof(incident.dfp_DriverId.dfp_PersonId));
                        }


                        // Load documents

                        await dynamicsContext.LoadPropertyAsync(item,
                            nameof(incident.bcgov_incident_bcgov_documenturl));

                        bool hasManualPassDocument = false;


                        foreach (var document in item.bcgov_incident_bcgov_documenturl)
                        {
                            if (document.dfp_submittalstatus == (int)submittalStatusOptionSet.ManualPass)
                            {
                                hasManualPassDocument = true;
                                Log.Information("MANUAL PASS TRUE");
                                break;
                            }
                        }

                        if (!hasManualPassDocument)
                        {
                            foreach (var document in item.bcgov_incident_bcgov_documenturl)
                            {
                                await dynamicsContext.LoadPropertyAsync(document, nameof(document.dfp_DocumentTypeID));

                                Log.Information(
                                    $"{item.dfp_DriverId.dfp_licensenumber} typeID {document.dfp_DocumentTypeID} Ticket {item.ticketnumber} Submittal Status {document.dfp_submittalstatus}");
                                // condition 1: check for
                                // 1. DMER type
                                // 2. submital status is in review and not in  Rejected, Clean Pass
                                // 3. Updating code on 2/21/2024 removed the check for manual pass

                                if (document.dfp_DocumentTypeID != null
                                    && document.dfp_DocumentTypeID.dfp_name == "DMER"
                                    && document.dfp_submittalstatus != null
                                    && document.dfp_submittalstatus != (int)submittalStatusOptionSet.Empty
                                    && document.dfp_submittalstatus != (int)submittalStatusOptionSet.CleanPass
                                    && document.dfp_submittalstatus != (int)submittalStatusOptionSet.ManualPass
                                    && document.dfp_submittalstatus != (int)submittalStatusOptionSet.Reject
                                    && document.dfp_submittalstatus != (int)submittalStatusOptionSet.Uploaded)
                                {
                                    outputArray.Add(item);
                                }

                                //condition 2: Check for
                                //1. DMER type
                                //2. Submital status is clean pass and is in review state
                                //3. Updating code on 2/21/2024 removed the check for manual pass

                                else if (document.dfp_DocumentTypeID != null
                                         && document.dfp_DocumentTypeID.dfp_name == "DMER"
                                         && document.dfp_submittalstatus == (int)submittalStatusOptionSet.CleanPass
                                        )
                                {
                                    outputArray.Add(item);
                                }

                                /* 24/4/18 - this code is not used at this time, left for a placeholder in the future if there is need for additional criteria.

                                else
                                {
                                    // condition 3 : Check for
                                    // 1. Document type is DMER
                                    // 2. Submital status is Uploaded

                                    if (document.dfp_DocumentTypeID != null
                                          && document.dfp_DocumentTypeID.dfp_name == "DMER"
                                          && (document.dfp_submittalstatus == (int)submittalStatusOptionSet.Uploaded)
                                          )
                                    {
                                        // This is an empty case for now and will be implemented in future for DMER when the document is uploaded state it should add the J flag
                                        //outputArray.Add(item);
                                    }

                                }

                                */
                            }
                        }
                    }
                }
            }
            // Loop if there is a next link
            while ((nextLink = response.GetContinuation()) != null);

            dynamicsContext.DetachAll();
            Log.Information("End GetUnsentMedicalAdjudication");

            return MapCases(outputArray);
        }

        /// <summary>
        /// Get Dps Processing Date
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset GetDpsProcessingDate()
        {
            var mostRecentRecord = dynamicsContext.bcgov_documenturls
                .Where(x => x.dfp_processdate != null)
                .OrderByDescending(i => i.dfp_processdate)
                .Take(1)
                .FirstOrDefault();

            if (mostRecentRecord != null)
            {
                return mostRecentRecord.dfp_processdate.Value;
            }
            else
            {
                return DateTimeOffset.UtcNow;
            }
        }

        /// <summary>
        /// Get Dps Uploaded Date
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset GetDpsUploadedDate()
        {
            // new calculation for DPS processing Date
            // 1. Check DocumentStatus = Uplaoded 100,000,010
            // 2. Check DPS class is 9-General
            // 3. Document Business are not equal to remedial
            // 4. Order By oldest uploaded date from the result of the filter 
            // DFTDP # 977 Change made on 05/02/2024
            // 5. Exclude  "ERA Result" and "Road Test - ICBC Re-exams" they should have the current Date 06/04/2024
            // Get the guid for the 

            Guid? eraResultsGuid = null;

            var tempQuery = dynamicsContext.dfp_submittaltypes.Where(x => x.dfp_name == "ERA Results").FirstOrDefault();
            if (tempQuery != null)
            {
                eraResultsGuid = tempQuery.dfp_submittaltypeid.Value;
            }


            Guid? reExamResultsGuid = null;

            tempQuery = dynamicsContext.dfp_submittaltypes.Where(x => x.dfp_name == "Road Test - ICBC Re-exam")
                .FirstOrDefault();
            if (tempQuery != null)
            {
                reExamResultsGuid = tempQuery.dfp_submittaltypeid.Value;
            }

            var mostRecentRecord = dynamicsContext.bcgov_documenturls
                .Expand(x => x.dfp_DocumentTypeID)
                .Where(x => x.dfp_uploadeddate != null
                            && x.dfp_submittalstatus == 100000010 // Document status Uplaoded
                            && x.dfp_dpsclass.Contains("9 - General") // Check for 9 - General
                            && x.dfp_DocumentTypeID.dfp_businessarea != 100000001 // Does not contain remedial
                            && x._dfp_documenttypeid_value != eraResultsGuid
                            && x._dfp_documenttypeid_value !=
                            reExamResultsGuid) // Exclude ERA Results and Road Test - ICBC Re-exam docuemnt types
                .OrderBy(i => i.dfp_uploadeddate) // order by oldest uploaded date 
                .Take(1)
                .FirstOrDefault();

            if (mostRecentRecord != null)
            {
                return mostRecentRecord.dfp_uploadeddate.Value;
            }
            else
            {
                return DateTimeOffset.UtcNow;
            }
        }


        /// <summary>
        /// Update NonComply Documents
        /// </summary>
        /// <returns></returns>
        public async Task UpdateNonComplyDocuments()
        {
            // This Calculated Date calculation is applicable to all documents except ERA results and Road Test - ICBC Re-exam

            var dpsUploadedDate = GetDpsUploadedDate();

            //Path 1 : This is for all the documents except ERA results and Road Test
            var querydocuments = from bcgov_documenturl
                    in dynamicsContext.bcgov_documenturls
                where bcgov_documenturl.dfp_submittalstatus ==
                      100000000 // Open Required                                                  
                      && bcgov_documenturl.dfp_compliancedate < dpsUploadedDate
                select bcgov_documenturl;


            DataServiceCollection<bcgov_documenturl> nonComplyDocuments =
                new DataServiceCollection<bcgov_documenturl>(querydocuments);

            UpdateNonComplyDocumentsQuery(nonComplyDocuments);

            // Path 2 : This is to set non comply on  ERA results and Road Test
            // This Current Date is applicable to only  ERA results and Road Test - ICBC Re-exam
            var dpsUploadDateEraResults = DateTimeOffset.UtcNow;

            if (dpsUploadDateEraResults != null)
            {
                var queryEraResults = from bcgov_documenturl
                        in dynamicsContext.bcgov_documenturls
                    where bcgov_documenturl.dfp_submittalstatus == 100000000 // Open Required
                          && (bcgov_documenturl.dfp_DocumentTypeID.dfp_name == "ERA Results" ||
                              bcgov_documenturl.dfp_DocumentTypeID.dfp_name == "Road Test - ICBC Re-exam")
                          && bcgov_documenturl.dfp_compliancedate < dpsUploadDateEraResults
                    select bcgov_documenturl;


                DataServiceCollection<bcgov_documenturl> nonComplyDocuments2 =
                    new DataServiceCollection<bcgov_documenturl>(queryEraResults);

                UpdateNonComplyDocumentsQuery(nonComplyDocuments2);
            }
        }

        private async void UpdateNonComplyDocumentsQuery(DataServiceCollection<bcgov_documenturl> nonComplyDocuments)
        {
            List<Guid> documentIds = new List<Guid>();
            foreach (var item in nonComplyDocuments)
            {
                documentIds.Add(item.bcgov_documenturlid.Value);
                dynamicsContext.Detach(item); // needed in order to allow the incident to be attached below
            }

            foreach (var documentId in documentIds)
            {
                var changedDocument = new bcgov_documenturl
                {
                    bcgov_documenturlid = documentId,
                    dfp_submittalstatus = 100000005
                };
                dynamicsContext.AttachTo("bcgov_documenturls", changedDocument);
                dynamicsContext.UpdateObject(changedDocument);
            }

            DataServiceResponse response = null;
            try
            {
                response = dynamicsContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, $"CMS.CaseManager UpdateNonComplyDocuments  ex.Message");
            }

            dynamicsContext.DetachAll();
        }

        /// <summary>
        /// Method to set the resolve case status
        /// </summary>
        /// <returns></returns>
        public async Task ResolveCaseStatusUpdates()
        {
            // 04/25/2024 Removing the check for dfp_bpfstage is FET as this field is inconsistant and this check is being done on mercury end
            var currentDate = DateTimeOffset.UtcNow;

            var query = from incident
                    in dynamicsContext.incidents
                where (incident.dfp_caseresolvedate != null
                       && incident.dfp_caseresolvedate <= currentDate
                       && incident.statecode == 0)
                      || incident.dfp_immediateclosure == true
                select incident;

            DataServiceCollection<incident> resolveCases = new DataServiceCollection<incident>(query);

            List<Guid> ids = new List<Guid>();
            foreach (var item in resolveCases)
            {
                ids.Add(item.incidentid.Value);
                dynamicsContext.Detach(item); // needed in order to allow the incident to be attached below
            }

            foreach (var id in ids)
            {
                var changedIncident = new incident
                {
                    incidentid = id,
                    dfp_resolvecase = true
                };
                dynamicsContext.AttachTo("incidents", changedIncident);
                dynamicsContext.UpdateObject(changedIncident);
            }

            DataServiceResponse response = null;
            try
            {
                response = dynamicsContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, $"CMS.CaseManager ResolveCaseStatusUpdates  ex.Message");
            }

            dynamicsContext.DetachAll();
        }


        /// <summary>
        /// Set Case Resolve Date
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="resolvedDate"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SetCaseResolveDate(string caseId, DateTimeOffset resolvedDate)
        {
            logger.LogInformation($"SetCaseResolveDate - looking for DMER with identifier {caseId} {resolvedDate}");

            // future state - the case name will contain three letters of the name and the driver licence number

            incident dmerEntity = dynamicsContext.incidents.ByKey(Guid.Parse(caseId)).GetValue();

            if (dmerEntity != null && resolvedDate != null)
            {
                dmerEntity.dfp_caseresolvedate = resolvedDate;

                try
                {
                    dynamicsContext.UpdateObject(dmerEntity);
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.DetachAll();
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"SetCaseResolveDate - Error updating");
                }
            }
        }


        /// <summary>
        /// Update Clean Pass Flag
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultStatusReply> UpdateCleanPassFlag(string caseId)
        {
            ResultStatusReply result = new ResultStatusReply()
            {
                Success = false
            };

            try
            {
                // string id = caseId;

                if (caseId != null && caseId != string.Empty)
                {
                    var currentCase = dynamicsContext.incidents.ByKey(Guid.Parse(caseId)).GetValue();

                    if (currentCase != null && currentCase.statecode == 0)
                    {
                        await dynamicsContext.LoadPropertyAsync(currentCase,
                            nameof(incident.bcgov_incident_bcgov_documenturl));

                        if (currentCase.bcgov_incident_bcgov_documenturl != null)
                        {
                            foreach (var document in currentCase.bcgov_incident_bcgov_documenturl)
                            {
                                await dynamicsContext.LoadPropertyAsync(document, nameof(document.dfp_DocumentTypeID));


                                if (document.dfp_DocumentTypeID != null && document.statecode == 0
                                                                        && document.dfp_submittalstatus ==
                                                                        (int)submittalStatusOptionSet.CleanPass)
                                {
                                    if (document.dfp_DocumentTypeID != null &&
                                        document.dfp_DocumentTypeID.dfp_name != null &&
                                        document.dfp_DocumentTypeID.dfp_name == "DMER")
                                    {
                                        bool detach = dynamicsContext.Detach(currentCase);
                                        var changedIncident = new incident()
                                        {
                                            incidentid = currentCase.incidentid,
                                            dfp_iscleanpass = true
                                        };
                                        dynamicsContext.AttachTo("incidents", changedIncident);
                                        dynamicsContext.UpdateObject(changedIncident);
                                        result.Success = true;
                                    }
                                }
                            }

                            try
                            {
                                dynamicsContext.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Error(ex, $"CMS.CaseManager Update Clean Pass  ex.Message");
                            }

                            dynamicsContext.DetachAll();
                        }
                    }
                }
            }

            catch (Exception e)
            {
                logger.LogError(e, $"Update Clean Pass Flag - Error updating");
                result.ErrorDetail = e.Message;
            }

            return result;
        }


        /// <summary>
        /// Set Case Status
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> SetCleanPassFlag(string caseId, bool cleanPassStatus)
        {
            // get the case
            incident @case = GetIncidentById(caseId);
            try
            {
                if (@case.statecode == 0)
                {
                    @case.dfp_iscleanpass = cleanPassStatus;

                    dynamicsContext.UpdateObject(@case);
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.DetachAll();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"SetCleanPassStatus - Error updating");
            }

            return true;
        }

        /// <summary>
        /// Update Manual Pass Flag
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultStatusReply> UpdateManualPassFlag(string caseId)
        {
            ResultStatusReply result = new ResultStatusReply()
            {
                Success = false
            };

            try
            {
                //string caseId = request.CaseId;

                if (caseId != null && caseId != string.Empty)
                {
                    var currentCase = dynamicsContext.incidents.ByKey(Guid.Parse(caseId)).GetValue();

                    if (currentCase != null && currentCase.statecode == 0)
                    {
                        await dynamicsContext.LoadPropertyAsync(currentCase,
                            nameof(incident.bcgov_incident_bcgov_documenturl));

                        if (currentCase.bcgov_incident_bcgov_documenturl != null)
                        {
                            foreach (var document in currentCase.bcgov_incident_bcgov_documenturl)
                            {
                                await dynamicsContext.LoadPropertyAsync(document, nameof(document.dfp_DocumentTypeID));

                                //await dynamicsContext.LoadPropertyAsync(pdfDocument, nameof(pdfDocument.dfp_DocumentTypeID));


                                if (document.dfp_DocumentTypeID != null && document.statecode == 0
                                                                        && document.dfp_submittalstatus ==
                                                                        (int)submittalStatusOptionSet.ManualPass
                                   )
                                {
                                    if (document.dfp_DocumentTypeID != null &&
                                        document.dfp_DocumentTypeID.dfp_name != null &&
                                        document.dfp_DocumentTypeID.dfp_name == "DMER")
                                    {
                                        bool detach = dynamicsContext.Detach(currentCase);
                                        var changedIncident = new incident()
                                        {
                                            incidentid = currentCase.incidentid,
                                        };

                                        dynamicsContext.AttachTo("incident", changedIncident);
                                        changedIncident.dfp_ismanualpass = true;
                                        dynamicsContext.UpdateObject(changedIncident);
                                        try
                                        {
                                            dynamicsContext.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            Serilog.Log.Error(ex, $"CMS.CaseManager Update Manual Pass  {ex.Message}");
                                        }

                                        result.Success = true;
                                    }
                                }
                            }


                            dynamicsContext.DetachAll();
                        }
                    }
                }
            }

            catch (Exception e)
            {
                logger.LogError(e, $"Update Manual Pass Flag - Error updating");
                result.ErrorDetail = e.Message;
            }

            return result;
        }

        /// <summary>
        /// Set Manual Pass Status
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> SetManualPassFlag(string caseId, bool manualPassStatus)
        {
            // get the case
            incident @case = GetIncidentById(caseId);
            try
            {
                if (@case.statecode == 0)
                {
                    @case.dfp_ismanualpass = manualPassStatus;

                    dynamicsContext.UpdateObject(@case);
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.DetachAll();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"SetManualPassStatus - Error updating");
            }

            return true;
        }

        /// <summary>
        /// GetListOfLettersSentToBcMail
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PdfDocument>> GetPdfDocuments()
        {
            // Call the documents and get the list of documents in "Sent To BC Mail" status

            List<PdfDocument> result = new List<PdfDocument>();

            try
            {
                var pdfDocuments = dynamicsContext.dfp_pdfdocuments.Where(
                    d =>
                        d.statuscode == (int)StatusCodeOptionSet.SendToBCMail // PDF documents in Send to BC mail Status
                        && d.statecode == 0).ToList();


                int count = 0;

                foreach (var pdfDocument in pdfDocuments)
                {
                    count++;
                    if (pdfDocument != null)
                    {
                        // get the associated document.

                        var document = dynamicsContext.bcgov_documenturls
                            .Where(x => x._dfp_pdfdocumentid_value == pdfDocument.dfp_pdfdocumentid).FirstOrDefault();

                        if (document != null)
                        {
                            await dynamicsContext.LoadPropertyAsync(pdfDocument,
                                nameof(pdfDocument.dfp_DocumentTypeID));

                            await dynamicsContext.LoadPropertyAsync(pdfDocument, nameof(pdfDocument.dfp_CaseId));

                            await dynamicsContext.LoadPropertyAsync(pdfDocument, nameof(pdfDocument.dfp_DriverId));


                            string filename = count.ToString();

                            if (pdfDocument.dfp_DriverId?.dfp_licensenumber == null ||
                                pdfDocument.dfp_CaseId?.title == null)
                            {
                                //Log.Information("Info - PDF record has document with no driver ");
                                Log.Information(
                                    $"No driver record available for case {pdfDocument?.dfp_CaseId?.title}");
                            }
                            else
                            {
                                var driverLicenceNumber = pdfDocument.dfp_DriverId.dfp_licensenumber;

                                filename = $"{driverLicenceNumber}-{filename}-DMF";
                            }

                            filename += ".pdf";

                            PdfDocument pdfDoc = new PdfDocument()
                            {
                                PdfDocumentId = pdfDocument.dfp_pdfdocumentid.ToString(),
                                //StateCode = pdfDocument.statecode,
                                Filename = filename,
                                ServerUrl = document.bcgov_url,
                                StatusCode = (StatusCodeOptionSet)pdfDocument.statuscode
                            };
                            result.Add(pdfDoc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"GetPdfDocuments");
            }

            return result;
        }


        /// <summary>
        /// Method to set the resolve case status
        /// </summary>
        /// <returns></returns>
        public async Task<Guid> CreatePdfDocument(PdfDocument pdfDocumentRequest)
        {
            PdfDocumentReply result = new PdfDocumentReply()
            {
                Success = false
            };

            dfp_pdfdocument newDoc = new dfp_pdfdocument()
            {
                statuscode = (int)pdfDocumentRequest.StatusCode
            };

            dynamicsContext.AddTodfp_pdfdocuments(newDoc);


            DataServiceResponse saveResult = dynamicsContext.SaveChanges();

            return GetCreatedId(saveResult).Value;
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
            {
            }


            return result;
        }

        /// <summary>
        /// Method to set the resolve case status
        /// </summary>
        /// <returns></returns>
        public async Task<PdfDocumentReply> UpdatePdfDocumentStatus(PdfDocument pdfDocumentRequest)
        {
            PdfDocumentReply result = new PdfDocumentReply()
            {
                Success = false
            };

            try
            {
                dfp_pdfdocument pdfDocument = dynamicsContext.dfp_pdfdocuments
                    .ByKey(Guid.Parse(pdfDocumentRequest.PdfDocumentId)).GetValue();

                if (pdfDocument != null)
                {
                    // status to SEND or Failed TO Send
                    pdfDocument.statuscode = (int)pdfDocumentRequest.StatusCode;

                    // 

                    dynamicsContext.UpdateObject(pdfDocument);
                    await dynamicsContext.SaveChangesAsync();
                    result.Success = true;
                }

                dynamicsContext.DetachAll();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Update Document Status - Error updating");
            }

            return result;
        }

        /// <summary>
        /// Translate the Dynamics Priority (status reason) field to text
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private int TranslatePdfDocumentStatus(string submittalStatusCode)
        {
            var statusMap = new Dictionary<string, int>()
            {
                { "Send To BCMail", 100000002 }, // Received
                { "Sent", 100000005 }, // Rejected
                { "Failed to Send", 100000006 },
            };

            if (submittalStatusCode != null && statusMap.ContainsKey(submittalStatusCode))
            {
                return statusMap[submittalStatusCode];
            }
            else
            {
                return 100000002;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ResultStatusReply> UpdateBirthDate(UpdateDriverRequest driverRequest)
        {
            ResultStatusReply result = new ResultStatusReply()
            {
                Success = false
            };
            var driverQuery = dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId)
                .Where(x => x.dfp_licensenumber == driverRequest.DriverLicenseNumber);

            var data = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToList();


            try
            {
                foreach (var driver in data)
                {
                    dynamicsContext.LoadProperty(driver, nameof(dfp_driver.dfp_PersonId));

                    contact driverContact;

                    if (driver.dfp_PersonId != null)
                    {
                        driverContact = driver.dfp_PersonId;
                        driverContact.birthdate = driverRequest.BirthDate;
                        dynamicsContext.UpdateObject(driverContact);
                        await dynamicsContext.SaveChangesAsync();
                        result.Success = true;
                    }

                    dynamicsContext.Detach(driver);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"UpdateBirthdate - Error updating");
                result.ErrorDetail = e.Message;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ResultStatusReply> UpdateDriver(Driver driver)
        {
            bool written = false; // if true, we have writtent to Dynamics

            ResultStatusReply result = new ResultStatusReply()
            {
                Success = false, ErrorDetail = string.Empty
            };
            var driverQuery = dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId)
                .Where(x => x.dfp_licensenumber == driver.DriverLicenseNumber);

            var data = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToList();
            dfp_driver[] driverResults = data.ToArray();

            try
            {
                if (driverResults.Length > 0)
                {
                    foreach (var item in driverResults)
                    {
                        // ensure the contact information exists.

                        var id = item.dfp_driverid;

                        dynamicsContext.LoadProperty(item, nameof(dfp_driver.dfp_PersonId));

                        contact driverContact;

                        if (item.dfp_PersonId != null)
                        {
                            var contactId = item.dfp_PersonId.contactid;

                            dynamicsContext.Detach(item.dfp_PersonId);

                            driverContact = new contact()
                            {
                                contactid = contactId,
                                firstname = driver.GivenName,
                                lastname = driver.Surname,
                                birthdate = driver.BirthDate
                            };
                            dynamicsContext.AttachTo("contacts", driverContact);
                            dynamicsContext.UpdateObject(driverContact);

                            written = true;
                            try
                            {
                                await dynamicsContext.SaveChangesAsync();
                                result.Success = true;
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e, $"UpdateDriver - Error Save Changes Update Contact");
                                result.ErrorDetail = e.Message;
                                result.Success = false;
                            }
                        }


                        dynamicsContext.Detach(item);

                        var updateDriver = new dfp_driver
                        {
                            dfp_driverid = id,
                            dfp_fullname = driver.DriverLicenseNumber + " - " + driver.Surname
                        };
                        dynamicsContext.AttachTo("dfp_drivers", updateDriver);
                        dynamicsContext.UpdateObject(updateDriver);

                        try
                        {
                            await dynamicsContext.SaveChangesAsync();
                            result.Success = true;
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, $"UpdateDriver - Error Save Changes Update Driver");
                            result.ErrorDetail = e.Message;
                            result.Success = false;
                        }
                    }
                }

                if (written)
                {
                    dynamicsContext.DetachAll();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"UpdateDriver - Generic Error updating");
                result.ErrorDetail = e.Message;
            }

            return result;
        }

        /// <summary>
        /// Set Case Status
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> SetCaseStatus(string caseId, bool caseStatus)
        {
            // get the case
            incident @case = GetIncidentById(caseId);

            @case.dfp_resolvecase = caseStatus;

            try
            {
                dynamicsContext.UpdateObject(@case);
                await dynamicsContext.SaveChangesAsync();
                dynamicsContext.DetachAll();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"SetCaseStatus - Error updating");
            }

            return true;
        }


        public async Task SwitchTo8Dl()
        {
            // get all the drivers.

            var drivers = dynamicsContext.dfp_drivers.Where(x => x.dfp_licensenumber != null).ToList();
            int i = 0;
            foreach (var driver in drivers)
            {
                if (driver.dfp_licensenumber.Length == 7)
                {
                    // zero pad
                    driver.dfp_licensenumber = "0" + driver.dfp_licensenumber;
                    dynamicsContext.UpdateObject(driver);
                    i++;
                }

                if (driver.dfp_licensenumber.Length == 8)
                {
                    // zero pad
                    driver.dfp_licensenumber = driver.dfp_licensenumber.Substring(1);
                    dynamicsContext.UpdateObject(driver);
                    i++;
                }

                if (i % 1000 == 0)
                {
                    await dynamicsContext.SaveChangesAsync();
                }
            }

            await dynamicsContext.SaveChangesAsync();
            dynamicsContext.DetachAll();
        }

        public async Task MakeFakeDls()
        {
            for (int i = 0; i < 1150000; i++)
            {
                int fakeDl = 2000000 + i;
                string dl = fakeDl.ToString();
                var driver = new dfp_driver()
                {
                    dfp_licensenumber = fakeDl.ToString(),
                    dfp_dob = DateTime.Now,
                    dfp_fullname = $"FAKE {dl}"
                };
                dynamicsContext.AddTodfp_drivers(driver);
                if (i % 500 == 0)
                {
                    await dynamicsContext.SaveChangesAsync();
                }
            }

            await dynamicsContext.SaveChangesAsync();
        }


        /// <summary>
        /// Mark Medical Updates Sent
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task MarkMedicalUpdatesSent(List<string> ids)
        {
            DateTimeOffset dateSent = DateTimeOffset.UtcNow;
            foreach (var id in ids)
            {
                var dmerEntity = dynamicsContext.incidents.Where(x => x.incidentid == Guid.Parse(id)).FirstOrDefault();
                if (dmerEntity != null)
                {
                    dmerEntity.dfp_datesenttoicbc = dateSent;
                    dynamicsContext.UpdateObject(dmerEntity);
                }
            }

            await dynamicsContext.SaveChangesAsync();
            dynamicsContext.DetachAll();
        }


        /// <summary>
        /// Mark Medical Update Error
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<ResultStatusReply> MarkMedicalUpdateError(IcbcErrorRequest request)
        {
            ResultStatusReply result = new ResultStatusReply()
            {
                Success = false
            };

            string caseId = request.CaseId;
            string errorMessage = request.ErrorMessage;
            if (!string.IsNullOrEmpty(caseId))
            {
                try
                {
                    var dmerEntity = dynamicsContext.incidents.Where(x => x.incidentid == Guid.Parse(caseId))
                        .FirstOrDefault();
                    bool reOpenIncident = dmerEntity.statecode == 1; // inactive
                    int? currentStatus = dmerEntity.statuscode;
                    if (reOpenIncident)
                    {
                        dynamicsContext.ActivateObject(dmerEntity, 2);
                        await dynamicsContext.SaveChangesAsync();
                    }


                    if (dmerEntity != null)
                    {
                        // Update the error message in CMS
                        dmerEntity.dfp_icbcerrorlog = errorMessage;
                    }

                    dynamicsContext.UpdateObject(dmerEntity);
                    await dynamicsContext.SaveChangesAsync();

                    if (reOpenIncident)
                    {
                        dynamicsContext.DeactivateObject(dmerEntity, currentStatus.Value);
                        await dynamicsContext.SaveChangesAsync();
                    }

                    dynamicsContext.DetachAll();
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    Log.Logger.Error(ex, ex.Message);
                    result.ErrorDetail = ex.Message;
                }
            }

            return result;
        }


        /// <summary>
        /// Add Document Url To Case If Not Exist
        /// </summary>
        /// <param name="dmerIdentifier"></param>
        /// <param name="fileKey"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public async Task AddDocumentUrlToCaseIfNotExist(string dmerIdentifier, string fileKey, Int64 fileSize)
        {
            // add links to documents.
            incident dmerEntity = dynamicsContext.incidents.ByKey(Guid.Parse(dmerIdentifier))
                .Expand(x => x.bcgov_incident_bcgov_documenturl).GetValue();

            if (dmerEntity != null)
            {
                if (dmerEntity.bcgov_incident_bcgov_documenturl.Count(x => x.bcgov_url == fileKey) == 0)
                {
                    // add the document url.
                    bcgov_documenturl givenUrl = dynamicsContext.bcgov_documenturls.Where(x => x.bcgov_url == fileKey)
                        .FirstOrDefault();
                    string filename;
                    if (fileKey.LastIndexOf("/") != -1)
                    {
                        filename = fileKey.Substring(fileKey.LastIndexOf("/") + 1);
                    }
                    else
                    {
                        filename = fileKey;
                    }

                    string extension;
                    if (fileKey.LastIndexOf(".") != -1)
                    {
                        extension = fileKey.Substring(fileKey.LastIndexOf("."));
                    }
                    else
                    {
                        extension = "";
                    }

                    if (givenUrl == null)
                    {
                        var dateUploaded = DateTimeOffset.Now;
                        givenUrl = new bcgov_documenturl()
                        {
                            bcgov_url = fileKey,
                            bcgov_receiveddate = dateUploaded,
                            dfp_uploadeddate = dateUploaded,
                            bcgov_filename = filename,
                            bcgov_fileextension = extension,
                            bcgov_origincode = 931490000,
                            bcgov_filesize = HumanReadableFileLength(fileSize)
                        };

                        await dynamicsContext.SaveChangesAsync();
                        dynamicsContext.AddTobcgov_documenturls(givenUrl);

                        var saveResult = await dynamicsContext.SaveChangesAsync();
                        var tempId = GetCreatedId(saveResult);
                        if (tempId != null)
                        {
                            givenUrl = dynamicsContext.bcgov_documenturls.ByKey(tempId).GetValue();
                        }
                    }

                    dynamicsContext.AddLink(dmerEntity, nameof(incident.bcgov_incident_bcgov_documenturl), givenUrl);
                }

                dynamicsContext.SaveChanges();
            }
        }

        /// <summary>
        /// Convert a file length to a string for display in Dynamics.
        /// </summary>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        private static string HumanReadableFileLength(Int64 byteCount)
        {
            string[] suffix = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //suffix for file size
            if (byteCount == 0)
                return "0" + " " + suffix[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return Math.Sign(byteCount) * num + " " + suffix[place];
        }

        /// <summary>
        /// Sanitize Label
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        private string SanitizeLabel(string label)
        {
            const int maxFieldLength = 200;
            string result = null;
            if (label != null)
            {
                if (label.Length > maxFieldLength)
                {
                    result = label.Substring(0, maxFieldLength); // Max 200 chars
                }
                else
                {
                    result = label;
                }
            }

            return result;
        }

        /// <summary>
        /// Set Case Flags
        /// </summary>
        /// <param name="dmerIdentifier"></param>
        /// <param name="isCleanPass"></param>
        /// <param name="flags"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<SetCaseFlagsReply> SetCaseFlags(string dmerIdentifier, bool isCleanPass, List<Flag> flags,
            Microsoft.Extensions.Logging.ILogger logger = null)
        {
            if (logger == null) logger = this.logger;
            /* The structure for cases is

            Case (incident) is the parent item
                - has children which are flag entities

             */

            int flagCount = flags == null ? 0 : flags.Count;

            logger.LogInformation($"SetCaseFlags - looking for DMER with identifier {dmerIdentifier} {flagCount}");

            // future state - the case name will contain three letters of the name and the driver licence number

            incident dmerEntity = dynamicsContext.incidents.ByKey(Guid.Parse(dmerIdentifier))
                .Expand(x => x.dfp_incident_dfp_dmerflag).GetValue();

            if (dmerEntity != null)
            {
                // close and re-open the case
                dynamicsContext.CloseIncident(new incidentresolution()
                {
                    incidentid = dmerEntity
                }, -1);

                dmerEntity.statecode = 0;
                dmerEntity.statuscode = 1;

                // clean pass is indicated by the presence of flags.
                logger.LogInformation($"SetCaseFlags - found DMER with identifier {dmerIdentifier}");

                // Explicitly load the flags
                dynamicsContext.LoadProperty(dmerEntity, nameof(incident.dfp_incident_dfp_dmerflag));
                if (dmerEntity.dfp_incident_dfp_dmerflag != null && dmerEntity.dfp_incident_dfp_dmerflag.Count > 0)
                {
                    foreach (var item in dmerEntity.dfp_incident_dfp_dmerflag)
                    {
                        // remove the old bridge.
                        dynamicsContext.DeleteObject(item);

                        //dmerEntity.dfp_incident_dfp_flag.
                        logger.LogInformation($"SetCaseFlags - removing flag {item.dfp_name}");
                    }
                }

                // Add the flags.

                foreach (var flag in flags)
                {
                    logger.LogInformation($"SetCaseFlags - starting update of flag {flag.Id} - {flag.Description}");
                    dfp_flag givenFlag = dynamicsContext.dfp_flags.Where(x => x.dfp_id == flag.Id).FirstOrDefault();
                    if (givenFlag == null)
                    {
                        givenFlag = new dfp_flag()
                        {
                            dfp_id = flag.Id,
                            dfp_description = flag.Description,
                            dfp_label = SanitizeLabel(flag.Description), // max 200 characters.
                            dfp_type = (int?)FlagTypeOptionSet.Review,
                            dfp_formid = flag.FormId
                        };
                        dynamicsContext.AddTodfp_flags(givenFlag);
                    }
                    else // may need to update
                    {
                        if (givenFlag.dfp_description != flag.Description)
                        {
                            givenFlag.dfp_description = flag.Description;
                            dynamicsContext.UpdateObject(givenFlag);
                        }

                        if (givenFlag.dfp_label == null || givenFlag.dfp_label != SanitizeLabel(flag.Description))
                        {
                            givenFlag.dfp_label = SanitizeLabel(flag.Description); // max 200 characters.
                            dynamicsContext.UpdateObject(givenFlag);
                        }
                    }

                    // configure the bridge entity

                    dfp_dmerflag newFlag = new dfp_dmerflag()
                    {
                        dfp_name = SanitizeLabel(flag.Description), // max 200 characters.,
                        dfp_description = flag.Description
                    };

                    dynamicsContext.AddTodfp_dmerflags(newFlag);
                    dynamicsContext.AddLink(dmerEntity, nameof(incident.dfp_incident_dfp_dmerflag), newFlag);
                    dynamicsContext.SetLink(newFlag, nameof(dfp_dmerflag.dfp_FlagId), givenFlag);

                    logger.LogInformation($"SetCaseFlags - Added Flag {flag}");
                }

                // indicate that the form has been filled out
                //dmerEntity.statuscode = 4; // Researching - was // 100000003; // Completed

                dmerEntity.dfp_iscleanpass = isCleanPass;

                dynamicsContext.UpdateObject(dmerEntity);

                try
                {
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.DetachAll();

                    return new SetCaseFlagsReply { Success = true };
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"SetCaseFlags - Error updating");
                }
            }
            else
            {
                logger.LogError($"SetCaseFlags - Unable to find DMER with identifier {dmerIdentifier}");
            }

            dynamicsContext.DetachAll();

            return new SetCaseFlagsReply { Success = false };
        }

        /// <summary>
        /// Set Case Practitioner Clinic
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="practitionerId"></param>
        /// <param name="clinicId"></param>
        /// <returns></returns>
        public async Task SetCasePractitionerClinic(string caseId, string practitionerId, string clinicId)
        {
            logger.LogInformation(
                $"SetCasePractitionerClinic - looking for DMER with identifier {caseId} {practitionerId} {clinicId}");

            // future state - the case name will contain three letters of the name and the driver licence number

            incident dmerEntity = dynamicsContext.incidents.ByKey(Guid.Parse(caseId)).GetValue();

            if (dmerEntity != null)
            {
                // set the Practitioner

                if (!string.IsNullOrEmpty(practitionerId))
                {
                    // set the Practitioner
                    dfp_medicalpractitioner medicalpractitioner =
                        dynamicsContext.dfp_medicalpractitioners.ByKey(Guid.Parse(caseId)).GetValue();

                    dynamicsContext.SetLink(dmerEntity, nameof(incident.dfp_MedicalPractitionerId),
                        medicalpractitioner);
                }

                if (!string.IsNullOrEmpty(clinicId))
                {
                    // set the Clinic
                    account clinic = dynamicsContext.accounts.ByKey(Guid.Parse(caseId)).GetValue();

                    dynamicsContext.SetLink(dmerEntity, nameof(incident.dfp_ClinicId), clinic);
                }

                dynamicsContext.UpdateObject(dmerEntity);

                try
                {
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.DetachAll();
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"SetCasePractitionerClinic - Error updating");
                }
            }
        }

        /// <summary>
        /// Translate Gender Code to string
        /// </summary>
        /// <param name="gendercode"></param>
        /// <returns></returns>
        private string TranslateGenderCode(int? gendercode)
        {
            string result = string.Empty;

            switch (gendercode)
            {
                case 1:
                    result = "Male";
                    break;
                case 2:
                    result = "Female";
                    break;
            }

            return result;
        }

        /// <summary>
        /// Translate Comment Type Code FromInt
        /// </summary>
        /// <param name="commentTypeCode"></param>
        /// <returns></returns>
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
    }

    /// <summary>
    /// Enum for Case Type Option set
    /// </summary>
    internal enum CaseTypeOptionSet
    {
        DMER = 2, // DMER
        UNSL = 100000005, // Unsolocitated document
        PDR = 3, // PDR Document
        POL = 100000002 // Police Report
    }

    /// <summary>
    /// Enum for Flag Type Option Set
    /// </summary>
    public enum FlagTypeOptionSet
    {
        Submittal = 100000000,
        Review = 100000001,
        FollowUp = 100000002,
        Message = 100000003
    }
}