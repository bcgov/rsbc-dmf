using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Dto;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Rsbc.Dmf.CaseManagement
{
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
                    .Where(doc => 
                        doc.dfp_DocumentTypeID.dfp_code == documentTypeCode
                        && (doc._dfp_loginid_value != null && doc._dfp_loginid_value.Value == loginId)
                    ));
            }

            return _mapper.Map<IEnumerable<Document>>(documents);
        }

        public Document GetDmer(Guid caseId)
        {
            var document = dynamicsContext.bcgov_documenturls
                .Expand(doc => doc.bcgov_CaseId)
                .Expand(doc => doc.dfp_DocumentTypeID)
                .Expand(doc => doc.dfp_LoginId)
                .Where(doc => doc.bcgov_CaseId.incidentid == caseId && doc.dfp_DocumentTypeID.dfp_code == _configuration["CONSTANTS_DOCUMENT_TYPE_DMER"])
                .FirstOrDefault();

            return _mapper.Map<Document>(document);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginIds"></param>
        /// <returns></returns>
        public Document UpdateClaimDmer(Guid loginId, Guid documentId)
        {
            ResultStatusReply result = new ResultStatusReply()
            {
                Success = false
            };

            var querydocument = dynamicsContext.bcgov_documenturls
            .Expand(doc => doc.dfp_DocumentTypeID)
            .Where(doc => doc.dfp_DocumentTypeID.dfp_code == _configuration["CONSTANTS_DOCUMENT_TYPE_DMER"] && doc.bcgov_documenturlid == documentId).FirstOrDefault();

           
                if (querydocument != null)
                {
                    querydocument._dfp_loginid_value = loginId;
                }

            dynamicsContext.UpdateObject(querydocument);
            dynamicsContext.SaveChanges();
            dynamicsContext.DetachAll();
            result.Success = true;
            return _mapper.Map<Document>(querydocument);
        }

   
        public Document UpdateUnClaimDmer(Guid loginId, Guid documentId)
        {
            ResultStatusReply result = new ResultStatusReply()
            {
                Success = false
            };

            var querydocument = dynamicsContext.bcgov_documenturls
            .Expand(doc => doc.dfp_DocumentTypeID)
            .Where(doc => doc.dfp_DocumentTypeID.dfp_code == _configuration["CONSTANTS_DOCUMENT_TYPE_DMER"] && doc.bcgov_documenturlid == documentId).FirstOrDefault();


            if (querydocument != null)
            {
                querydocument._dfp_loginid_value = null;
            }

            dynamicsContext.UpdateObject(querydocument);
            dynamicsContext.SaveChanges();
            dynamicsContext.DetachAll();
            result.Success = true;
            return _mapper.Map<Document>(querydocument);
        }
       
    }
}


