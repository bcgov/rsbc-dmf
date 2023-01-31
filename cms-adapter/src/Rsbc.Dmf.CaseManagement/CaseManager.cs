using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Rsbc.Dmf.CaseManagement
{
    public interface ICaseManager
    {
        Task<CaseSearchReply> CaseSearch(CaseSearchRequest request);

        Task<CreateStatusReply> CreateLegacyCaseComment(LegacyComment request);

        Task<CreateStatusReply> CreateLegacyCaseDocument(LegacyDocument request);

        Task<bool> DeleteComment(string commentId);

        Task<bool> DeleteLegacyDocument(string documentId);
        
        Task<IEnumerable<LegacyComment>> GetDriverLegacyComments(string driverLicenseNumber, bool allComments);

        Task<LegacyComment> GetComment(string commentId);

        Task<IEnumerable<LegacyComment>> GetCaseLegacyComments(string caseId, bool allComments);

        Task<IEnumerable<LegacyDocument>> GetCaseLegacyDocuments(string caseId);

        Task<IEnumerable<LegacyDocument>> GetDriverLegacyDocuments(string driverLicenseNumber);

        Task<LegacyDocument> GetLegacyDocument(string documentId);

        Task<ResultStatusReply> CreateBringForward(BringForwardRequest request);

        Task<IEnumerable<Driver>> GetDriver(string licensenumber);

        Task<IEnumerable<Driver>> GetDrivers();

        Task<CaseSearchReply> LegacyCandidateSearch(LegacyCandidateSearchRequest request);

        /// <summary>
        /// Create a Legacy Candidate
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Guid of the created case</returns>
        Task<Guid?> LegacyCandidateCreate(LegacyCandidateSearchRequest request, DateTimeOffset? birthDate, DateTimeOffset? effectiveDate);

        Task MarkMedicalUpdatesSent(List<string> ids);

        Task<ResultStatusReply> MarkMedicalUpdateError(IcbcErrorRequest request);

        Task<SetCaseFlagsReply> SetCaseFlags(string dmerIdentifier, bool isCleanPass, List<Flag> flags, Microsoft.Extensions.Logging.ILogger logger = null);

        Task SetCasePractitionerClinic(string caseId, string practitionerId, string clinicId);

        Task<List<Flag>> GetAllFlags();

        Task<CaseSearchReply> GetUnsentMedicalUpdates();

        Task AddDocumentUrlToCaseIfNotExist(string dmerIdentifier, string fileKey, Int64 fileSize);

        DateTimeOffset GetDpsProcessingDate();

        Task UpdateNonComplyDocuments();

        Task ResolveCaseStatusUpdates();

        Task SetCaseResolveDate(string caseId, DateTimeOffset resolvedDate);

        Task <bool> SetCaseStatus(string caseId , bool caseStatus);  
    }
       

    public class CaseSearchRequest
    {
        public string CaseId { get; set; }
        public string Title { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string ClinicId { get; set; }        
    }

    public class LegacyCandidateSearchRequest
    {
        public string DriverLicenseNumber {  get; set;}
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
    }


    public class LegacyDocument
    {     
        public int? SequenceNumber { get; set; }    
        
        public string DocumentTypeCode { get; set; }

        public string DocumentType { get; set; }

        public string BusinessArea { get; set; }

        public string DocumentUrl { get; set; }
        public long FileSize { get; set; }
        public string UserId { get; set; }
        public string CaseId { get; set; }
        public DateTimeOffset FaxReceivedDate { get; set; }
        public DateTimeOffset ImportDate { get; set; }
        public string DocumentId { get; set; }
        public string ImportId { get; set; }
        public Driver Driver { get; set; }

        public string BatchId { get; set; }
        public string OriginatingNumber { get; set; }
        public int DocumentPages { get; set; }
        public string ValidationMethod { get; set; }
        public string ValidationPrevious { get; set; }
    }

    public class CreateStatusReply
    {
        public string Id;
        public bool Success { get; set; }
    }

    public class CaseSearchReply
    {
        public IEnumerable<Case> Items { get; set; }
    }

    public class SetCaseFlagsReply
    {
        public bool Success { get; set; }
    }

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

        public string ClinicName { get; set;}

        public string DmerType { get; set;}
    }

    public class Flag
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public FlagTypeOptionSet? FlagType { get; set; }
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
        public DateTimeOffset CreatedOn {  get; set;}
        public DecisionOutcome? Outcome { get; set; }
    }

    public class BringForwardRequest
    {
        public string CaseId { get; set; }
        public string Assignee{ get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public BringForwardPriority? Priority { get; set; }
        
    }

    public class IcbcErrorRequest
    {
        public string CaseId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum BringForwardPriority
    {
        Low = 0,
        Normal = 1,
        High = 2

    }

    public class ResultStatusReply
    {
        public string Id;
        public bool Success { get; set; }
    }

    internal class CaseManager : ICaseManager
    {
        private readonly DynamicsContext dynamicsContext;
        private readonly ILogger<CaseManager> logger;

        public CaseManager(DynamicsContext dynamicsContext, ILogger<CaseManager> logger)
        {
            this.dynamicsContext = dynamicsContext;
            this.logger = logger;
        }

        /// <summary>
        /// Lazy Load Properties
        /// </summary>
        /// <param name="case"></param>
        /// <returns></returns>
        private async Task LazyLoadProperties(incident @case)
        {
            //load clinic details (assuming customer as clinic for now)
            if (@case.customerid_contact == null) await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.customerid_contact));

            if (@case._dfp_driverid_value.HasValue)
            {
                //load driver info
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_DriverId));
                if (@case.dfp_DriverId != null) await dynamicsContext.LoadPropertyAsync(@case.dfp_DriverId, nameof(incident.dfp_DriverId.dfp_PersonId));
            }

            if (@case._dfp_medicalpractitionerid_value.HasValue)
            {
                //load driver info
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_MedicalPractitionerId));
                if (@case.dfp_MedicalPractitionerId != null) await dynamicsContext.LoadPropertyAsync(@case.dfp_MedicalPractitionerId, nameof(incident.dfp_MedicalPractitionerId.dfp_PersonId));
            }

            //load case's flags
            await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_dmerflag));
            foreach (var flag in @case.dfp_incident_dfp_dmerflag)
            {
                await dynamicsContext.LoadPropertyAsync(flag, nameof(dfp_dmerflag.dfp_FlagId));
            }

            //load decisions
            await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_decision));
            foreach (var decision in @case.dfp_incident_dfp_decision)
            {
                await dynamicsContext.LoadPropertyAsync(decision, nameof(dfp_decision.dfp_decisionid));
                if (decision.dfp_OutcomeStatus != null) await dynamicsContext.LoadPropertyAsync(decision.dfp_OutcomeStatus, nameof(dfp_decision.dfp_OutcomeStatus));
            }

            // load owner
           /* if (@case._ownerid_value.HasValue)
            {
                //load driver info
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.ownerid));
            }*/
        }

        public async Task<CaseSearchReply> CaseSearch(CaseSearchRequest request)
        {
            //search matching cases
            var cases = (await SearchCases(dynamicsContext, request)).Concat(await SearchDriverCases(dynamicsContext, request));

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
        public async Task<IEnumerable<LegacyComment>> GetCaseLegacyComments(string caseId, bool allComments)
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
                    // ignore inactive.
                    if (comment.statecode != null && comment.statecode == 0)
                    {
                        await dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.dfp_commentid));
                        if (allComments || comment.dfp_icbc.GetValueOrDefault())
                        {
                            LegacyComment legacyComment = new LegacyComment
                            {
                                CaseId = @case.incidentid.ToString(),
                                CommentDate = comment.createdon.GetValueOrDefault(),
                                CommentId = comment.dfp_commentid.ToString(),
                                CommentText = comment.dfp_commentdetails,
                                CommentTypeCode = TranslateCommentTypeCodeFromInt(comment.dfp_commenttype),
                                SequenceNumber = @case.importsequencenumber.GetValueOrDefault(),
                                UserId = comment.dfp_userid
                            };
                            result.Add(legacyComment);
                        }
                    }
                        
                }
            }
               
            return result;
        }

        /// <summary>
        /// Get Case Legacy Documents
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyDocument>> GetCaseLegacyDocuments(string caseId)
        {
            List<LegacyDocument> result = new List<LegacyDocument>();
            
            var casesRaw = dynamicsContext.incidents.Where(i => i.incidentid == Guid.Parse(caseId));
            if (casesRaw != null)
            {
                var @cases = casesRaw.ToList();
                foreach (var @case in @cases)
                {
                    // ensure related data is loaded.
                    await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_DriverId));
                    await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.bcgov_incident_bcgov_documenturl));

                    Driver driver = new Driver()
                    {
                        DriverLicenseNumber = @case.dfp_DriverId?.dfp_licensenumber ?? string.Empty,
                    };
                    if (@case.bcgov_incident_bcgov_documenturl != null)
                    {
                        foreach (var document in @case.bcgov_incident_bcgov_documenturl)
                        {
                            // ignore inactive.
                            if (document.statecode == 0)
                            {
                                await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.bcgov_documenturlid));
                                await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.dfp_DocumentTypeID));

                                LegacyDocument legacyDocument = new LegacyDocument
                                {
                                    BatchId = document.dfp_batchid ?? string.Empty,
                                    CaseId = @case.incidentid.ToString(),
                                    DocumentPages = ConvertPagesToInt(document.dfp_documentpages),
                                    DocumentId = document.bcgov_documenturlid.ToString(),
                                    DocumentTypeCode = document.dfp_DocumentTypeID?.dfp_name ?? string.Empty,
                                    DocumentUrl = document.bcgov_url ?? string.Empty,
                                    FaxReceivedDate = document.dfp_faxreceiveddate.GetValueOrDefault(),
                                    ImportDate = document.dfp_dpsprocessingdate.GetValueOrDefault(),
                                    ImportId = document.dfp_importid ?? string.Empty,
                                    OriginatingNumber = document.dfp_faxsender ?? string.Empty,
                                    ValidationMethod = document.dfp_validationmethod ?? string.Empty,
                                    ValidationPrevious = document.dfp_validationprevious ?? string.Empty,
                                    SequenceNumber = @case.importsequencenumber.GetValueOrDefault(),
                                    Driver = driver
                                };

                                result.Add(legacyDocument);
                            }
                            
                        }
                    }                    
                }
            }
            
            return result;
        }

        /// <summary>
        /// Convert Pages To Int
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int ConvertPagesToInt (string data)
        {
            int result = 0;
            if (!int.TryParse (data, out result))
            {
                result = 0;
            }
            return result;

        }

        /// <summary>
        /// Get Driver Legacy Comments
        /// </summary>
        /// <param name="driverLicenceNumber"></param>
        /// <param name="allComments"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyComment>> GetDriverLegacyComments(string driverLicenceNumber, bool allComments )
        {
            List<LegacyComment> result = new List<LegacyComment>();
            // start by the driver

            var @drivers = dynamicsContext.dfp_drivers.Where(d => d.dfp_licensenumber == driverLicenceNumber && d.statuscode == 1).ToList();

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
                    var @cases = dynamicsContext.incidents.Where(i => i._dfp_driverid_value == driverItem.dfp_driverid
                    ).ToList();

                    foreach (var @case in @cases)
                    {
                        // ensure related data is loaded.

                        await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_comment));
                        await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_DriverId));

                        foreach (var comment in @case.dfp_incident_dfp_comment)
                        {
                            await dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.dfp_commentid));
                            if (allComments || comment.dfp_icbc.GetValueOrDefault())
                            {                                
                                LegacyComment legacyComment = new LegacyComment
                                {
                                    CaseId = @case.incidentid.ToString(),
                                    CommentDate = comment.createdon.GetValueOrDefault(),
                                    CommentId = comment.dfp_commentid.ToString(),
                                    CommentText = comment.dfp_commentdetails,
                                    CommentTypeCode = TranslateCommentTypeCodeFromInt (comment.dfp_commenttype),
                                    SequenceNumber = @case.importsequencenumber.GetValueOrDefault(),
                                    UserId = comment.dfp_userid,
                                    Driver = driver
                                };
                                result.Add(legacyComment);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get Driver Legacy Documents
        /// </summary>
        /// <param name="driverLicenceNumber"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyDocument>> GetDriverLegacyDocuments(string driverLicenceNumber)
        {
            List<LegacyDocument> result = new List<LegacyDocument>();
            // start by the driver

            var driversRaw = dynamicsContext.dfp_drivers.Where(d => d.dfp_licensenumber == driverLicenceNumber && d.statecode == 0);

            if (driversRaw != null)
            {
                var drivers = driversRaw.ToList();
                foreach (var @driver in drivers)
                {
                    if (@driver != null)
                    {
                        /*
                        // .Expand(x => x.dfp_incident_dfp_comment)
                        // get the cases for that driver.
                        var @cases = dynamicsContext.incidents.Where(i => i._dfp_driverid_value == @driver.dfp_driverid
                        ).ToList();

                        foreach (var @case in @cases)
                        {
                            // ensure related data is loaded.
                            await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_DriverId));

                            if (@case.dfp_DriverId != null)
                            {
                                await dynamicsContext.LoadPropertyAsync(@case.dfp_DriverId, nameof(dfp_driver.dfp_PersonId));
                            }

                            Driver caseDriver = new Driver()
                            {
                                DriverLicenseNumber = @case.dfp_DriverId?.dfp_licensenumber,
                                Surname = @case.dfp_DriverId?.dfp_PersonId?.lastname ?? string.Empty
                            };

                            await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.bcgov_incident_bcgov_documenturl));
                            foreach (var document in @case.bcgov_incident_bcgov_documenturl)
                            {
                                // only include documents that have a URL
                                if (! string.IsNullOrEmpty(document.bcgov_url))                               
                                {
                                    await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.bcgov_documenturlid));
                                    await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.dfp_DocumentTypeID));

                                    LegacyDocument legacyDocument = new LegacyDocument
                                    {
                                        BatchId = document.dfp_batchid ?? string.Empty,
                                        CaseId = @case.incidentid.ToString(),
                                        DocumentPages = ConvertPagesToInt(document.dfp_documentpages),
                                        DocumentId = document.bcgov_documenturlid.ToString(),
                                        DocumentTypeCode = document.dfp_DocumentTypeID?.dfp_apidocumenttype ?? string.Empty,
                                        DocumentType = document.dfp_DocumentTypeID?.dfp_name ?? string.Empty,
                                        BusinessArea = ConvertBusinessAreaToString(document.dfp_DocumentTypeID?.dfp_businessarea),
                                        DocumentUrl = document.bcgov_url ?? string.Empty,
                                        FaxReceivedDate = document.dfp_faxreceiveddate.GetValueOrDefault(),
                                        ImportDate = document.dfp_dpsprocessingdate.GetValueOrDefault(),
                                        ImportId = document.dfp_importid ?? string.Empty,
                                        OriginatingNumber = document.dfp_faxsender ?? string.Empty,
                                        ValidationMethod = document.dfp_validationmethod ?? string.Empty,
                                        ValidationPrevious = document.dfp_validationprevious ?? string.Empty,
                                        SequenceNumber = @case.importsequencenumber.GetValueOrDefault(),
                                        Driver = caseDriver
                                    };

                                    result.Add(legacyDocument);
                                }                       
                            }
                        }
                        */


                        var driverDocuments = dynamicsContext.bcgov_documenturls.Where(d => d._dfp_driverid_value == driver.dfp_driverid && d.statecode == 0).ToList();
                        foreach (var document in driverDocuments)
                        {
                            // only include documents that have a URL
                            if (!string.IsNullOrEmpty(document.bcgov_url))
                            {
                                await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.bcgov_documenturlid));
                                await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.dfp_DocumentTypeID));


                                LegacyDocument legacyDocument = new LegacyDocument
                                {
                                    BatchId = document.dfp_batchid ?? string.Empty,
                                    
                                    DocumentPages = ConvertPagesToInt(document.dfp_documentpages),
                                    DocumentId = document.bcgov_documenturlid.ToString(),
                                    DocumentTypeCode = document.dfp_DocumentTypeID?.dfp_apidocumenttype ?? string.Empty,
                                    DocumentType = document.dfp_DocumentTypeID?.dfp_name ?? string.Empty,
                                    BusinessArea = ConvertBusinessAreaToString(document.dfp_DocumentTypeID?.dfp_businessarea),
                                    DocumentUrl = document.bcgov_url ?? string.Empty,
                                    FaxReceivedDate = document.dfp_faxreceiveddate.GetValueOrDefault(),
                                    ImportDate = document.dfp_dpsprocessingdate.GetValueOrDefault(),
                                    ImportId = document.dfp_importid ?? string.Empty,
                                    OriginatingNumber = document.dfp_faxsender ?? string.Empty,
                                    ValidationMethod = document.dfp_validationmethod ?? string.Empty,
                                    ValidationPrevious = document.dfp_validationprevious ?? string.Empty,
                                    
                                };

                                Driver caseDriver = new Driver()
                                {
                                    DriverLicenseNumber = driverLicenceNumber,

                                };

                                if (document._bcgov_caseid_value != null)
                                {
                                    var @case = dynamicsContext.incidents.Where(i => i.incidentid == document._bcgov_caseid_value).FirstOrDefault();

                                    if (@case != null)

                                    {
                                        legacyDocument.CaseId = @case.incidentid.ToString();
                                        if (@case.dfp_DriverId != null)
                                        {
                                            await dynamicsContext.LoadPropertyAsync(@case.dfp_DriverId, nameof(dfp_driver.dfp_PersonId));
                                            caseDriver.Surname = @case.dfp_DriverId?.dfp_PersonId?.lastname ?? string.Empty;
                                        }                                       
                                    }
                                }

                                legacyDocument.Driver = caseDriver;

                                //if (! result.Contains(legacyDocument))
                                //{
                                    result.Add(legacyDocument);
                                //}
                            }
                        }
                    }
                }
            }


            return result;
        }

        /// <summary>
        /// Convert String To Business Area
        /// </summary>
        /// <param name="businessArea"></param>
        /// <returns></returns>
        private int? ConvertStringToBusinessArea(string businessArea)
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
        /// Convert Business Area To String
        /// </summary>
        /// <param name="businessArea"></param>
        /// <returns></returns>
        private string ConvertBusinessAreaToString(int? businessArea)
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

        public IEnumerable<dfp_driver> GetDriverObjects(string licensenumber)
        {
            var result = dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d => d.statuscode == 1 && d.dfp_licensenumber == licensenumber).ToList();   
            return result;
        }

        /// <summary>
        /// Get Driver
        /// </summary>
        /// <param name="licensenumber"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Driver>> GetDriver(string licensenumber)
        {
            List<Driver> result = new List<Driver>();

            var @drivers = GetDriverObjects(licensenumber);

            foreach (var item in @drivers)
            {
                Driver d = new Driver()
                {
                    Id = item.dfp_driverid.ToString(),
                    DriverLicenseNumber = item.dfp_licensenumber,
                    Surname = item.dfp_PersonId?.lastname
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

            var @drivers = dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d => d.statuscode == 1).ToList();

            foreach (var item in @drivers)
            {
                Driver d = new Driver ()
                {
                    DriverLicenseNumber = item.dfp_licensenumber,
                    Surname = item.dfp_PersonId?.lastname
                };
                result.Add(d);
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
                var comment = dynamicsContext.dfp_comments.Where(d => d.dfp_commentid == Guid.Parse(commentId)).FirstOrDefault();
                if (comment != null)
                {
                    legacyComment = new LegacyComment
                    {
                        CaseId = comment._dfp_caseid_value.ToString(),
                        CommentDate = comment.createdon.GetValueOrDefault(),
                        CommentId = comment.dfp_commentid.ToString(),
                        CommentText = comment.dfp_commentdetails,
                        CommentTypeCode = TranslateCommentTypeCodeFromInt(comment.dfp_commenttype),
                        SequenceNumber = null,
                        UserId = comment.dfp_userid,
                        Driver = new Driver() // TODO - fetch driver
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error (ex, $"Error getting comment {commentId}");
            }
            

            return legacyComment;

        }

        /// <summary>
        /// Get Legacy Document
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<LegacyDocument> GetLegacyDocument(string documentId)
        {
            LegacyDocument legacyDocument  = null;
            
            var document = dynamicsContext.bcgov_documenturls.Where(d => d.bcgov_documenturlid == Guid.Parse(documentId)).FirstOrDefault();
            if (document != null)
            {
                legacyDocument = new LegacyDocument
                {
                    BatchId = document.dfp_batchid ?? string.Empty,
                    CaseId = null,
                    DocumentPages = ConvertPagesToInt(document.dfp_documentpages),
                    DocumentId = document.bcgov_documenturlid.ToString(),
                    DocumentTypeCode = document.dfp_DocumentTypeID?.dfp_name ?? string.Empty,
                    DocumentUrl = document.bcgov_url ?? string.Empty,
                    FaxReceivedDate = document.dfp_faxreceiveddate.GetValueOrDefault(),
                    ImportDate = document.dfp_dpsprocessingdate.GetValueOrDefault(),
                    ImportId = document.dfp_importid ?? string.Empty,
                    OriginatingNumber = document.dfp_faxsender ?? string.Empty,
                    ValidationMethod = document.dfp_validationmethod ?? string.Empty,
                    ValidationPrevious = document.dfp_validationprevious ?? string.Empty,
                    SequenceNumber = null,
                    Driver = null
                };
            }

            return legacyDocument;

        }

        /// <summary>
        /// Add BringForward to Task Entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                    
                   
                };
                // Get the case
                var @case = GetIncidentById(caseId);

                // Create a bring Forward
                try
                {
                    dynamicsContext.AddTotasks(newTask);
                    // set Case Id
                    dynamicsContext.SetLink(newTask, nameof(task.regardingobjectid_incident), @case);
                    // set the Assignee
                    if (string.IsNullOrEmpty(request.Assignee) && @case.ownerid != null) 
                    {
                        
                        // set the assignee to case owner
                        dynamicsContext.SetLink(newTask, nameof(task.ownerid),@case.ownerid );

                    };
                   
                    await dynamicsContext.SaveChangesAsync();
                    result.Success = true;
                    //result.Id = newTask.regardingobjectid_incident.ToString();
                    dynamicsContext.DetachAll();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    Log.Logger.Error(ex.Message);
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
                    result = dynamicsContext.incidents.Where(d => d.incidentid == Guid.Parse(id)).FirstOrDefault();
                    // ensure the driver is fetched.
                    LazyLoadProperties(result).GetAwaiter().GetResult();
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
                Success = false
            };            
            string caseId = request.CaseId;
            if (string.IsNullOrEmpty(caseId))
            {
                // create a new case.
                LegacyCandidateSearchRequest newCandidate = new LegacyCandidateSearchRequest()
                {
                     DriverLicenseNumber = request.Driver.DriverLicenseNumber,
                     SequenceNumber = null,
                     Surname = request.Driver.Surname
                };

                await LegacyCandidateCreate(newCandidate, request.Driver.BirthDate, DateTimeOffset.MinValue);
                
                // now do a search to get the case.
                var searchResult = await LegacyCandidateSearch(newCandidate);
                var firstCase = searchResult.Items.FirstOrDefault();
                if (firstCase != null)
                {
                    caseId = firstCase.Id;
                }

            }
            if (!string.IsNullOrEmpty(caseId))
            {
                // get the case
                incident @case = GetIncidentById(caseId);

                var driver = @case.dfp_DriverId;

                // create the comment
                dfp_comment comment = new dfp_comment()
                {
                    createdon = request.CommentDate,
                    dfp_commenttype = TranslateCommentTypeCodeToInt(request.CommentTypeCode),
                    dfp_icbc = request.CommentTypeCode == "W" || request.CommentTypeCode == "I",
                    dfp_userid = request.UserId,
                    dfp_commentdetails = request.CommentText, 
                    dfp_date = request.CommentDate  
                };

                try
                {
                    dynamicsContext.AddTodfp_comments(comment);

                    dynamicsContext.SetLink(comment, nameof(dfp_comment.dfp_DriverId), driver);
                    dynamicsContext.AddLink(@case, nameof(incident.dfp_incident_dfp_comment), comment);

                    await dynamicsContext.SaveChangesAsync();
                    result.Success = true;
                    result.Id = comment.dfp_commentid.ToString();
                    dynamicsContext.DetachAll();
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "CreateLegacyCaseComment Error");
                    result.Success = false;
                }
            }           

            return result;
        }

        /// <summary>
        /// Get Document Type
        /// </summary>
        /// <param name="documentTypeCode"></param>
        /// <param name="documentType"></param>
        /// <param name="businessArea"></param>
        /// <returns></returns>
        private dfp_submittaltype GetDocumentType(string documentTypeCodeInput, string documentType, string businessArea)
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
                    documentTypeCode = documentType.Replace(" ","");
                }
            }
            // lookup the document Type Code
            
            if (!string.IsNullOrEmpty(documentTypeCode))
            {
                try
                {
                    var record = dynamicsContext.dfp_submittaltypes.Where(d => d.dfp_apidocumenttype == documentTypeCode).FirstOrDefault();                     
                    result = record;                    
                    
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, $"Error searching for document Type Code {documentTypeCode}");
                    result = null;
                }
            }

            if (result == null)
            {
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

                result = dynamicsContext.dfp_submittaltypes.Where(d => d.dfp_apidocumenttype == documentTypeCode).FirstOrDefault();
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

            // create the document.
            incident driverCase = GetIncidentById(request.CaseId);

            // get the driver
            var driver = GetDriverObjects(request.Driver.DriverLicenseNumber).FirstOrDefault();

            bool driverMismatch = false;

            if (driverCase != null && driver.dfp_driverid != driverCase._dfp_driverid_value)
            {
                // driver mismatch
                driverMismatch = true;
            }


            // document type ID
            var documentTypeId = GetDocumentType(request.DocumentTypeCode, request.DocumentType, request.BusinessArea);


            if (driverCase == null)
            {                
                // create it.
                var newDriver = new LegacyCandidateSearchRequest() { DriverLicenseNumber = request.Driver.DriverLicenseNumber, Surname = request.Driver.Surname, SequenceNumber = request.SequenceNumber };
                await LegacyCandidateCreate(newDriver, request.Driver.BirthDate, DateTime.MinValue);
                var newDriverResult = await LegacyCandidateSearch(newDriver);

                var firstCase = newDriverResult.Items.FirstOrDefault();

                if (firstCase != null)
                {
                    driverCase = GetIncidentById(firstCase.Id);
                }

            }

            if (driverCase != null)
            {
                bool found = false;
                // ensure we have the documents.
                await dynamicsContext.LoadPropertyAsync(driverCase, nameof(incident.bcgov_incident_bcgov_documenturl));
                bcgov_documenturl bcgovDocumentUrl = null;

                if (documentTypeId != null)
                {
                    foreach (var doc in driverCase.bcgov_incident_bcgov_documenturl)
                    {
                        await dynamicsContext.LoadPropertyAsync(doc, nameof(bcgovDocumentUrl.dfp_DocumentTypeID));
                        if (doc.statecode == 0 // active
                            && doc.dfp_submittalstatus == 100000000
                            && doc.dfp_DocumentTypeID.dfp_submittaltypeid == documentTypeId.dfp_submittaltypeid) // open - required
                        {
                            bcgovDocumentUrl = doc;
                            found = true;
                            break;
                        }
                    }
                }

                if (bcgovDocumentUrl == null)
                {
                    bcgovDocumentUrl = new bcgov_documenturl() ;                   
                }               
                
                bcgovDocumentUrl.dfp_batchid = request.BatchId;
                bcgovDocumentUrl.dfp_documentpages = request.DocumentPages.ToString();
                bcgovDocumentUrl.bcgov_url = request.DocumentUrl;
                bcgovDocumentUrl.bcgov_receiveddate = DateTimeOffset.Now;
                bcgovDocumentUrl.dfp_faxreceiveddate = request.FaxReceivedDate;
                bcgovDocumentUrl.dfp_uploadeddate = DateTimeOffset.Now;
                bcgovDocumentUrl.dfp_dpsprocessingdate = request.ImportDate;
                bcgovDocumentUrl.dfp_importid = request.ImportId;
                bcgovDocumentUrl.dfp_faxnumber = request.OriginatingNumber;
                bcgovDocumentUrl.dfp_validationmethod = request.ValidationMethod;
                bcgovDocumentUrl.dfp_validationprevious = request.ValidationPrevious;
                bcgovDocumentUrl.dfp_submittalstatus = 100000001; // Received                                                       

                if (!string.IsNullOrEmpty(request.DocumentUrl))
                {
                    bcgovDocumentUrl.bcgov_fileextension = Path.GetExtension(request.DocumentUrl);
                    bcgovDocumentUrl.bcgov_filename = Path.GetFileName(request.DocumentUrl);
                }

                if (found) // update
                {
                    try
                    {
                        dynamicsContext.UpdateObject(bcgovDocumentUrl);                        

                        dynamicsContext.SetLink (bcgovDocumentUrl, nameof(bcgovDocumentUrl.dfp_DocumentTypeID), documentTypeId);
                        dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgovDocumentUrl.dfp_DriverId), driver);
                        
                        await dynamicsContext.SaveChangesAsync();
                        result.Success = true;
                        result.Id = bcgovDocumentUrl.bcgov_documenturlid.ToString();
                        dynamicsContext.DetachAll();
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
                        dynamicsContext.AddTobcgov_documenturls(bcgovDocumentUrl);

                        if (documentTypeId != null)
                        {
                            dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgov_documenturl.dfp_DocumentTypeID), documentTypeId);
                        }

                        if (!driverMismatch && driverCase != null)
                        {
                            dynamicsContext.AddLink(driverCase, nameof(incident.bcgov_incident_bcgov_documenturl), bcgovDocumentUrl);                                
                        }
                        dynamicsContext.SetLink(bcgovDocumentUrl, nameof(bcgovDocumentUrl.dfp_DriverId), driver);


                        await dynamicsContext.SaveChangesAsync();
                        result.Success = true;
                        result.Id = bcgovDocumentUrl.bcgov_documenturlid.ToString();
                        dynamicsContext.DetachAll();
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Error(ex, "CreateLegacyCaseDocument");
                        result.Success = false;
                    }
                }
            }

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
        public async Task<bool> DeleteLegacyDocument(string documentId)
        {
            bool result = false;

            var document = dynamicsContext.bcgov_documenturls.ByKey(Guid.Parse(documentId)).GetValue(); 
            if (document != null)
            {
                dynamicsContext.DeactivateObject(document, 2);                
                // set to inactive.                
                await dynamicsContext.SaveChangesAsync();
                dynamicsContext.DetachAll();
                result = true;
            }
            else
            {
                Log.Error($"Could not find document {documentId}");                
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
                if (d != null && d.dfp_OutcomeStatus != null )
                {
                    // ensure the decision has data.
                    dynamicsContext.LoadPropertyAsync(d, nameof(dfp_decision.dfp_OutcomeStatus)).GetAwaiter().GetResult();
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
        private string CombineName (string firstname, string lastname)
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
        /// Map Cases
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
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
                        Name = CombineName(c.dfp_DriverId?.dfp_PersonId?.lastname, c.dfp_DriverId?.dfp_PersonId?.firstname)
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
                        ClinicName = c.dfp_ClinicId?.name  ?? string.Empty,
                        DmerType = TranslateDmerType (c.dfp_dmertype),
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
                                Description = f.dfp_FlagId?.dfp_description
                            }).ToArray(),
                        Decisions = c.dfp_incident_dfp_decision
                            .Select(d => new Decision
                            {
                                Id = d.dfp_decisionid.ToString(),
                                Outcome = TranslateDecisionOutcome(d.dfp_decisionid),
                                CreatedOn = d.createdon ?? default
                            }),                        
                        Status = TranslateStatus(c.statuscode)
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
        private DateTime FilterLastCaseModified (DateTimeOffset? value, DateTime created)
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
        public async Task<Guid?> LegacyCandidateCreate(LegacyCandidateSearchRequest request, DateTimeOffset? birthDate, DateTimeOffset? effectiveDate)
        {
            Guid? result = null;

            dfp_driver driver;
            contact driverContact;
            Guid? driverContactId;

            var driverQuery = dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d => d.dfp_licensenumber == request.DriverLicenseNumber && d.statuscode == 1);
            var data = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToList();

            if (birthDate != null && birthDate.Value.Year < 1753)
            {
                birthDate = new DateTime(1753,1,1);
            }


            dfp_driver[] driverResults;
            
            if (! string.IsNullOrEmpty(request?.Surname))
            {
                driverResults = data.Where(x => x?.dfp_PersonId?.lastname != null && (bool)(x?.dfp_PersonId?.lastname.StartsWith(request?.Surname))).ToArray();
            }
            else
            {
                driverResults = data.ToArray();
            }

            if (driverResults.Length > 0)
            {
                driver = driverResults[0];
                if (driver.dfp_PersonId != null)
                {
                    driverContactId = driver.dfp_PersonId.contactid;
                    driverContact = driver.dfp_PersonId;
                }
                else
                {
                    driverContact = new contact()
                    {
                        lastname = request.Surname                                                
                    };

                    if (birthDate != null)
                    {
                        driverContact.birthdate = new Microsoft.OData.Edm.Date(birthDate.Value.Year,
                            birthDate.Value.Month, birthDate.Value.Day);
                    }

                    dynamicsContext.AddTocontacts(driverContact);
                    driver.dfp_PersonId = driverContact;
                    dynamicsContext.SetLink(driver, nameof(dfp_driver.dfp_PersonId), driverContact);
                }
                
            }
            else // create the driver.
            {
                driverContact = new contact()
                {
                    lastname = request.Surname
                };
                if (birthDate != null)
                {
                    driverContact.birthdate = new Microsoft.OData.Edm.Date(birthDate.Value.Year,
                        birthDate.Value.Month, birthDate.Value.Day);
                }

                dynamicsContext.AddTocontacts(driverContact);

                driver = new dfp_driver()
                {
                    dfp_licensenumber = request.DriverLicenseNumber,
                    dfp_PersonId = driverContact,
                    statuscode = 1,                    
                };
                if (birthDate != null)
                {
                    driver.dfp_dob = birthDate.Value;
                }

                dynamicsContext.AddTodfp_drivers(driver);
                dynamicsContext.SetLink(driver, nameof(dfp_driver.dfp_PersonId), driverContact);                
            }


            // create the case.
            incident @case = new incident()
            {
                customerid_contact = driverContact,
                // set status to Open Pending for Submission
                statuscode = 100000000,                
                casetypecode = 2, // DMER
                // set progress status to in queue, ready for review
                dfp_progressstatus = 100000000,
                importsequencenumber = request.SequenceNumber,
                dfp_DriverId = driver

            };

            if (effectiveDate != null && effectiveDate != DateTimeOffset.MinValue)
            {
                //@case.dfp_datesenttoicbc = effectiveDate;
            }

            dynamicsContext.AddToincidents(@case);

            if (driverContact != null)
            {
                dynamicsContext.SetLink(@case, nameof(incident.customerid_contact), driverContact);
            }

            dynamicsContext.SetLink(@case, nameof(incident.dfp_DriverId), driver);

            // temporarily turn off batch so that incident will create properly
            dynamicsContext.SaveChangesDefaultOptions = SaveChangesOptions.None;
            await dynamicsContext.SaveChangesAsync();

            result = @case.incidentid;

            dynamicsContext.SaveChangesDefaultOptions = SaveChangesOptions.BatchWithSingleChangeset;

            dynamicsContext.DetachAll();

            return result;
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


        /// <summary>
        /// Translate the Dynamics statuscode (status reason) field to text
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private string TranslateStatus (int? statusCode)
        {
            var statusMap = new Dictionary<int, string>()
            {
                { 1, "In Progress" },
                { 2, "On Hold" },
                { 3, "Waiting on Details" },
                { 4, "Case Created" },
                { 100000000, "Open - Pending Submission" },
                { 5, "RSBC Decision Rendered" },
                { 1000, "RSBC Received" },
                { 6, "Closed/Canceled" },
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
            /*
             * {
                { 100000000, "Commercial/NSC" },
                { 100000001, "Age" },
                { 100000002, "Industrial Road" },
                { 100000003, "Known/Suspected Condition" },
                { 100000004, "Scheduled Routine"},
                { 100000005, "No DMER"}
               };
            */
            {
                { 100000000, "Commercial" },
                { 100000001, "Scheduled Age" },
                { 100000002, "Commercial" },
                { 100000003, "Known/Suspected Condition" },
                { 100000004, "Scheduled Routine"},
                { 100000005, "No DMER"}
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
        /// Search Cases
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<incident>> SearchCases(DynamicsContext ctx, CaseSearchRequest criteria)
        {
            bool notJustDriverLicenseSearch = !string.IsNullOrEmpty(criteria.CaseId) || !string.IsNullOrEmpty(criteria.Title) || !string.IsNullOrEmpty(criteria.ClinicId);
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
                .Expand(i => i.ownerid)
                .Where(i => i.statecode == 1);

            if (!string.IsNullOrEmpty(criteria.CaseId)) 
            {
                caseQuery = caseQuery.Where(i => i.incidentid == Guid.Parse(criteria.CaseId));
            }
            else
            {
                caseQuery = caseQuery.Where(i => i.casetypecode == (int)CaseTypeOptionSet.DMER);
            }

            if (!string.IsNullOrEmpty(criteria.Title)) caseQuery = caseQuery.Where(i => i.ticketnumber == criteria.Title);
            if (!string.IsNullOrEmpty(criteria.ClinicId)) caseQuery = caseQuery.Where(i => i._dfp_clinicid_value == Guid.Parse(criteria.ClinicId));
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
        private static async Task<IEnumerable<incident>> SearchLegacyCandidate(DynamicsContext ctx, LegacyCandidateSearchRequest criteria)
        {
            var shouldSearchCases =
                !string.IsNullOrEmpty(criteria.DriverLicenseNumber) ||
                !string.IsNullOrEmpty(criteria.Surname);

            if (!shouldSearchCases) return Array.Empty<incident>();

            Guid? driverId = Guid.Empty;            
            var driverQuery = ctx.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d => d.dfp_licensenumber == criteria.DriverLicenseNumber );
            var data = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToList();

            var driverResults = data.Where (x => x?.dfp_PersonId?.lastname != null && (bool)(x?.dfp_PersonId?.lastname.StartsWith( criteria?.Surname))).ToArray();

            if (driverResults.Length > 0)
            {
                driverId = driverResults[0].dfp_driverid;
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
        private static async Task<IEnumerable<incident>> SearchDriverCases(DynamicsContext ctx, CaseSearchRequest criteria)
        {
            var shouldSearchDrivers = !string.IsNullOrEmpty(criteria.DriverLicenseNumber);

            if (!shouldSearchDrivers) return Array.Empty<incident>();

            var driverQuery = ctx.dfp_drivers.Expand(d => d.dfp_PersonId).Where(d => d.statecode == (int)EntityState.Active);
            if (!string.IsNullOrEmpty(criteria.DriverLicenseNumber)) driverQuery = driverQuery.Where(i => i.dfp_licensenumber == criteria.DriverLicenseNumber);
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
                    Description = flag.dfp_description
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
        /// Get Unsent Medical Updates
        /// </summary>
        /// <returns></returns>
        public async Task<CaseSearchReply> GetUnsentMedicalUpdates()
        {
            var caseQuery = dynamicsContext.incidents
                .Expand(i => i.dfp_DriverId)                
                .Expand(i => i.dfp_incident_dfp_decision)
                .Where(i => i.statecode == 0 // Active
                        && i.dfp_datesenttoicbc == null);
            var cases = await ((DataServiceQuery<incident>)caseQuery).GetAllPagesAsync();
    
            var outputArray = new List<incident>();

            foreach (var @case in cases)
            {
                if (@case._dfp_driverid_value.HasValue)
                {
                    //load driver info
                    await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_DriverId));
                    if (@case.dfp_DriverId != null) await dynamicsContext.LoadPropertyAsync(@case.dfp_DriverId, nameof(incident.dfp_DriverId.dfp_PersonId));
                }

                //load decisions
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_decision));
                if (@case.dfp_incident_dfp_decision.Count > 0)
                {
                    foreach (var decision in @case.dfp_incident_dfp_decision)
                    {
                        await dynamicsContext.LoadPropertyAsync(decision, nameof(dfp_decision.dfp_decisionid));
                        if (decision.dfp_OutcomeStatus != null) await dynamicsContext.LoadPropertyAsync(decision.dfp_OutcomeStatus, nameof(dfp_decision.dfp_OutcomeStatus));
                    }
                    outputArray.Add(@case);

                }
                
                
            }

            dynamicsContext.DetachAll();

            return MapCases(outputArray);            
        }

        /// <summary>
        /// Get Dps Processing Date
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset GetDpsProcessingDate()
        {
            var mostRecentRecord = dynamicsContext.bcgov_documenturls
                .OrderByDescending(i => i.dfp_dpsprocessingdate)
                .Take(1)
                .FirstOrDefault();

            if (mostRecentRecord != null && mostRecentRecord.dfp_dpsprocessingdate != null)
            {
                return mostRecentRecord.dfp_dpsprocessingdate.Value;
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

            var dpsProcessingDate = GetDpsProcessingDate();

            var nonComplyDocuments = dynamicsContext.bcgov_documenturls.Where(
                x => x.dfp_submittalstatus == 100000000 // Open - Required
                && x.dfp_compliancedate < dpsProcessingDate
                );

            foreach (var document in nonComplyDocuments)
            {
                document.dfp_submittalstatus = 100000005; // Non Comply
                dynamicsContext.UpdateObject(document);
            }

            await dynamicsContext.SaveChangesAsync();
            dynamicsContext.DetachAll();
        }

        /// <summary>
        /// Method to set the resolve case status
        /// </summary>
        /// <returns></returns>

        public async Task ResolveCaseStatusUpdates()
        {

           var dpsProcessingDate = GetDpsProcessingDate();

            var resolveCases = dynamicsContext.incidents.Where(
                 x => x.dfp_caseresolvedate < dpsProcessingDate
                 && x.statecode == 0 // ensure that we only get active records
                 );

            foreach (var incident in resolveCases)
            {
                // set resolve case status to yes
                incident.dfp_resolvecase = true;

                dynamicsContext.UpdateObject(incident);
            }

            await dynamicsContext.SaveChangesAsync();
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
                    var dmerEntity = dynamicsContext.incidents.Where(x => x.incidentid == Guid.Parse(caseId)).FirstOrDefault();
                    if (dmerEntity != null)
                    {
                        // Update the error message in CMS
                        dmerEntity.dfp_icbcerrorlog = errorMessage;
                        
                    }
                    dynamicsContext.UpdateObject(dmerEntity);
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.DetachAll();
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    Log.Logger.Error(ex.Message);
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
            incident dmerEntity = dynamicsContext.incidents.ByKey(Guid.Parse(dmerIdentifier)).Expand(x => x.bcgov_incident_bcgov_documenturl).GetValue();

            if (dmerEntity != null)
            {
                if (dmerEntity.bcgov_incident_bcgov_documenturl.Count(x => x.bcgov_url == fileKey) == 0)
                {
                    // add the document url.
                    bcgov_documenturl givenUrl = dynamicsContext.bcgov_documenturls.Where(x => x.bcgov_url == fileKey).FirstOrDefault();
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
                        dynamicsContext.AddTobcgov_documenturls(givenUrl);
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
                    result = label.Substring(0,maxFieldLength); // Max 200 chars
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
        public async Task<SetCaseFlagsReply> SetCaseFlags(string dmerIdentifier, bool isCleanPass, List<Flag> flags, Microsoft.Extensions.Logging.ILogger logger = null)
        {
            if (logger == null) logger = this.logger;
            /* The structure for cases is

            Case (incident) is the parent item
                - has children which are flag entities

             */

            int flagCount = flags == null ? 0 : flags.Count;

            logger.LogInformation($"SetCaseFlags - looking for DMER with identifier {dmerIdentifier} {flagCount}");

            // future state - the case name will contain three letters of the name and the driver licence number

            incident dmerEntity = dynamicsContext.incidents.ByKey(Guid.Parse(dmerIdentifier)).Expand(x => x.dfp_incident_dfp_dmerflag).GetValue();

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
                            dfp_type = (int?)FlagTypeOptionSet.Review
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
            logger.LogInformation($"SetCasePractitionerClinic - looking for DMER with identifier {caseId} {practitionerId} {clinicId}");

            // future state - the case name will contain three letters of the name and the driver licence number

            incident dmerEntity = dynamicsContext.incidents.ByKey(Guid.Parse(caseId)).GetValue();

            if (dmerEntity != null)
            {
                // set the Practitioner

                if (!string.IsNullOrEmpty(practitionerId))
                {
                    // set the Practitioner
                    dfp_medicalpractitioner medicalpractitioner = dynamicsContext.dfp_medicalpractitioners.ByKey(Guid.Parse(caseId)).GetValue();

                    dynamicsContext.SetLink(dmerEntity, nameof(incident.dfp_MedicalPractitionerId), medicalpractitioner);
                }

                if (! string.IsNullOrEmpty(clinicId))
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
        DMER = 2
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