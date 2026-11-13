using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using Microsoft.Xrm.Sdk;
using Rsbc.Dmf.CaseManagement.Dto;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.CaseManagement.Dynamics.Mapper;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
//using System.Xml.Linq;
//using Microsoft.Xrm.Sdk;
//using Microsoft.Xrm.Sdk.Query;

namespace Rsbc.Dmf.CaseManagement
{
    public enum DPSStatusOptionSet
    {
        Accept = 100000000,
        Reject = 100000001
    }

    public enum DpsPriorityOptionSet
    {
        Regular = 100000000,
        UrgentImmediate = 100000001,
        Expedited = 100000002,
        CriticalReview = 100000003
    }

    public enum AIProccessedStatus
    {
        NA = 100000000,
        InProgress = 100000001,
        Proccessed = 100000002,
        Error = 100000003
    }
    internal class DocumentManager : DocumentMapper, IDocumentManager
    {
        internal readonly DynamicsContext dynamicsContext;
        private readonly ILogger<DocumentManager> logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DocumentManager(DynamicsContext dynamicsContext, ILogger<DocumentManager> logger, IMapper mapper, IConfiguration configuration)
        {
            this.dynamicsContext = dynamicsContext;
            this.logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        public enum submittalStatusOptionSet
        {
            OpenRequired = 100000000,
            NonComply = 100000005,
            ActionNonCOmply = 100000007,
            Accept = 100000001,
            Reject = 100000004,
            CleanPass = 100000009,
            ManualPass = 100000012,
            Empty = 100000013,
            Reviewed = 100000003,
            Uploaded = 100000010
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
                                    // TODO should we use bcgov_receiveddate
                                    FaxReceivedDate = document.dfp_faxreceiveddate.GetValueOrDefault(),
                                    ImportDate = document.dfp_dpsprocessingdate.GetValueOrDefault(),
                                    ImportId = document.dfp_importid ?? string.Empty,
                                    OriginatingNumber = document.dfp_faxsender ?? string.Empty,
                                    ValidationMethod = document.dfp_validationmethod ?? string.Empty,
                                    ValidationPrevious = document.dfp_validationprevious ?? string.Empty,
                                    SequenceNumber = @case.dfp_dfcmscasesequencenumber.GetValueOrDefault(),
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
        /// Get Driver Legacy Documents by driver license
        /// </summary>
        /// <param name="driverLicenceNumber"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyDocument>> GetDriverLegacyDocuments(string driverLicenceNumber, bool includeEmpty)
        {
            var result = new List<LegacyDocument>();
            var driversRaw = dynamicsContext.dfp_drivers.Where(d => d.dfp_licensenumber == driverLicenceNumber && d.statecode == 0);

            if (driversRaw != null)
            {
                var drivers = driversRaw.ToList();
                foreach (var @driver in drivers)
                {
                    if (@driver != null)
                    {
                        var driverDocuments = dynamicsContext.bcgov_documenturls.Where(d => d._dfp_driverid_value == driver.dfp_driverid && d.statecode == 0).ToList();
                        foreach (var document in driverDocuments)
                        {
                            // only include documents that have a URL
                            if (!string.IsNullOrEmpty(document.bcgov_url) || includeEmpty)
                            {
                                await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.bcgov_documenturlid));
                                await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.dfp_DocumentTypeID));

                                // TODO replace with
                                // var legacyDocument = _mapper.Map<LegacyDocument>(document);
                                // add mapper for Driver to AutoMapperProfile
                                var legacyDocument = new LegacyDocument
                                {
                                    BatchId = document.dfp_batchid ?? string.Empty,
                                    DocumentPages = ConvertPagesToInt(document.dfp_documentpages),
                                    DocumentId = document.bcgov_documenturlid.ToString(),
                                    DocumentTypeCode = document.dfp_DocumentTypeID?.dfp_apidocumenttype ?? string.Empty,
                                    DocumentType = document.dfp_DocumentTypeID?.dfp_name ?? string.Empty,
                                    BusinessArea = ConvertBusinessAreaToString(document.dfp_DocumentTypeID?.dfp_businessarea),
                                    DocumentUrl = document.bcgov_url ?? string.Empty,
                                    // TODO should we use bcgov_receiveddate, see below
                                    FaxReceivedDate = document.dfp_faxreceiveddate.GetValueOrDefault(),
                                    ImportDate = document.dfp_dpsprocessingdate.GetValueOrDefault(),
                                    ImportId = document.dfp_importid ?? string.Empty,
                                    OriginatingNumber = document.dfp_faxsender ?? string.Empty,
                                    ValidationMethod = document.dfp_validationmethod ?? string.Empty,
                                    ValidationPrevious = document.dfp_validationprevious ?? string.Empty,
                                    SubmittalStatus = TranslateSubmittalStatusInt(document.dfp_submittalstatus)
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

                                result.Add(legacyDocument);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get Legacy Document
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<LegacyDocument> GetLegacyDocument(string documentId)
        {
            LegacyDocument legacyDocument = null;

            var document = dynamicsContext.bcgov_documenturls.Expand(x => x.dfp_DriverId).Where(d => d.bcgov_documenturlid == Guid.Parse(documentId)).FirstOrDefault();
            if (document != null)
            {
                dynamicsContext.LoadProperty(document, nameof(bcgov_documenturl.dfp_DriverId));
                legacyDocument = new LegacyDocument
                {
                    BatchId = document.dfp_batchid ?? string.Empty,
                    DocumentPages = ConvertPagesToInt(document.dfp_documentpages),
                    DocumentId = document.bcgov_documenturlid.ToString(),
                    DocumentTypeCode = document.dfp_DocumentTypeID?.dfp_name ?? string.Empty,
                    DocumentUrl = document.bcgov_url ?? string.Empty,
                    // 
                    FaxReceivedDate = document.bcgov_receiveddate.GetValueOrDefault(),
                    ImportDate = document.dfp_dpsprocessingdate.GetValueOrDefault(),
                    ImportId = document.dfp_importid ?? string.Empty,
                    OriginatingNumber = document.dfp_faxsender ?? string.Empty,
                    ValidationMethod = document.dfp_validationmethod ?? string.Empty,
                    ValidationPrevious = document.dfp_validationprevious ?? string.Empty,
                    SequenceNumber = null
                };

                if (document.createdon != null)
                {
                    legacyDocument.CreateDate = document.createdon.Value;
                }

                if (document.dfp_documentorigin != null)
                {
                    legacyDocument.Origin = TranslateDocumentOrigin(document.dfp_documentorigin.Value);
                }

                if (document._bcgov_caseid_value != null)
                {
                    legacyDocument.CaseId = document._bcgov_caseid_value.ToString();
                }

                if (document.dfp_attachmentnumber != null)
                {
                    legacyDocument.DpsDocumentId = document.dfp_attachmentnumber.Value;
                }

                if (document._bcgov_caseid_value != null)
                {
                    legacyDocument.CaseId = document._bcgov_caseid_value.ToString();
                }

                if (document.dfp_queue != null)
                {
                    legacyDocument.Queue = TranslateQueueCodeInt(document.dfp_queue.Value);
                }

                if (document.dfp_priority != null)
                {
                    legacyDocument.Priority = TranslatePriorityCode(document.dfp_priority.Value);
                }

                if (document.dfp_DriverId != null)
                {
                    legacyDocument.Driver = new Driver
                    {
                        Id = document.dfp_DriverId.dfp_driverid.ToString(),
                        DriverLicenseNumber = document.dfp_DriverId.dfp_licensenumber
                    };
                }
            }

            return legacyDocument;
        }

        public IEnumerable<Document> GetDocumentsByTypeForUsers(IEnumerable<Guid> loginIds, string documentTypeCode)
        {
            var documents = new List<bcgov_documenturl>();

            foreach (var loginId in loginIds)
            {
                documents.AddRange(dynamicsContext.bcgov_documenturls
                    .Expand(doc => doc.dfp_DocumentTypeID)
                    .Expand(doc => doc.bcgov_CaseId)
                    .Expand(doc => doc.bcgov_CaseId.customerid_contact)
                    .Expand(doc => doc.dfp_LoginId)
                    .Where(doc =>
                        doc.dfp_DocumentTypeID.dfp_code == documentTypeCode
                        && doc.dfp_LoginId.dfp_loginid == loginId
                    ));
            }

            return _mapper.Map<IEnumerable<Document>>(documents);
        }

        public Document GetDmer(Guid caseId)
        {


            var dmerdocumentsList = dynamicsContext.bcgov_documenturls
                .Expand(doc => doc.bcgov_CaseId)
                .Expand(doc => doc.dfp_DocumentTypeID)
                .Expand(doc => doc.dfp_LoginId)
                .Where(doc => doc.bcgov_CaseId.incidentid == caseId && doc.dfp_DocumentTypeID.dfp_code == _configuration["CONSTANTS_DOCUMENT_TYPE_DMER"]
                 && (doc.dfp_submittalstatus == (int)submittalStatusOptionSet.OpenRequired || doc.dfp_submittalstatus == (int)submittalStatusOptionSet.NonComply)) // Verify the documents have Open required or Non Comply Documents
                .FirstOrDefault();


            return _mapper.Map<Document>(dmerdocumentsList);

        }

        // NOTE you can add command properties similar to "SubmittalStatus", as long you check for not default, so that it does not break existing functionality
        public bool UpdateDocument(UpdateDocumentCommand request)
        {
            var document = dynamicsContext.bcgov_documenturls
                .Where(x => x.bcgov_documenturlid == request.Id)
                .FirstOrDefault();

            if (request.SubmittalStatus != default)
            {
                document.dfp_submittalstatus = request.SubmittalStatus;

            }

            document.dfp_dpspriority = TranslatePriorityCode(request.DpsPriority);
            document.dfp_queue = TranslateQueueCode(request.Queue);
            //document.dfp_DocumentTypeID.dfp_name = "DMER";

            dynamicsContext.UpdateObject(document);
            dynamicsContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginIds"></param>
        /// <returns></returns>
        public Document UpdateClaimDmer(Guid loginId, Guid documentId)
        {
            var result = new ResultStatusReply()
            {
                Success = false
            };

            var loggedinUser = dynamicsContext.dfp_logins
                .Where(login => login.dfp_loginid == loginId)
                .FirstOrDefault();

            var querydocument = dynamicsContext.bcgov_documenturls
                .Expand(doc => doc.dfp_DocumentTypeID)
                .Expand(doc => doc.dfp_LoginId)
                .Where(doc => doc.dfp_DocumentTypeID.dfp_code == _configuration["CONSTANTS_DOCUMENT_TYPE_DMER"] && doc.bcgov_documenturlid == documentId)
                .FirstOrDefault();
            if (querydocument != null && loggedinUser != null)
            {
                dynamicsContext.SetLink(querydocument, nameof(bcgov_documenturl.dfp_LoginId), loggedinUser);
                querydocument.dfp_LoginId = loggedinUser;
            }

            dynamicsContext.SaveChanges();
            dynamicsContext.Detach(querydocument);
            result.Success = true;

            return _mapper.Map<Document>(querydocument);
        }


        public Document UpdateUnClaimDmer(Guid loginId, Guid documentId)
        {
            ResultStatusReply result = new ResultStatusReply()
            {
                Success = false
            };

            var loggedinUser = dynamicsContext.dfp_logins.Where(login => login.dfp_loginid == loginId).FirstOrDefault();

            var querydocument = dynamicsContext.bcgov_documenturls
            .Expand(doc => doc.dfp_DocumentTypeID)
            .Where(doc => doc.dfp_DocumentTypeID.dfp_code == _configuration["CONSTANTS_DOCUMENT_TYPE_DMER"] && doc.bcgov_documenturlid == documentId).FirstOrDefault();

            if (querydocument != null)
            {
                dynamicsContext.EntityParameterSendOption = EntityParameterSendOption.SendFullProperties;

                dynamicsContext.SetLink(querydocument, nameof(bcgov_documenturl.dfp_LoginId), null);


            }

            //dynamicsContext.UpdateObject(querydocument);
            dynamicsContext.SaveChanges();

            dynamicsContext.EntityParameterSendOption = EntityParameterSendOption.SendOnlySetProperties;
            dynamicsContext.DetachAll();
            result.Success = true;
            return _mapper.Map<Document>(querydocument);
        }


        /// <summary>
        /// Get Driver Legacy Documents by driver id
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LegacyDocument>> GetDriverDocumentsById(Guid driverId)
        {
            var driverDocuments = dynamicsContext.bcgov_documenturls
                .Expand(d => d.dfp_DocumentTypeID)
                .Expand(sd => sd.dfp_DocumentSubType)
                .Expand(d => d.bcgov_CaseId)
                .Where(d => d._dfp_driverid_value == driverId && d.statecode == (int)Dynamics.EntityState.Active && d.dfp_showonportals != false)
            .ToList();

            return _mapper.Map<IEnumerable<LegacyDocument>>(driverDocuments);
        }

        private string _DMER_DOCUMENT_TYPE_CODE = "001";

        public async Task<List<DmerDocumentRecord>> GetDmerRecordsPaginated(DmerRecordsPaginatedRequest request)
        {
            string fetchXml = $@"
                    <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'  count='{request.Count}'>
                      <entity name='bcgov_documenturl'>
                        <attribute name='bcgov_documenturlid'/>
                        <attribute name='bcgov_filename'/>
                        <attribute name='bcgov_url'/>
                        <attribute name='dfp_queue'/>
                        <attribute name='dfp_priority'/>
                        <attribute name='dfp_submittalstatus'/>
                        <attribute name='dfp_faxreceiveddate'/>
                        <attribute name='createdon'/>
                        <attribute name='dfp_dmerstatus'/>
                        <attribute name='dfp_driverid'/>
                        <attribute name='dfp_documenttypeid'/>
                        <attribute name='bcgov_caseid'/>
                        <link-entity name='dfp_submittaltype' from='dfp_submittaltypeid' to='dfp_documenttypeid' link-type='inner'>
                          <filter type='and'>
                            <condition attribute='dfp_code' operator='eq' value='{_DMER_DOCUMENT_TYPE_CODE}'/> 
                          </filter>
                        </link-entity> 
                        <filter type='and'>
                          <condition attribute='dfp_driverid' operator='not-null'/>
                          <condition attribute='bcgov_caseid' operator='not-null'/>
                          <condition attribute='dfp_submittalstatus' operator='eq' value='100000010'/>
                          <filter type='or'>
                              <condition attribute='dfp_aiprocessedstatus' operator='eq' value='100000000'/>
                              <condition attribute='dfp_aiprocessedstatus' operator='null'/>
                          </filter>
                        </filter>
                      </entity>
                    </fetch>";

            var dmerDocumenturls = dynamicsContext.CreateQuery<bcgov_documenturl>("bcgov_documenturls")
                                                .AddQueryOption("fetchXml", fetchXml)
                                                .ToList();
            foreach (var document in dmerDocumenturls)
            {
                await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.dfp_DriverId));
                await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.dfp_dmerstatus));
                await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.dfp_DocumentTypeID));
                await dynamicsContext.LoadPropertyAsync(document, nameof(bcgov_documenturl.dfp_documenttypebusinessarea));

                
            }

            var dmerRecords = _mapper.Map<List<DmerDocumentRecord>>(dmerDocumenturls);

            foreach (var document in dmerRecords)
            {
                document.DmerDocumentDriver.Documents = await GetDmerDocumentsForDriver(document.DmerDocumentDriver.DriverId, document.DocumentRecordId);
                document.DmerDocumentCaseRecord = await GetDocumentCaseRecord(document.DmerCaseId);
            }

            return dmerRecords;
        }

        public async Task<ResultStatusReply> SetDmerAIProccessedStatus(SetDmerAIProccessedStatus request)
        {
            var result = new ResultStatusReply();
            try
            {
                var dmerDoc = dynamicsContext.bcgov_documenturls.Where(d => d.bcgov_documenturlid == new Guid(request.DocumenturlId)).FirstOrDefault();

                if (request.IsRejected)
                {
                    await RejectDocument(dmerDoc);
                    result.Success = true;
                    return result;
                }

                if(request.AIProccessedStatus == (int)AIProccessedStatus.Error)
                {
                    await ProccessDocumentError(dmerDoc, request);
                    result.Success = true;
                    return result;
                }
           
                dmerDoc.dfp_aiprocessedstatus = request.AIProccessedStatus;
                dmerDoc.dfp_aidpspriority = request.AIPriority != null ? request.AIPriority : dmerDoc.dfp_priority;
                dmerDoc.dfp_queue = request.Queue != null ? request.Queue : dmerDoc.dfp_queue;
                dmerDoc.dfp_aidmerstatus = request.DmerStatus != null ? request.DmerStatus : dmerDoc.dfp_dmerstatus;
                dmerDoc.dfp_notes = request.Notes ?? dmerDoc.dfp_notes;
                dynamicsContext.UpdateObject(dmerDoc);
                await dynamicsContext.SaveChangesAsync();

                result.Success = true;
            }
            catch(Exception ex)
            {
                throw (ex);

            }
            return result;
        }

        public async Task<ResultStatusReply> BatchSetDmerAIProccessedStatusInProgress(BatchSetDmerAIProccessedStatusInProgress request)
        {
            var result = new ResultStatusReply();
            try
            {
                foreach (var documenturlId in request.DocumenturlIds)
                {
                    var dmerDoc = dynamicsContext.bcgov_documenturls.Where(d => d.bcgov_documenturlid == new Guid(documenturlId)).FirstOrDefault();
                    dmerDoc.dfp_aiprocessedstatus = request.DmerAiStatus;
                    dynamicsContext.UpdateObject(dmerDoc);
                }
                await dynamicsContext.SaveChangesAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return result;
        }

        private async Task<List<DmerDriverDocument>> GetDmerDocumentsForDriver(string driverId, string documentUrlId)
        {
            var driverGuid = Guid.Parse(driverId);
            var driverDocuments = dynamicsContext.bcgov_documenturls
                .Expand(d=> d.dfp_DocumentTypeID)
                .Where(d => d._dfp_driverid_value == driverGuid 
                            && d.bcgov_documenturlid != new Guid(documentUrlId)  
                            && d.dfp_DocumentTypeID.dfp_code == _DMER_DOCUMENT_TYPE_CODE
                            && d.dfp_submittalstatus == (int)submittalStatusOptionSet.Uploaded)
                .ToList();
            return _mapper.Map<List<DmerDriverDocument>>(driverDocuments);
        }

        private async Task<DmerDocumentCaseRecord> GetDocumentCaseRecord(string caseId)
        {
            var caseGuid = Guid.Parse(caseId);
            var caseRecord = dynamicsContext.incidents
                .Expand(c => c.owninguser)
                .Expand(c => c.owningteam)
                .Where(d => d.incidentid == caseGuid)
                .First();

            return _mapper.Map<DmerDocumentCaseRecord>(caseRecord);
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
                { "Urgent/ Immediate",  100000001 },
                { "Critical Review" , 100000003},
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
        /// 
        /// </summary>
        /// <param name="priorityCode"></param>
        /// <returns></returns>
        private int TranslateQueueCode(string queueCode)
        {
            var statusMap = new Dictionary<string, int>()
            {
                { "Team - Intake", 100000000 },
                { "Team - Adjudicators",  100000001 },
                { "Team - Case Managers" ,  100000002},
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
        private async Task RejectDocument(bcgov_documenturl doc)
        {
            doc.dfp_dmerstatus = (int)DMERStatusOptionSet.Reject;
            doc.dfp_dpsstatus = (int)DPSStatusOptionSet.Reject;
            doc.dfp_dpspriority = (int)DpsPriorityOptionSet.Regular;
            dynamicsContext.UpdateObject(doc);
            await dynamicsContext.SaveChangesAsync();
        }

        private async Task ProccessDocumentError(bcgov_documenturl dmerDoc, SetDmerAIProccessedStatus request)
        {
            dmerDoc.dfp_notes = request.Notes;
            dmerDoc.dfp_aiprocessedstatus = request.AIProccessedStatus;
            dynamicsContext.UpdateObject(dmerDoc);
            await dynamicsContext.SaveChangesAsync();
        }

        /// <summary>
        /// Add driver to AI DMER document
        /// Checks if driver exists; if not, creates one. Links driver to document.
        /// Returns any open case linked to the driver.
        /// </summary>
        /// <param name="request">DmerDriverRequest with driver details and document ID</param>
        /// <returns>AddDriverToAiDmerDocumentReply with error status, message, and case ID if found</returns>
        public async Task<AddDriverToAiDmerDocumentReply> AddDriverToAiDmerDocument(DmerDriverRequest request)
        {
            var reply = ValidateAddDriverRequest(request);
            if(!reply.Success)
            {
                return reply;
            }

            try
            {
                Guid driverId = Guid.Empty;
                dfp_driver driver = null;

                driver = dynamicsContext.dfp_drivers
                    .Where(d => d.dfp_licensenumber == request.DriverLicenceNumber && d.statecode == 0)
                    .FirstOrDefault();

                if (driver == null)
                {
                    driver = await CreateDriver(request);
                }
                else
                {
                    driverId = driver.dfp_driverid.Value;
                }

                await LinkDriverWithDocument(request, driver);

                try
                {
                    var dmerCaseTypeCode = 2;
                    var openCase = dynamicsContext.incidents
                        .Where(c => c._dfp_driverid_value == driverId && c.statecode == 0 && c.casetypecode == dmerCaseTypeCode)
                        .FirstOrDefault();

                    if (openCase != null)
                    {
                        reply.DriverOpenCase = _mapper.Map<DriverOpenCase>(openCase);
                    }
                }
                catch (Exception ex)
                {
                    reply.Success = true;
                    reply.ReplyMessage = $"Unexpected retrieving drivers open case: {ex.Message}";
                    return reply;
                }
                
                reply.Success = true;
                reply.ReplyMessage = "Driver successfully linked to document";
                return reply;
            }
            catch (Exception ex)
            {
                logger.LogError($"Unexpected error in AddDriverToAiDmerDocument: {ex.Message}");
                reply.Success = false;
                reply.ReplyMessage = $"Unexpected error Adding driver to Dmer document: {ex.Message}";
                return reply;
            }
        }


        private AddDriverToAiDmerDocumentReply ValidateAddDriverRequest(DmerDriverRequest request)
        {
            var reply = new AddDriverToAiDmerDocumentReply
            {
                Success = true,
            };
            // Validate input
            if (string.IsNullOrWhiteSpace(request?.DriverLicenceNumber))
            {
                reply.Success = false;
                reply.ReplyMessage = "Driver license number is required";
                return reply;
            }

            if (request.DriverLicenceNumber.Length > 8)
            {
                reply.Success = false;
                reply.ReplyMessage = "Driver license number cannot exceed 8 characters";
                return reply;
            }

            if (!request.DriverLicenceNumber.All(char.IsDigit))
            {
                reply.Success = false;
                reply.ReplyMessage = "Driver license number must contain only numeric digits";
                return reply;
            }

            if (string.IsNullOrWhiteSpace(request.DmerId))
            {
                reply.Success = false;
                reply.ReplyMessage = "Document ID is required";
                return reply;
            }
            return reply;
        }

        private async Task LinkDriverWithDocument(DmerDriverRequest request, dfp_driver driver)
        {
            try
            {
                var document = dynamicsContext.bcgov_documenturls
                    .Where(d => d.bcgov_documenturlid == Guid.Parse(request.DmerId))
                    .FirstOrDefault();

                if (document == null)
                {
                    throw(new Exception("Document not found"));
                }

                document._dfp_driverid_value = driver.dfp_driverid;
                dynamicsContext.SetLink(document, nameof(bcgov_documenturl.dfp_DriverId), driver);
                await dynamicsContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error linking driver to document: {ex.Message}");
                throw(new Exception($"Failed to link driver to document: {ex.Message}"));

            }
        }

        private async Task<dfp_driver> CreateDriver(DmerDriverRequest request)
        {
            try
            {
                // Create driver using existing pattern
                string randomDriverId = string.Format("e27d7c69-3913-4116-a360-f5e99972b7e8");
                string driverSubId = randomDriverId.Substring(0, randomDriverId.Length - request.DriverLicenceNumber.Length);
                var driverId = Guid.Parse(driverSubId + request.DriverLicenceNumber);

                // Create driver contact
                string contactIdString = "FCBCE0AC-82EF-411D-BD95-DB84D5E3D927";
                string contactSubId = contactIdString.Substring(0, contactIdString.Length - request.DriverLicenceNumber.Length);
                var contactId = new Guid(contactSubId + request.DriverLicenceNumber);

                contact driverContact = null;

                try
                {
                    driverContact = dynamicsContext.contacts
                        .Where(x => x.contactid == contactId)
                        .FirstOrDefault();
                }
                catch
                {
                    driverContact = null;
                }


                if (driverContact == null)
                {
                    driverContact = new contact()
                    {
                        contactid = contactId,
                        lastname = request.Surname ?? string.Empty,
                        firstname = request.FirstName ?? string.Empty,
                    };

                    if (request.DateOfBirth.HasValue && request.DateOfBirth > new DateTime(1900, 1, 1))
                    {
                        driverContact.birthdate = new Microsoft.OData.Edm.Date(
                            request.DateOfBirth.Value.Year,
                            request.DateOfBirth.Value.Month,
                            request.DateOfBirth.Value.Day);
                    }

                    dynamicsContext.AddTocontacts(driverContact);
                    await dynamicsContext.SaveChangesAsync();
                }

                // Create the driver entity
                var driver = new dfp_driver()
                {
                    dfp_driverid = driverId,
                    dfp_licensenumber = request.DriverLicenceNumber,
                    statuscode = 1,
                };

                if (request.DateOfBirth.HasValue && request.DateOfBirth > new DateTime(1900, 1, 1))
                {
                    driver.dfp_dob = request.DateOfBirth.Value;
                }

                dynamicsContext.AddTodfp_drivers(driver);
                dynamicsContext.SetLink(driver, nameof(dfp_driver.dfp_PersonId), driverContact);

                await dynamicsContext.SaveChangesAsync();
                return driver;
            }
            catch (Exception ex)
            {
                logger.LogError($"\nTrace: DocumentDriver.CreateDriver \nError creating driver: {ex.Message}");
                throw(new Exception($"{ex.Message}"));
            }
        }

    }

}
