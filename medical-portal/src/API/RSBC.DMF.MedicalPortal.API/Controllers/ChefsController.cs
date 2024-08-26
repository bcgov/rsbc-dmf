using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using System.Security.Claims;
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

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChefsController : ControllerBase
    {
        private readonly ILogger<ChefsController> logger;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IUserService userService;
        private readonly ICachedIcbcAdapterClient icbcAdapterClient;
        private readonly CaseManager.CaseManagerClient cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient;
        private readonly IAuthorizationService _authorizationService;

        private const string DATA_ENTITY_NAME = "incident";
        private const string DATA_FILENAME = "data.json";

        public ChefsController(
            ILogger<ChefsController> logger,
            IMapper mapper,
            IConfiguration configuration,
            IUserService userService,
            CaseManager.CaseManagerClient cmsAdapterClient,
            ICachedIcbcAdapterClient icbcAdapterClient,
            DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient,
            IAuthorizationService authorizationService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
            this.cmsAdapterClient = cmsAdapterClient;
            this.documentStorageAdapterClient = documentStorageAdapterClient;
            this.userService = userService;
            this.icbcAdapterClient = icbcAdapterClient;
            _authorizationService = authorizationService;
        }

        [HttpGet("submission")]
        [ProducesResponseType(typeof(ChefsSubmission), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
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
                return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error - unable to generate documentUrl");
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
                return StatusCode((int)HttpStatusCode.NotFound, "Not found error - unable to fetch file from storage");
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
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error processing JSON content");
            }
        }

        // TODO why is this triggered when loading the chefs form?
        [HttpPut("submission")]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName(nameof(PutSubmission))]
        public async Task<ActionResult> PutSubmission([FromQuery] string caseId, [FromQuery] string documentId, [FromBody] ChefsSubmission submission)
        {
            var profile = await this.userService.GetCurrentUserContext();
            logger.LogInformation($"PUT Submission - userId is {profile.Id}");

            var jsonString = JsonSerializer.Serialize(submission);
            logger.LogInformation($"ChefsSubmission payload: {jsonString}");

            // to submit final, make sure they are licenced practitioner, otherwise submission should be a draft
            submission.Status = await CheckFinalSubmissionAuthorization(submission.Status);

            UploadFileRequest jsonData = null;
            if (submission.Status == SubmissionStatus.Draft)
            {
                jsonData = new UploadFileRequest()
                {
                    ContentType = "application/json",
                    Data = ByteString.CopyFromUtf8(jsonString),
                    EntityName = DATA_ENTITY_NAME,
                    FileName = DATA_FILENAME,
                    FolderName = caseId
                };
            }
            else if (submission.Status == SubmissionStatus.Final)
            {
                // TODO clean up these comments and use these values on a new service e.g. UpdateDmer
                string chefsAssign = submission.Assign;         // Queue? e.g. Team - Intake, Team - Adjudicator, Team - Nurse Case Manager
                string chefsPriority = submission.Priority;     // DPS Priority e.g. Regular (prioritycode - 1 Critical Review, 2 Regular, 3 Urgent, 4 Expedited 100,000,000)
                // TODO these are just here for reference, could use some of these lines in the UpdateDmer service or maybe something already exists
                //bcgovDocumentUrl.dfp_priority = TranslatePriorityCode(request.Priority);
                //bcgovDocumentUrl.dfp_issuedate = DateTimeOffset.Now;
                //bcgovDocumentUrl.dfp_dpspriority = TranslatePriorityCode(request.Priority);
                //bcgovDocumentUrl.dfp_documentorigin = TranslateDocumentOrigin(request.Origin);
                //bcgovDocumentUrl.dfp_queue = TranslateQueueCode(request.Queue);

                // get a list of all available Case Flags 
                var getAllFlagsReply = cmsAdapterClient.GetAllFlags(new EmptyRequest());
                if (getAllFlagsReply == null || getAllFlagsReply.Flags.Count == 0)
                {
                    logger.LogInformation("Could not find all flags in the CMS");
                    return StatusCode((int)HttpStatusCode.NotFound, "Not found error - could not find all flags in the CMS");
                }

                var allCaseFlags = mapper.Map<IEnumerable<Flag>>(getAllFlagsReply.Flags);

                // if any Case Flags are present and active (true) in CHEFS, update Case and set IsCleanPass to false
                var matchedFlags = allCaseFlags
                    .Where(flag => submission.Flags.ContainsKey(flag.FormId) && (bool)submission.Flags[flag.FormId])
                    .ToArray();

                var updateCaseRequest = new UpdateCaseRequest()
                {
                    CaseId = caseId,
                };

                foreach (var item in matchedFlags)
                {
                    updateCaseRequest.Flags.Add(mapper.Map<FlagItem>(item));
                }

                var caseResult = cmsAdapterClient.UpdateCase(updateCaseRequest);

                logger.LogInformation($"Case Update Result is {caseResult.ResultStatus}");

                jsonData = new UploadFileRequest()
                {
                    ContentType = "application/json",
                    Data = ByteString.CopyFromUtf8(jsonString),
                    EntityName = "dfp",
                    FileName = $"{caseId}.json",
                    FolderName = "triage-request"
                };
            }

            if (jsonData == null)
            {
                logger.LogError($"{nameof(PutSubmission)} error: unable to get upload jsonData");
                return StatusCode(500);
            }

            var dataFileKey = "";
            Int64 dataFileSize = 0;

            var reply = documentStorageAdapterClient.UploadFile(jsonData);
            if (reply.ResultStatus != DocumentStorageResultStatus.Success)
            {
                logger.LogError($"{nameof(PutSubmission)} error: unable to upload documents for this case - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }

            // TODO Create a PDF based on jsonData, user friendly values is not in scope of initial version
            // Upload a copy of the PDF to S3
            // UploadPDF reuse logic of UploadJson

            dataFileKey = reply.FileName;
            dataFileSize = jsonData.Data.Length;

            if (submission.Status == SubmissionStatus.Final)
            {
                // TriageRequest triageRequest = new TriageRequest()
                // {
                //     Processed = false,
                //     TimeCreated = Timestamp.FromDateTime(DateTimeOffset.Now.UtcDateTime),
                //     Id = profile.Id,
                //     DataFileKey = dataFileKey,
                //     DataFileSize = dataFileSize,
                //     PractitionerId = practitionerId == null ? "" : practitionerId,
                //     ClinicId = "" // in order to pass this by value we would need some way to pass the data to the PHSA controller.
                // };
            }

            logger.LogInformation($"PUT Submission - Successfully uploaded JSON to S3, dataFileKey: {dataFileKey}, dataFileSize: {dataFileSize}, reply: {JsonSerializer.Serialize(reply)}");

            return Ok(submission);
        }

        [HttpGet("bundle")]
        [ProducesResponseType(typeof(ChefsBundle), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
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

            var c = cmsAdapterClient.GetCaseDetail(new CaseIdRequest { CaseId = caseId });
            if (c != null && c.ResultStatus == CMSResultStatus.Success)
            {
                chefsBundle.patientCase = caseResult;
                caseResult.DriverLicenseNumber = c.Item.DriverLicenseNumber;
                chefsBundle.medicalConditions = mapper.Map<IEnumerable<MedicalCondition>>(c.Item.MedicalConditions);
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
                return StatusCode((int)HttpStatusCode.NotFound,
                    "Not found error - could not find case details or driver license number");
            }

            if (driverInfoReply.ResultStatus == ResultStatus.Success)
            {
                chefsBundle.driverInfo = new Driver()
                {
                    Name = driverInfoReply.GivenName + ' ' + driverInfoReply.Surname,
                    GivenName = driverInfoReply.GivenName,
                    Surname = driverInfoReply.Surname,
                    BirthDate = driverInfoReply.BirthDate,
                    DriverLicenceNumber = caseResult.DriverLicenseNumber,
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
                return StatusCode((int)HttpStatusCode.NotFound,
                    "Not found error - could not find icbc driver info details");
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
    }
}