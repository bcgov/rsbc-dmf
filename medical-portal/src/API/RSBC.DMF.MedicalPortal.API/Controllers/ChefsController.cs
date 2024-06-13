using System.Net;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using System.Security.Claims;
using UploadFileRequest = Pssg.DocumentStorageAdapter.UploadFileRequest;
using Google.Protobuf;
using Newtonsoft.Json;
using Pssg.DocumentStorageAdapter;
using RSBC.DMF.MedicalPortal.API.Utilities;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;
using ResultStatus = Pssg.DocumentStorageAdapter.ResultStatus;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChefsController : ControllerBase
    {
        private readonly ILogger<ChefsController> logger;
        private readonly IConfiguration configuration;
        private readonly ICaseQueryService caseQueryService;
        private readonly IUserService userService;
        private readonly CaseManager.CaseManagerClient cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient;

        private const string DATA_ENTITY_NAME = "incident";
        private const string DATA_FILENAME = "data.json";

        public ChefsController(ILogger<ChefsController> logger, IConfiguration configuration,
            IUserService userService,
            ICaseQueryService caseQueryService,
            CaseManager.CaseManagerClient cmsAdapterClient,
            DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.caseQueryService = caseQueryService;
            this.cmsAdapterClient = cmsAdapterClient;
            this.documentStorageAdapterClient = documentStorageAdapterClient;
            this.userService = userService;
        }

        [HttpGet("submission")]
        public async Task<ActionResult> GetSubmission([FromQuery] SubmissionStatus status = SubmissionStatus.Final)
        {
            UserContext profile = await this.userService.GetCurrentUserContext();
            logger.LogInformation($"GET Submission - SubmissionStatus: {status}, userId is {profile.Id}");

            string documentUrl = "";

            if (status == SubmissionStatus.Draft)
            {
                documentUrl = $"{DATA_ENTITY_NAME}/{profile.Id}/{DATA_FILENAME}";
            }
            else if (status == SubmissionStatus.Final)
            {
                documentUrl = $"dfp/triage-request/{profile.Id}.json";
            }

            if (documentUrl == "")
            {
                logger.LogError($"Unexpected error - unable to generate documentUrl");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    "Unexpected error - unable to generate documentUrl");
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
                return StatusCode((int)HttpStatusCode.NotFound,
                    "Not found error - unable to fetch file from storage");
            }

            byte[] fileContents = documentReply.Data.ToByteArray();
            string jsonContent = System.Text.Encoding.UTF8.GetString(fileContents);

            try
            {
                var jsonData = JsonConvert.DeserializeObject<ChefsSubmission>(jsonContent, new LowercaseEnumConverter());
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

        [HttpPut("submission")]
        public async Task<ActionResult> PutSubmission([FromBody] ChefsSubmission submission)
        {
            UserContext profile = await this.userService.GetCurrentUserContext();

            // get the practitioner ID
            string practitionerId = User.FindFirstValue("sid");

            SubmissionStatus status = submission.Status;

            logger.LogInformation($"PUT Submission - userId is {profile.Id}, practitionerId is {practitionerId}");

            var jsonString = JsonSerializer.Serialize(submission);
            logger.LogInformation($"ChefsSubmission payload: {jsonString}");

            UploadFileRequest jsonData = null;
            if (status == SubmissionStatus.Draft)
            {
                jsonData = new UploadFileRequest()
                {
                    ContentType = "application/json",
                    Data = ByteString.CopyFromUtf8(jsonString),
                    EntityName = DATA_ENTITY_NAME,
                    FileName = DATA_FILENAME,
                    FolderName = profile.Id
                };
            }
            else if (status == SubmissionStatus.Final)
            {
                jsonData = new UploadFileRequest()
                {
                    ContentType = "application/json",
                    Data = ByteString.CopyFromUtf8(jsonString),
                    EntityName = "dfp",
                    FileName = $"{profile.Id}.json",
                    FolderName = "triage-request"
                };
            }

            if (jsonData == null)
            {
                logger.LogError($"{nameof(PutSubmission)} error: unable to get upload jsonData");
                return StatusCode(500);
            }

            string dataFileKey = "";
            Int64 dataFileSize = 0;

            var reply = documentStorageAdapterClient.UploadFile(jsonData);

            if (reply.ResultStatus != ResultStatus.Success)
            {
                logger.LogError(
                    $"{nameof(PutSubmission)} error: unable to upload documents for this case - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }


            dataFileKey = reply.FileName;
            dataFileSize = jsonData.Data.Length;

            if (status == SubmissionStatus.Final)
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

            logger.LogInformation(
                $"PUT Submission - Successfully uploaded JSON to S3, dataFileKey: {dataFileKey}, dataFileSize: {dataFileSize}, reply: {JsonSerializer.Serialize(reply)}");

            return Ok(submission);
        }
    }
}