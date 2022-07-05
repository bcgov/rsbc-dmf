using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface ICaseManager
    {
        Task<CaseSearchReply> CaseSearch(CaseSearchRequest request);

        Task<CreateStatusReply> CreateLegacyCaseComment(LegacyComment request);

        Task<CreateStatusReply> CreateLegacyCaseDocument(LegacyDocument request);        

        Task<IEnumerable<LegacyComment>> GetDriverLegacyComments(string driverLicenseNumber, bool allComments);

        Task<IEnumerable<LegacyComment>> GetCaseLegacyComments(string caseId, bool allComments);

        Task<IEnumerable<LegacyDocument>> GetCaseLegacyDocuments(string caseId);

        Task<IEnumerable<LegacyDocument>> GetDriverLegacyDocuments(string driverLicenseNumber);

        Task<IEnumerable<Driver>> GetDrivers();

        Task<CaseSearchReply> LegacyCandidateSearch(LegacyCandidateSearchRequest request);

        /// <summary>
        /// Create a Legacy Candidate
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Guid of the created case</returns>
        Task<Guid?> LegacyCandidateCreate(LegacyCandidateSearchRequest request);

        Task MarkMedicalUpdatesSent(List<string> ids);

        Task<SetCaseFlagsReply> SetCaseFlags(string dmerIdentifier, bool isCleanPass, List<Flag> flags, ILogger logger = null);

        Task SetCasePractitionerClinic(string caseId, string practitionerId, string clinicId);

        Task<List<Flag>> GetAllFlags();

        Task<CaseSearchReply> GetUnsentMedicalUpdates();

        Task AddDocumentUrlToCaseIfNotExist(string dmerIdentifier, string fileKey, Int64 fileSize);
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

    internal class CaseManager : ICaseManager
    {
        private readonly DynamicsContext dynamicsContext;
        private readonly ILogger<CaseManager> logger;

        public CaseManager(DynamicsContext dynamicsContext, ILogger<CaseManager> logger)
        {
            this.dynamicsContext = dynamicsContext;
            this.logger = logger;
        }


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
                    await dynamicsContext.LoadPropertyAsync(comment, nameof(dfp_comment.dfp_commentid));
                    if (allComments || comment.dfp_icbc.GetValueOrDefault())
                    {
                        LegacyComment legacyComment = new LegacyComment
                        {
                            CaseId = @case.incidentid.ToString(),
                            CommentDate = comment.dfp_date.GetValueOrDefault(),
                            CommentId = comment.dfp_commentid.ToString(),
                            CommentText = comment.dfp_commentdetails,
                            CommentTypeCode = comment.dfp_icbc.GetValueOrDefault() == true ? "W" : "",
                            SequenceNumber = @case.importsequencenumber.GetValueOrDefault(),
                            UserId = comment.dfp_userid
                        };
                        result.Add(legacyComment);
                    }
                }
            }
               
            return result;
        }


        public async Task<IEnumerable<LegacyDocument>> GetCaseLegacyDocuments(string caseId)
        {
            List<LegacyDocument> result = new List<LegacyDocument>();
            
            var @cases = dynamicsContext.incidents.Where(i => i.incidentid == Guid.Parse(caseId)    ).ToList();

            foreach (var @case in @cases)
            {
                // ensure related data is loaded.
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_DriverId));
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.bcgov_incident_bcgov_documenturl));

                Driver driver = new Driver()
                {
                    DriverLicenseNumber = @case.dfp_DriverId?.dfp_licensenumber,                    
                };

                foreach (var document in @case.bcgov_incident_bcgov_documenturl)
                {
                    await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.bcgov_documenturlid));
                    await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.dfp_DocumentTypeID));

                    LegacyDocument legacyDocument = new LegacyDocument
                    {
                        BatchId = document.dfp_batchid,                       
                        CaseId = @case.incidentid.ToString(),
                        DocumentPages = ConvertPagesToInt (document.dfp_documentpages),
                        DocumentId = document.bcgov_documenturlid.ToString(),
                        DocumentTypeCode = document.dfp_DocumentTypeID?.dfp_name,
                        DocumentUrl = document.bcgov_url,
                        FaxReceivedDate = document.dfp_faxreceiveddate.GetValueOrDefault(),
                        ImportDate = document.dfp_dpsprocessingdate.GetValueOrDefault(),
                        ImportId = document.dfp_importid,
                        OriginatingNumber = document.dfp_faxsender,
                        ValidationMethod = document.dfp_validationmethod,
                        ValidationPrevious = document.dfp_validationprevious,                      
                        SequenceNumber = @case.importsequencenumber.GetValueOrDefault(),
                        Driver = driver
                    };

                    result.Add(legacyDocument);
                }
            }
             
            return result;
        }

        private int ConvertPagesToInt (string data)
        {
            int result = 0;
            if (!int.TryParse (data, out result))
            {
                result = 0;
            }
            return result;

        }

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
                                    CommentDate = comment.dfp_date.GetValueOrDefault(),
                                    CommentId = comment.dfp_commentid.ToString(),
                                    CommentText = comment.dfp_commentdetails,
                                    CommentTypeCode = comment.dfp_icbc.GetValueOrDefault() == true ? "W" : "",
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


        public async Task<IEnumerable<LegacyDocument>> GetDriverLegacyDocuments(string driverLicenceNumber)
        {
            List<LegacyDocument> result = new List<LegacyDocument>();
            // start by the driver

            var @drivers = dynamicsContext.dfp_drivers.Where(d => d.dfp_licensenumber == driverLicenceNumber && d.statuscode == 1).ToList();

            foreach (var @driver in @drivers)
            {
                if (@driver != null)
                {
                    // .Expand(x => x.dfp_incident_dfp_comment)
                    // get the cases for that driver.
                    var @cases = dynamicsContext.incidents.Where(i => i._dfp_driverid_value == @driver.dfp_driverid
                    ).ToList();

                    foreach (var @case in @cases)
                    {
                        // ensure related data is loaded.

                        Driver caseDriver = new Driver()
                        {
                            DriverLicenseNumber = @case.dfp_DriverId?.dfp_licensenumber,
                        };

                        await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.bcgov_incident_bcgov_documenturl));
                        foreach (var document in @case.bcgov_incident_bcgov_documenturl)
                        {
                            await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.bcgov_documenturlid));

                            LegacyDocument legacyDocument = new LegacyDocument
                            {
                                BatchId = document.dfp_batchid,
                                CaseId = @case.incidentid.ToString(),
                                DocumentPages = ConvertPagesToInt(document.dfp_documentpages),
                                DocumentId = document.bcgov_documenturlid.ToString(),
                                DocumentTypeCode = document.dfp_DocumentTypeID?.dfp_name,
                                DocumentUrl = document.bcgov_url,
                                FaxReceivedDate = document.dfp_faxreceiveddate.GetValueOrDefault(),
                                ImportDate = document.dfp_dpsprocessingdate.GetValueOrDefault(),
                                ImportId = document.dfp_importid,
                                OriginatingNumber = document.dfp_faxsender,
                                ValidationMethod = document.dfp_validationmethod,
                                ValidationPrevious = document.dfp_validationprevious,
                                SequenceNumber = @case.importsequencenumber.GetValueOrDefault(),
                                Driver = caseDriver
                            };

                            result.Add(legacyDocument);
                        }
                    }
                }
            }

            return result;
        }

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

        private incident GetIncidentById(string id)
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


        private incident GetIncidentByIdWithComments(string id)
        {
            incident result = null;
            try
            {
                result = dynamicsContext.incidents.Expand(d => d.dfp_incident_dfp_comment)
                    .Where(d => d.incidentid == Guid.Parse(id))
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        private incident GetIncidentBySequence (int sequence)
        {
            incident result = null;
            try
            {
                result = dynamicsContext.incidents.Where(d => d.importsequencenumber == sequence).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        private incident GetIncidentBySequenceWithComments(int sequence)
        {
            incident result = null;
            try
            {
                result = dynamicsContext.incidents.Expand(d => d.dfp_incident_dfp_comment).FirstOrDefault(d => d.importsequencenumber == sequence);
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public async Task<CreateStatusReply> CreateLegacyCaseComment(LegacyComment request)
        {
            CreateStatusReply result = new CreateStatusReply()
            {
                Success = false
            };            
            string caseId = request.CaseId;
            if (string.IsNullOrEmpty( request.CaseId))
            {
                // create a new case.
                LegacyCandidateSearchRequest newCandidate = new LegacyCandidateSearchRequest()
                {
                     DriverLicenseNumber = request.Driver.DriverLicenseNumber,
                     SequenceNumber = request.SequenceNumber,
                     Surname = request.Driver.Surname
                };

                await LegacyCandidateCreate(newCandidate);
                
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
                dfp_comment @comment = new dfp_comment()
                {
                    dfp_date = DateTimeOffset.Now,
                    dfp_icbc = true,
                    dfp_userid = request.UserId,
                    dfp_commentdetails = request.CommentText
                };

                try
                {
                    dynamicsContext.AddTodfp_comments(@comment);

                    dynamicsContext.SetLink(@comment, nameof(dfp_comment.dfp_DriverId), driver);
                    dynamicsContext.AddLink(@case, nameof(incident.dfp_incident_dfp_comment), @comment);

                    await dynamicsContext.SaveChangesAsync();
                    result.Success = true;
                    result.Id = @comment.dfp_commentid.ToString();
                    dynamicsContext.DetachAll();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                }
            }           

            return result;
        }

        private string GetDocumentTypeId(string documentTypeCode)
        {
            // lookup the document Type Code
            string result = null;
            if (!string.IsNullOrEmpty(documentTypeCode))
            {
                try
                {
                    var record = dynamicsContext.dfp_submittaltypes.Where(d => d.dfp_code == documentTypeCode).FirstOrDefault(); 
                    if (record == null)
                    {
                        var newRecord = new dfp_submittaltype { dfp_code = documentTypeCode, dfp_name = $"NEW CODE {documentTypeCode}" };
                        dynamicsContext.AddTodfp_submittaltypes(newRecord);
                        dynamicsContext.SaveChanges();

                        record = dynamicsContext.dfp_submittaltypes.Where(d => d.dfp_code == documentTypeCode).FirstOrDefault();
                    }
                    if (record != null)
                    {
                        result = record.dfp_submittaltypeid.ToString();
                    }
                    
                }
                catch (Exception ex)
                {
                    result = null;
                }
            }

            return result;
        }

        public async Task<CreateStatusReply> CreateLegacyCaseDocument(LegacyDocument request)
        {
            CreateStatusReply result = new CreateStatusReply();

            // create the document.
            incident @case = GetIncidentById(request.CaseId);

            if (@case == null)
            {
                // create it.
                await LegacyCandidateCreate(new LegacyCandidateSearchRequest() { DriverLicenseNumber = request.Driver.DriverLicenseNumber, Surname = request.Driver.Surname, SequenceNumber = request.SequenceNumber });
                @case = GetIncidentById(request.CaseId);
            }

            // TODO - add missing properties

            // create the document.
            bcgov_documenturl @documentUrl = new bcgov_documenturl()
            {
                 dfp_batchid = request.BatchId,
                 dfp_documentpages = request.DocumentPages.ToString(),
                 bcgov_url = request.DocumentUrl,
                 dfp_faxreceiveddate = request.FaxReceivedDate,
                 dfp_dpsprocessingdate = request.ImportDate,
                 dfp_importid = request.ImportId,
                 dfp_faxnumber = request.OriginatingNumber,
                 dfp_validationmethod = request.ValidationMethod,
                 dfp_validationprevious = request.ValidationPrevious
            };

            // document type ID
            var documentTypeId = GetDocumentTypeId(request.DocumentTypeCode);

            try
            {
                dynamicsContext.AddTobcgov_documenturls(@documentUrl);
                dynamicsContext.AddLink( @documentUrl, nameof (bcgov_documenturl.dfp_DocumentTypeID), documentTypeId);
                dynamicsContext.AddLink(@case, nameof(incident.bcgov_incident_bcgov_documenturl), @documentUrl);


                await dynamicsContext.SaveChangesAsync();
                result.Success = true;
                result.Id = @documentUrl.bcgov_documenturlid.ToString();
                dynamicsContext.DetachAll();
            }
            catch (Exception ex)
            {
                result.Success = false;                
            }

            return result;
        }

        private DecisionOutcome? TranslateDecisionOutcome(Guid? decisionId)
        {
            DecisionOutcome? result = null;
            if (decisionId != null)
            {
                // get the decision record.
                var d = dynamicsContext.dfp_decisions.Expand(x => x.dfp_OutcomeStatus)
                    .First(x => x.dfp_decisionid == decisionId);
                if (d != null && d.dfp_OutcomeStatus != null )
                {
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

        //map cases from query results (TODO: consider replacing with AutoMapper)
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
                    return new DmerCase
                    {
                        Id = c.incidentid.ToString(),
                        Title = c.title,
                        CreatedBy = $"{c.customerid_contact?.lastname?.ToUpper()}, {c.customerid_contact?.firstname}",
                        CreatedOn = c.createdon.Value.DateTime,
                        ModifiedBy = $"{c.customerid_contact?.lastname?.ToUpper()}, {c.customerid_contact?.firstname}",
                        ModifiedOn = c.modifiedon.Value.DateTime,
                        ClinicId = c.dfp_ClinicId?.accountid.ToString(),
                        ClinicName = c.dfp_ClinicId?.name,
                        DmerType = TranslateDmerType (c.dfp_dmertype),
                        Driver = new CaseManagement.Driver()
                        {
                            Id = c.dfp_DriverId?.dfp_driverid.ToString(),
                            Address = new Address()
                            {
                                City = c.dfp_DriverId?.dfp_PersonId?.address1_city,
                                Postal = c.dfp_DriverId?.dfp_PersonId?.address1_postalcode,
                                Line1 = c.dfp_DriverId?.dfp_PersonId?.address1_line1,
                                Line2 = c.dfp_DriverId?.dfp_PersonId?.address1_line2,
                            },
                            BirthDate = c.dfp_DriverId?.dfp_PersonId?.birthdate ?? default(DateTime),
                            DriverLicenseNumber = c.dfp_DriverId?.dfp_licensenumber,
                            GivenName = c.dfp_DriverId?.dfp_PersonId?.firstname,
                            Middlename = c.dfp_DriverId?.dfp_PersonId?.middlename,
                            Sex = TranslateGenderCode(c.dfp_DriverId?.dfp_PersonId?.gendercode),
                            Surname = c.dfp_DriverId?.dfp_PersonId?.lastname,
                            Name =
                                $"{c.dfp_DriverId?.dfp_PersonId?.lastname.ToUpper()}, {c.dfp_DriverId?.dfp_PersonId?.firstname}",
                        },
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
                                Outcome = TranslateDecisionOutcome(d.dfp_OutcomeStatus?.dfp_outcomestatusid),
                                CreatedOn = d.createdon ?? default
                            }),                        
                        Status = TranslateStatus(c.statuscode)
                    };
                }).ToArray()
            };

       }


        public async Task<Guid?> LegacyCandidateCreate(LegacyCandidateSearchRequest request)
        {
            Guid? result = null;

            dfp_driver driver;
            contact driverContact;
            Guid? driverContactId;

            var driverQuery = dynamicsContext.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d => d.dfp_licensenumber == request.DriverLicenseNumber && d.statuscode == 1);
            var data = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToList();

            dfp_driver[] driverResults;
            
            if (! string.IsNullOrEmpty(request?.Surname))
            {
                driverResults = data.Where(x => (bool)(x?.dfp_PersonId?.lastname.StartsWith(request?.Surname))).ToArray();
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
                dynamicsContext.AddTocontacts(driverContact);

                driver = new dfp_driver()
                {
                    dfp_licensenumber = request.DriverLicenseNumber,
                    dfp_PersonId = driverContact,
                    statuscode = 1
                };
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
            {
                { 100000000, "Commercial/NSC" },
                { 100000001, "Age" },
                { 100000002, "Industrial Road" },
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
                .Where(i => i.casetypecode == (int)CaseTypeOptionSet.DMER);

            if (!string.IsNullOrEmpty(criteria.CaseId)) caseQuery = caseQuery.Where(i => i.incidentid == Guid.Parse(criteria.CaseId));
            if (!string.IsNullOrEmpty(criteria.Title)) caseQuery = caseQuery.Where(i => i.title == criteria.Title);
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


        private static async Task<IEnumerable<incident>> SearchLegacyCandidate(DynamicsContext ctx, LegacyCandidateSearchRequest criteria)
        {
            var shouldSearchCases =
                !string.IsNullOrEmpty(criteria.DriverLicenseNumber) ||
                !string.IsNullOrEmpty(criteria.Surname);

            if (!shouldSearchCases) return Array.Empty<incident>();

            Guid? driverId = Guid.Empty;            
            var driverQuery = ctx.dfp_drivers.Expand(x => x.dfp_PersonId).Where(d => d.dfp_licensenumber == criteria.DriverLicenseNumber );
            var data = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToList();

            var driverResults = data.Where (x => (bool)(x?.dfp_PersonId?.lastname.StartsWith( criteria?.Surname))).ToArray();

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

        public async Task<CaseSearchReply> GetUnsentMedicalUpdates()
        {
            var caseQuery = dynamicsContext.incidents
                .Expand(i => i.dfp_DriverId)
                .Expand(i => i.customerid_contact)
                .Expand(i => i.dfp_ClinicId)
                .Expand(i => i.dfp_MedicalPractitionerId)
                .Expand(i => i.dfp_incident_dfp_dmerflag)
                .Expand(i => i.dfp_incident_dfp_decision)
                .Where(i => i.dfp_datesenttoicbc == null);
            var cases = await ((DataServiceQuery<incident>)caseQuery).GetAllPagesAsync();
            var caseArray = cases.ToArray();

            foreach (var @case in caseArray)
            {
                await LazyLoadProperties(@case);
            }

            dynamicsContext.DetachAll();

            return MapCases(caseArray);            
        }

        public async Task MarkMedicalUpdatesSent(List<string> ids)
        {
            DateTimeOffset dateSent = DateTimeOffset.UtcNow;
            foreach (var id in ids)
            {
                var dmerEntity = dynamicsContext.incidents.First(x => x.incidentid == Guid.Parse(id));
                dmerEntity.dfp_datesenttoicbc = dateSent;
                dynamicsContext.UpdateObject(dmerEntity);
            }
            
            await dynamicsContext.SaveChangesAsync();
            dynamicsContext.DetachAll();
        }

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
                        givenUrl = new bcgov_documenturl()
                        {
                            bcgov_url = fileKey,
                            bcgov_receiveddate = DateTimeOffset.Now,
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

        public async Task<SetCaseFlagsReply> SetCaseFlags(string dmerIdentifier, bool isCleanPass, List<Flag> flags, ILogger logger = null)
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

        ///
        /// translate a gendercode into a common string.
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
    }

    internal enum CaseTypeOptionSet
    {
        DMER = 2
    }

    public enum FlagTypeOptionSet
    {
        Submittal = 100000000,
        Review = 100000001,
        FollowUp = 100000002,
        Message = 100000003
    }
}