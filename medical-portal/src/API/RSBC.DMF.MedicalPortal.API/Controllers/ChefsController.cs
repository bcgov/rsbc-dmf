using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using AutoMapper;
using UploadFileRequest = Pssg.DocumentStorageAdapter.UploadFileRequest;
using Google.Protobuf;
using Newtonsoft.Json;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.IcbcAdapter;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;
using CMSResultStatus = Rsbc.Dmf.CaseManagement.Service.ResultStatus;
using DocumentStorageResultStatus = Pssg.DocumentStorageAdapter.ResultStatus;
using Driver = RSBC.DMF.MedicalPortal.API.ViewModels.Driver;
using EmptyRequest = Rsbc.Dmf.CaseManagement.Service.EmptyRequest;
using ResultStatus = Rsbc.Dmf.IcbcAdapter.ResultStatus;
using Rsbc.Dmf.IcbcAdapter.Client;
using Microsoft.AspNetCore.Authorization;
using static RSBC.DMF.MedicalPortal.API.Auth.AuthConstant;
using Pssg.SharedUtils;
using System.Globalization;
using System.Globalization;


namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChefsController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ICachedIcbcAdapterClient icbcAdapterClient;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentManager.DocumentManagerClient _documentManagerClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient;
        private readonly IAuthorizationService _authorizationService;
        private readonly PdfService _pdfService;
        private readonly ILogger<ChefsController> logger;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        private const string DATA_ENTITY_NAME = "incident";
        private const string DATA_FILENAME = "data.json";

        public ChefsController(
            IUserService userService,
            CaseManager.CaseManagerClient cmsAdapterClient,
            DocumentManager.DocumentManagerClient documentManagerClient,
            ICachedIcbcAdapterClient icbcAdapterClient,
            DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient,
            IAuthorizationService authorizationService,
            PdfService pdfService,
            ILogger<ChefsController> logger,
            IMapper mapper,
            IConfiguration configuration)
        {
            _cmsAdapterClient = cmsAdapterClient;
            _documentManagerClient = documentManagerClient;
            this.documentStorageAdapterClient = documentStorageAdapterClient;
            this.userService = userService;
            this.icbcAdapterClient = icbcAdapterClient;
            _authorizationService = authorizationService;
            _pdfService = pdfService;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet("submission")]
        [ProducesResponseType(typeof(ChefsSubmission), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ActionName(nameof(GetSubmission))]
        public async Task<ActionResult> GetSubmission([FromQuery] string caseId,
            [FromQuery] string status = SubmissionStatus.Draft)
        {
            var profile = await this.userService.GetCurrentUserContext();

            logger.LogInformation($"GET Submission - SubmissionStatus: {status}, userId is {profile.Id}, caseId is ${caseId}");

            string documentUrl = "";

            if (status == SubmissionStatus.Draft)
            {
                documentUrl = $"{DATA_ENTITY_NAME}/{caseId}/{DATA_FILENAME}";
            }
            else if (status == SubmissionStatus.Final)
            {
                documentUrl = $"dfp/triage-request/{caseId}.json";
            }

            if (documentUrl == "")
            {
                logger.LogError($"Unexpected error - unable to generate documentUrl");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error - unable to generate documentUrl");
            }

            // fetch the file from S3
            var downloadFileRequest = new DownloadFileRequest()
            {
                ServerRelativeUrl = documentUrl
            };
            var documentReply = documentStorageAdapterClient.DownloadFile(downloadFileRequest);
            if (documentReply.ResultStatus != Pssg.DocumentStorageAdapter.ResultStatus.Success)
            {
                logger.LogError($"Not found error - unable to fetch file");
                return StatusCode(StatusCodes.Status404NotFound, "Not found error - unable to fetch file from storage");
            }

            byte[] fileContents = documentReply.Data.ToByteArray();
            string jsonContent = System.Text.Encoding.UTF8.GetString(fileContents);

            try
            {
                var jsonData = JsonConvert.DeserializeObject<ChefsSubmission>(jsonContent);
                string formattedJson = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
                logger.LogInformation("JSON Data: {0}", formattedJson);
                return Ok(jsonData);
            }
            catch (JsonException ex)
            {
                logger.LogError("Error deserializing JSON content: {0}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing JSON content");
            }
        }

        // TODO why is this triggered when loading the chefs form?
        [HttpPut("submission")]
        [ProducesResponseType(typeof(ChefsSubmission), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ActionName(nameof(PutSubmission))]
        public async Task<ActionResult> PutSubmission([FromQuery] string caseId, [FromQuery] string documentId, [FromBody] ChefsSubmission submission)
        {
            // invalid arguments
            if (
                submission == null ||
                (submission.Status != SubmissionStatus.Draft && submission.Status != SubmissionStatus.Final)
                || string.IsNullOrEmpty(caseId) || string.IsNullOrEmpty(documentId))
            {
                logger.LogError($"{nameof(PutSubmission)} error: invalid submission");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            

            // serialize and log the payload
            var jsonString = JsonSerializer.Serialize(submission);
            logger.LogInformation($"ChefsSubmission payload: {jsonString}");

            // to submit final, make sure they are licenced practitioner, otherwise submission should be a draft
            submission.Status = await CheckFinalSubmissionAuthorization(submission.Status);

            // prepare the JSON data
            var jsonUploadRequest = new UploadFileRequest()
            {
                ContentType = "application/json",
                Data = ByteString.CopyFromUtf8(jsonString),
            };
            if (submission.Status == SubmissionStatus.Draft)
            {
                jsonUploadRequest.EntityName = DATA_ENTITY_NAME;
                jsonUploadRequest.FileName = DATA_FILENAME;
                jsonUploadRequest.FolderName = caseId;
            }
            else if (submission.Status == SubmissionStatus.Final)
            {
                jsonUploadRequest.EntityName = "dfp"; 
                jsonUploadRequest.FileName = $"{caseId}.json"; // Document Type - DRiver (DL-Surname)
                jsonUploadRequest.FolderName = "triage-request";
            }

            // upload the JSON data to S3
            var jsonUploadReply = documentStorageAdapterClient.UploadFile(jsonUploadRequest);
            if (jsonUploadReply.ResultStatus != DocumentStorageResultStatus.Success)
            {
                logger.LogError($"{nameof(PutSubmission)} error: unable to upload documents for this case - {jsonUploadReply.ErrorDetail}");
                return StatusCode(StatusCodes.Status500InternalServerError, jsonUploadReply.ErrorDetail);
            }
            logger.LogInformation($"PUT Submission - Successfully uploaded JSON to S3, dataFileKey: {jsonUploadReply.FileName}, dataFileSize: {jsonUploadRequest.Data.Length}, reply: {JsonSerializer.Serialize(jsonUploadReply)}");

            if (submission.Status == SubmissionStatus.Final) 
            {
                // create a PDF version of the JSON data
                var pdfData = _pdfService.GeneratePdf(jsonString);

                // upload the PDF version to S3
                var pdfUploadRequest = new UploadFileRequest
                {
                    ContentType = "application/pdf",
                    Data = ByteString.CopyFrom(pdfData),
                    EntityName = "dfp",
                    FileName = $"{caseId}.pdf",
                    FolderName = "triage-request"
                };
                var pdfUploadReply = documentStorageAdapterClient.UploadFile(pdfUploadRequest);
                if (pdfUploadReply.ResultStatus != DocumentStorageResultStatus.Success)
                {
                    logger.LogError($"{nameof(PutSubmission)} error: unable to upload documents for this case - {pdfUploadReply.ErrorDetail}");
                    return StatusCode(StatusCodes.Status500InternalServerError, pdfUploadReply.ErrorDetail);
                }
                logger.LogInformation($"PUT Submission - Successfully uploaded PDF to S3, dataFileKey: {pdfUploadReply.FileName}, dataFileSize: {pdfData.Length}, reply: {JsonSerializer.Serialize(pdfUploadReply)}");

                // get a list of all available Case Flags 
                var getAllFlagsReply = _cmsAdapterClient.GetAllFlags(new EmptyRequest());
                if (getAllFlagsReply == null || getAllFlagsReply.Flags.Count == 0)
                {
                    logger.LogInformation("Could not find all flags in the CMS");
                    return StatusCode(StatusCodes.Status404NotFound, "Not found error - could not find all flags in the CMS");
                }

                var allCaseFlags = mapper.Map<IEnumerable<Flag>>(getAllFlagsReply.Flags);

                // if any Case Flags are present and active (true) in CHEFS, update Case and set IsCleanPass to false
                var matchedFlags = allCaseFlags
                    .Where(flag => submission.Flags.ContainsKey(flag.FormId) && (bool)submission.Flags[flag.FormId])
                    .ToArray();

                var updateCaseRequest = new UpdateCaseRequest();
                updateCaseRequest.IsDmer = true;
                updateCaseRequest.CaseId = caseId;
                updateCaseRequest.Priority = TranslatePriority( submission.Priority);
                updateCaseRequest.Assign = TranslateAssign(submission.Assign);
                // used to add Document linked to case for the JSON data S3 file
                updateCaseRequest.DataFileKey = jsonUploadReply.FileName;
                updateCaseRequest.DataFileSize = jsonUploadRequest.Data.Length;
                // used to add Document linked to case for the PDF version
                updateCaseRequest.PdfFileKey = pdfUploadReply.FileName;
                updateCaseRequest.PdfFileSize = pdfUploadRequest.Data.Length;

                foreach (var item in matchedFlags)
                {
                    updateCaseRequest.Flags.Add(mapper.Map<FlagItem>(item));
                }

                var caseResult = _cmsAdapterClient.UpdateCase(updateCaseRequest);

                // update DMER document status to "Under Reviewed" on success
                if (caseResult.ResultStatus == CMSResultStatus.Success)
                {
                    var UpdateDocumentRequest = new UpdateDocumentRequest
                    {
                        Id = documentId,
                        // TODO portals should be agnostic of Dynamics specific values, this should be an enum [translated in CMS] or string value
                        SubmittalStatus = (int)SubmittalStatus.UnderReview,
                        DocumentType = "DMER",
                    };
                    _documentManagerClient.UpdateDocument(UpdateDocumentRequest);
                }

                logger.LogInformation($"Case Update Result is {caseResult.ResultStatus}");
            }

            return Ok(submission);
        }

        [HttpGet("bundle")]
        [ProducesResponseType(typeof(ChefsBundle), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ActionName(nameof(GetChefsBundle))]
        public async Task<ActionResult> GetChefsBundle([Required][FromQuery] string caseId)
        {
            var chefsBundle = new ChefsBundle();
            var caseResult = new PatientCase();

            if (string.IsNullOrEmpty(caseId) || caseId == Guid.Empty.ToString())
            {
                return BadRequest("Bad caseId due to null or empty string.");
            }

            if (!string.IsNullOrEmpty(caseId) && !Guid.TryParse(caseId, out Guid parsedUuid))
            {
                return BadRequest("Bad caseId due to invalid Guid format.");
            }

            // set caseId to return to CHEFS
            chefsBundle.caseId = caseId;

            var c = _cmsAdapterClient.GetCaseDetail(new CaseIdRequest { CaseId = caseId });
            if (c != null && c.ResultStatus == CMSResultStatus.Success)
            {
                chefsBundle.patientCase = caseResult;
                caseResult.DriverLicenseNumber = c.Item.DriverLicenseNumber;
                chefsBundle.medicalConditions = mapper.Map<IEnumerable<MedicalCondition>>(c.Item.MedicalConditions);
                chefsBundle.dmerType = c.Item.DmerType;
            }
           
            var driverInfoReply = new DriverInfoReply();

            if (caseResult.DriverLicenseNumber != null)
            {
                var request = new DriverInfoRequest();
                request.DriverLicence = caseResult.DriverLicenseNumber;
                driverInfoReply = await icbcAdapterClient.GetDriverInfoAsync(request);
            }
            else
            {
                logger.LogInformation("Could not find DriverLicenseNumber in the case details");
                return StatusCode(StatusCodes.Status404NotFound, "Not found error - could not find case details or driver license number");
            }

            if (driverInfoReply.ResultStatus == ResultStatus.Success)
            {
                chefsBundle.driverInfo = new Driver()
                {
                    Name = driverInfoReply.GivenName + ' ' + driverInfoReply.Surname,
                    GivenName = driverInfoReply.GivenName,
                    Surname = driverInfoReply.Surname,
                    BirthDate = DateTime.TryParse(driverInfoReply.BirthDate, out var parsedDate)
                    ? parsedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture): string.Empty,
                    DriverLicenceNumber = caseResult.DriverLicenseNumber,
                    LicenceClass = driverInfoReply.LicenceClass,
                    Address = new Address()
                    {
                        Line1 = driverInfoReply.AddressLine1,
                        City = driverInfoReply.City,
                        Postal = driverInfoReply.Postal
                    }
                };
            }
            else
            {
                logger.LogInformation("Could not find icbc driver info details");
                return StatusCode(StatusCodes.Status404NotFound, "Not found error - could not find icbc driver info details");
            }

            return Ok(chefsBundle);
        }

        private async Task<string> CheckFinalSubmissionAuthorization(string submissionStatus)
        {
            if (submissionStatus == SubmissionStatus.Final)
            {
                var user = userService.GetUser();
                var authorizationResult = await _authorizationService.AuthorizeAsync(user, Policies.MedicalPractitioner);
                if (!authorizationResult.Succeeded)
                {
                    return SubmissionStatus.Draft;
                }
            }
            return submissionStatus;
        }

        private string TranslatePriority(string priority)
        {

            var statusMap = new Dictionary<string, string>()
            {
                {"PR", "Regular" },
                {"PC", "Critical Review" },
                {"PU", "Urgent / Immediate" },
            };

            if (priority != null && statusMap.ContainsKey(priority))
            {
                return statusMap[priority];
            }
            else
            {
                return priority;
            }
        }

        private string TranslateAssign(string Assign)
        {

            var statusMap = new Dictionary<string, string>()
            {
                {"EI", "Team - Intake" },
                {"EN", "Team - Nurse Case Manager" },
                {"EA", "Team - Adjudicator" },
            };

            if (Assign != null && statusMap.ContainsKey(Assign))
            {
                return statusMap[Assign];
            }
            else
            {
                return Assign;
            }
        }

    }
}

