using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Pssg.Rsbc.Dmf.DocumentTriage;
using UploadFileRequest = Pssg.DocumentStorageAdapter.UploadFileRequest;
using System.Security.AccessControl;
using RSBC.DMF.MedicalPortal.API.ViewModels;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChefsController : ControllerBase
    {
        private readonly ICaseQueryService caseQueryService;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        // private readonly ILogger<ChefsController> logger;

        public ChefsController(ICaseQueryService caseQueryService, CaseManager.CaseManagerClient cmsAdapterClient)
        {
            this.caseQueryService = caseQueryService;
            _cmsAdapterClient = cmsAdapterClient;
        }

        [HttpPut("submission")]
        public IActionResult PutSubmission([FromBody] ChefsSubmission submission)
        {
            // get the practitioner ID
            string practitionerId = User.FindFirstValue("sid");
            practitionerId = null;

            Serilog.Log.Logger.Information($"PUT Submission - practitionerId is {practitionerId}");

            // Access variables dynamically
            foreach (var kvp in submission.Submission)
            {
                var variableName = kvp.Key;
                var variableValue = kvp.Value;
                // Handle variable as needed
            }

            // using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            // {
            //     string body = await reader.ReadToEndAsync();
            //
            //     // FhirJsonParser can return a typed object.
            //     // There is also a more generic FhirJsonNode.Parse
            //     // The built in Json Deserializer does not seem to work
            //
            //     var parser = new FhirJsonParser();
            //     var bundle = parser.Parse<Bundle>(body);
            //
            //     string dataFileKey = "";
            //     string pdfFileKey = "";
            //     Int64 dataFileSize = 0;
            //     Int64 pdfFileSize = 0;
            //
            //     logger.LogInformation(bundle.ToJson());
            //
            //     // AFIAK there are no files yet from CHEFS that we need to process
            //     // first pass to get the files.
            //     // foreach (var entry in bundle.Entry)
            //     // {
            //     //   // find the PDF entry
            //     //   if (entry.Resource.ResourceType == ResourceType.Binary &&
            //     //       ((Binary)entry.Resource).ContentType == "application/pdf")
            //     //   {
            //     //     var b = (Binary)entry.Resource;
            //     //     UploadFileRequest pdfData = new UploadFileRequest()
            //     //     {
            //     //       ContentType = "application/pdf",
            //     //       Data = ByteString.CopyFrom(b.Data),
            //     //       EntityName = "incident",
            //     //       FileName = $"DMER.pdf",
            //     //       FolderName = bundle.Id,
            //     //     };
            //
            //     //     var reply = _documentStorageAdapterClient.UploadFile(pdfData);
            //     //     pdfFileKey = reply.FileName;
            //     //     pdfFileSize = pdfData.Data.Length;
            //     //   }
            //
            //     //   if (entry.Resource.ResourceType == ResourceType.Binary &&
            //     //       ((Binary)entry.Resource).ContentType == "application/eforms")
            //     //   {
            //     //     var b = (Binary)entry.Resource;
            //     //     UploadFileRequest jsonData = new UploadFileRequest()
            //     //     {
            //     //       ContentType = "application/json",
            //     //       Data = ByteString.CopyFrom(b.Data),
            //     //       EntityName = DATA_ENTITY_NAME,
            //     //       FileName = DATA_FILENAME,
            //     //       FolderName = bundle.Id
            //     //     };
            //
            //     //     var reply = _documentStorageAdapterClient.UploadFile(jsonData);
            //     //     dataFileKey = reply.FileName;
            //     //     dataFileSize = jsonData.Data.Length;
            //     //   }
            //     // }
            //
            //     // second pass to handle the questionnaire response
            //     foreach (var entry in bundle.Entry)
            //     {
            //
            //         if (entry.Resource.ResourceType == ResourceType.QuestionnaireResponse)
            //         {
            //             var questionnaireResponse = (QuestionnaireResponse)entry.Resource;
            //             // only triage completed items.
            //             if (questionnaireResponse.StatusElement.Value ==
            //                 QuestionnaireResponse.QuestionnaireResponseStatus.Completed)
            //             {
            //
            //                 // convert the questionnaire response into json.
            //                 TriageRequest triageRequest = new TriageRequest()
            //                 {
            //                     Processed = false,
            //                     TimeCreated = Timestamp.FromDateTime(DateTimeOffset.Now.UtcDateTime),
            //                     Id = bundle.Id,
            //                     PdfFileKey = pdfFileKey,
            //                     PdfFileSize = pdfFileSize,
            //                     DataFileKey = dataFileKey,
            //                     DataFileSize = dataFileSize,
            //                     PractitionerId = practitionerId == null ? "" : practitionerId,
            //                     ClinicId = "" // in order to pass this by value we would need some way to pass the data to the PHSA controller.
            //                 };
            //
            //                 triageRequest.AddItems(questionnaireResponse.Item);
            //
            //                 // unlike the others this file is saved into a "folder" that can be used for queueing.
            //                 // S3 does not use folders like a file system, it is simple a convention for the key.
            //                 string jsonString = JsonConvert.SerializeObject(triageRequest);
            //                 UploadFileRequest jsonData = new UploadFileRequest()
            //                 {
            //                     ContentType = "application/json",
            //                     Data = ByteString.CopyFromUtf8(jsonString),
            //                     EntityName = "dfp",
            //                     FileName = $"{bundle.Id}.json",
            //                     FolderName = "triage-request"
            //                 };
            //
            //                 // save a copy in the S3.
            //                 _documentStorageAdapterClient.UploadFile(jsonData);
            //
            //                 // and send to the triage service.
            //                 _documentTriageClient.Triage(triageRequest);
            //             }
            //             else // it is a save as draft.
            //             {
            //                 // No additional logic required for save as draft at this time.
            //             }
            //         }
            //     }
            //
            //
            // }

            return Ok(submission);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<DmerCaseListItem>>> GetCases([FromQuery] CaseSearchQuery query)
        {
            var cases = await caseQueryService.SearchCases(query);

            // second pass to populate birthdate.

            return Ok(cases);
        }
    }
}