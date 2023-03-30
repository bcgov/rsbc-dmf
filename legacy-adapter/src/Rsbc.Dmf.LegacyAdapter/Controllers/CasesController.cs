using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pssg.DocumentStorageAdapter;
using Pssg.Interfaces;
using Rsbc.Dmf.CaseManagement.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;



namespace Rsbc.Dmf.LegacyAdapter.Controllers
{

    /// <summary>
    /// Case Controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CasesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CasesController> _logger;


        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly IIcbcClient _icbcClient;

        /// <summary>
        /// Cases Controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="cmsAdapterClient"></param>
        /// <param name="documentStorageAdapterClient"></param>
        /// <param name="icbcClient"></param>
        public CasesController(ILogger<CasesController> logger, IConfiguration configuration, CaseManager.CaseManagerClient cmsAdapterClient, DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient, IIcbcClient icbcClient)
        {
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _logger = logger;
            _icbcClient = icbcClient;
        }

        /// <summary>
        /// DoesCaseExist
        /// </summary>
        /// <param name="licenseNumber"></param>
        /// <param name="surcode"></param>
        /// <returns>The Case Id or Null</returns>
        // GET: /Cases/Exist
        [HttpGet("Exist")]
        public ActionResult DoesCaseExist([Required] string licenseNumber, [Required] string surcode)
        {
            string caseId = GetCaseId( licenseNumber, surcode);
            
            if (caseId == null) // create it
            {
                try
                {
                    var driver = _icbcClient.GetDriverHistory(licenseNumber);
                    if (driver != null)
                    {
                        LegacyCandidateRequest legacyCandidateRequest = new LegacyCandidateRequest
                        {
                            LicenseNumber = licenseNumber,
                            EffectiveDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                            Surname = driver.INAM?.SURN ?? string.Empty
                        };
                        _cmsAdapterClient.ProcessLegacyCandidate(legacyCandidateRequest);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e,"Error getting driver.");
                }
                caseId = GetCaseId(licenseNumber, surcode);
            }
            
            return Json(caseId);
        }

        /// <summary>
        /// DoesCaseExist
        /// </summary>
        /// <param name="licenseNumber"></param>
        /// <returns>The Case Id or Null</returns>
        // GET: /Cases/ExistByDl
        [HttpGet("ExistByDl")]
        public ActionResult DoesCaseExistByDl([Required] string licenseNumber)
        {
            string caseId = GetCaseIdByDl(licenseNumber);

            if (caseId == null) // create it
            {
                try
                {
                    var driver = _icbcClient.GetDriverHistory(licenseNumber);
                    if (driver != null)
                    {
                        LegacyCandidateRequest legacyCandidateRequest = new LegacyCandidateRequest
                        {
                            LicenseNumber = licenseNumber,
                            EffectiveDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                            Surname = driver.INAM?.SURN ?? string.Empty
                        };
                        _cmsAdapterClient.ProcessLegacyCandidate(legacyCandidateRequest);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "Error getting driver.");
                }
                caseId = GetCaseIdByDl(licenseNumber);
            }

            return Json(caseId);
        }

        private string GetCaseId(string licenseNumber, string surcode)
        {
            string caseId = null;
            var reply = _cmsAdapterClient.Search(new SearchRequest { DriverLicenseNumber = licenseNumber ?? string.Empty });
            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                foreach (var item in reply.Items)
                {
                    if ((bool)(item.Driver?.Surname.StartsWith(surcode)))
                    {
                        caseId = item.CaseId;
                    }
                }
            }
            return caseId;
        }


        private string GetCaseIdByDl(string licenseNumber)
        {
            string caseId = null;
            var reply = _cmsAdapterClient.Search(new SearchRequest { DriverLicenseNumber = licenseNumber ?? string.Empty });
            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                foreach (var item in reply.Items)
                {
                    caseId = item.CaseId;
                    break;
                }
            }
            return caseId;
        }

        /// <summary>
        /// Get Comments for a case
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        // GET: /Cases/Exist
        [HttpGet("{caseId}/Comments")]
        [ProducesResponseType(typeof(List<ViewModels.Comment>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult GetComments([FromRoute] string caseId)
        
        {
            // call the back end

            var reply = _cmsAdapterClient.GetCaseComments(new CaseIdRequest() { CaseId = caseId });

            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // get the comments
                List<ViewModels.Comment> result = new List<ViewModels.Comment>();

                foreach (var item in reply.Items)
                {
                    // todo - get the driver details from ICBC, get the MedicalIssueDate from Dynamics
                    ViewModels.Driver driver = new ViewModels.Driver()
                    {
                        LicenseNumber = item.Driver.DriverLicenseNumber,
                        Flag51 = false,
                        LastName = item.Driver.Surname,
                        LoadedFromICBC = false,
                        MedicalIssueDate = DateTimeOffset.Now
                    };

                    result.Add(new ViewModels.Comment
                    {
                        CaseId = item.CaseId,
                        CommentDate = item.CommentDate.ToDateTimeOffset(),
                        CommentId = item.CommentId,
                        CommentText = item.CommentText,
                        CommentTypeCode = item.CommentTypeCode,
                        Driver = driver,
                        SequenceNumber = item.SequenceNumber,
                        UserId = item.UserId
                    });
                }
                return Json(result);
            }
            else
            {
                return StatusCode(500);
            }
            /*
            result.Add (new ViewModels.Comment() { CaseId = Guid.NewGuid().ToString(), CommentText = "SAMPLE TEXT", CommentTypeCode="W",  CommentDate = DateTime.Now, CommentId = Guid.NewGuid().ToString(),
                Driver = new ViewModels.Driver() { Flag51 = false, LastName = "LASTNAME", LicenseNumber = "01234567", LoadedFromICBC = false, MedicalIssueDate = DateTimeOffset.Now }, 
                SequenceNumber = 0, UserId = "TESTUSER" });
            */
        }

        /// <summary>
        /// Add a comment to a case
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost("{caseId}/Comments")]
        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult CreateComments([FromRoute] string caseId, [FromBody] ViewModels.Comment comment )
        {            
            // add the comment
            return CreatedAtAction("Comments", comment);
        }

        /// <summary>
        /// Get Documents
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        [HttpGet("{caseId}/Documents")]
        [ProducesResponseType(typeof(List<ViewModels.Document>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult GetDocuments([FromRoute] string caseId)
        {
            // call the back end

            var reply = _cmsAdapterClient.GetCaseDocuments(new CaseIdRequest() { CaseId = caseId });

            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // get the comments
                List<ViewModels.Document> result = new List<ViewModels.Document>();

                foreach (var item in reply.Items)
                {
                    // todo - get the driver details from ICBC, get the MedicalIssueDate from Dynamics
                    ViewModels.Driver driver = new ViewModels.Driver()
                    {
                        LicenseNumber = item.Driver.DriverLicenseNumber,
                        Flag51 = false,
                        LastName = item.Driver.Surname,
                        LoadedFromICBC = false,
                        MedicalIssueDate = DateTimeOffset.Now
                    };
                 
                    result.Add(new ViewModels.Document
                    {
                        CaseId = item.CaseId,
                        FaxReceivedDate = item.FaxReceivedDate.ToDateTimeOffset(),
                        ImportDate = item.ImportDate.ToDateTimeOffset(),
                        DocumentId = item.DocumentId,
                        Driver = driver,
                        SequenceNumber = item.SequenceNumber,
                        UserId = item.UserId
                    });
                }
                return Json(result);
            }
            else
            {
                return StatusCode(500,reply.ErrorDetail);
            }
            /*
            result.Add (new ViewModels.Comment() { CaseId = Guid.NewGuid().ToString(), CommentText = "SAMPLE TEXT", CommentTypeCode="W",  CommentDate = DateTime.Now, CommentId = Guid.NewGuid().ToString(),
                Driver = new ViewModels.Driver() { Flag51 = false, LastName = "LASTNAME", LicenseNumber = "01234567", LoadedFromICBC = false, MedicalIssueDate = DateTimeOffset.Now }, 
                SequenceNumber = 0, UserId = "TESTUSER" });
            */
        }

        /// <summary>
        /// Add Case Document
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="skipDpsProcessing"></param>
        /// <param name="driversLicense"></param>
        /// <param name="batchId"></param>
        /// <param name="faxReceivedDate"></param>
        /// <param name="importDate"></param>
        /// <param name="importID"></param>
        /// <param name="originatingNumber"></param>
        /// <param name="documentPages"></param>
        /// <param name="documentType"></param>
        /// <param name="documentTypeCode"></param>
        /// <param name="validationMethod"></param>
        /// <param name="validationPrevious"></param>
        /// <param name="file"></param>
        /// <param name="priority"></param>
        /// <param name="assign"></param>
        /// <param name="submittalStatus"></param>
        /// <param name="surcode"></param>
        /// <param name="envelopeId"></param>
        /// <returns></returns>
        [HttpPost("{caseId}/Documents")]
        // allow large uploads
        [DisableRequestSizeLimit]
        public async Task<IActionResult> AddCaseDocument([FromRoute] string caseId,  // GUID
            [FromForm] [Required] string driversLicense,  // Driver -> DL            
            [FromForm] string batchId,         // add to document entity
            [FromForm] string faxReceivedDateString,  // dfp_faxreceivedate
            [FromForm] string importDateString,  // dfp_dpsprocessingdate
            [FromForm] string importID, // add to document entity
            [FromForm] string originatingNumber, // dfp_faxnumber
            [FromForm] int? documentPages, // add to document entity
            [FromForm] string documentType, // dfp_documenttypeid
            [FromForm] string documentTypeCode, // "form type" in DPS; the code that will be used for a lookup.
            [FromForm] string validationMethod, // add to document entity
            [FromForm] string validationPrevious, // add to document entity
            [FromForm] IFormFile file,
            [FromForm] string priority = "Regular",
            [FromForm] string assign = null,
            [FromForm] string submittalStatus = null,
            [FromForm] string surcode = null,         // Driver -> Lastname
            [FromForm] string envelopeId = null
            )
        {
            DateTimeOffset faxReceivedDate  = DocumentUtils.ParseDpsDate(faxReceivedDateString);
            DateTimeOffset importDate= DocumentUtils.ParseDpsDate(importDateString);

            var debugObject = new { driversLicense = driversLicense, batchId = batchId, faxReceivedDate = faxReceivedDate, importDate = importDate,
                importID = importID,
                originatingNumber = originatingNumber,
                documentPages = documentPages,
                documentType = documentType,
                documentTypeCode = documentTypeCode,
                validationMethod = validationMethod,
                validationPrevious = validationPrevious,
                priority = priority,
                assign = assign,
                submittalStatus = submittalStatus,
                surcode = surcode
            };       
            
            //Log.Information(JsonConvert.SerializeObject(debugObject));

            var driver = new CaseManagement.Service.Driver()
            {
                DriverLicenseNumber = driversLicense,
                Address = new Address()
                {
                    City = String.Empty,
                    Line1 = String.Empty,
                    Line2 = String.Empty,
                    Postal =String.Empty
                },
                BirthDate = Timestamp.FromDateTimeOffset(new DateTimeOffset(1970,1,1,0,0,0,TimeSpan.Zero)),
                GivenName = String.Empty,
                Height = 0.0,
                Id = String.Empty,
                Middlename = String.Empty,
                Name = String.Empty,
                Seck = String.Empty ,
                Sex = String.Empty ,
                Surname = String.Empty ,
                Weight = 0.0
            };


            var driverRequest = new DriverLicenseRequest() { DriverLicenseNumber = driversLicense };
            var driverReply = _cmsAdapterClient.GetDriver(driverRequest);

            string driverId = "";

            if (driverReply.ResultStatus == CaseManagement.Service.ResultStatus.Success && driverReply.Items != null && driverReply.Items.Count > 0)
            {
                driverId = driverReply.Items.FirstOrDefault()?.Id;
            }

            if (faxReceivedDate == null)
            {
                faxReceivedDate = DateTimeOffset.Now;
            }

            if (importDate == null)
            {
                importDate = DateTimeOffset.Now;
            }
            

            SearchReply reply;
            if (caseId != null)
            {
                reply = _cmsAdapterClient.Search(new SearchRequest { CaseId = caseId });
            }
            else
            {
                reply = _cmsAdapterClient.Search(new SearchRequest { DriverLicenseNumber = driversLicense });
            }

             
            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {                
                // add the document
                var ms = new MemoryStream();
                if (file != null)
                {
                    file.OpenReadStream().CopyTo(ms);

                    string jsonFile = JsonConvert.SerializeObject(file);
                    Serilog.Log.Error($"AddCaseDocument - File is {jsonFile}");
                }
                else
                {
                    Serilog.Log.Error("AddCaseDocument - File is empty");
                }

                var data = ms.ToArray();
                string fileName = file?.FileName ?? "UnknownFile.pdf";

                // ensure there are no invalid characters.

                fileName = DocumentUtils.SanitizeKeyFilename(fileName);

                UploadFileRequest pdfData = new UploadFileRequest()
                {
                    ContentType = DocumentUtils.GetMimeType(fileName),
                    Data = ByteString.CopyFrom(data),
                    EntityName = "dfp_driver",
                    FileName = fileName,
                    FolderName = driverId,
                };
                var fileReply = _documentStorageAdapterClient.UploadFile(pdfData);


                if (fileReply.ResultStatus != Pssg.DocumentStorageAdapter.ResultStatus.Success
                    || string.IsNullOrEmpty(fileReply.FileName)) // do not proceed if the URL is empty
                {
                    return StatusCode(500, $"S3 Error - Filename is '{fileReply.FileName}', error is '{fileReply.ErrorDetail}'");
                }

                string legacyDocumentType = documentType ?? String.Empty;

                var document = new LegacyDocument()
                {
                    BatchId = batchId ?? String.Empty,
                    DocumentPages = documentPages ?? 1,
                    DocumentType = legacyDocumentType,
                    DocumentTypeCode = documentTypeCode ?? legacyDocumentType,
                    DocumentUrl = fileReply.FileName,
                    CaseId = caseId ?? string.Empty,
                    FaxReceivedDate = Timestamp.FromDateTimeOffset(faxReceivedDate),
                    ImportDate = Timestamp.FromDateTimeOffset(importDate),
                    ImportId = importID ?? string.Empty,

                    OriginatingNumber = originatingNumber ?? string.Empty,
                    Driver = driver,
                    ValidationMethod = validationMethod ?? string.Empty,
                    ValidationPrevious = validationPrevious ?? string.Empty
                };

                var result = _cmsAdapterClient.CreateLegacyCaseDocument(document);

                if (result.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                {
                    var actionName = nameof(AddCaseDocument);
                    var routeValues = new
                    {
                        driversLicence = driversLicense
                    };

                    return CreatedAtAction(actionName, routeValues, document);
                }
                else
                {
                    Serilog.Log.Error(result.ErrorDetail);
                    return StatusCode(500, result.ErrorDetail);
                }
            }
            else
            {
                Serilog.Log.Error("Unable to fetch Case details " + reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }

        }

    }
}
