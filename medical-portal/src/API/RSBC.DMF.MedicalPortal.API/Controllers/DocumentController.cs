using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using Google.Protobuf;
using Pssg.DocumentStorageAdapter;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;
using Winista.Mime;
using RSBC.DMF.MedicalPortal.API.Model;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly DocumentManager.DocumentManagerClient _documentManagerClient;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentController> _logger;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly DocumentFactory _documentFactory;


        public DocumentController(DocumentManager.DocumentManagerClient documentManagerClient, IUserService userService, IMapper mapper, IConfiguration configuration, ILoggerFactory loggerFactory, CaseManager.CaseManagerClient cmsAdapterClient, DocumentStorageAdapterClient documentStorageAdapterClient, DocumentFactory documentFactory)
        {
            _documentManagerClient = documentManagerClient;
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<DocumentController>();
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _documentFactory = documentFactory;
        }

        [HttpGet("MyDmers")]
        [ProducesResponseType(typeof(IEnumerable<CaseDocument>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMyDocumentsByType()
        {
            var profile = await _userService.GetCurrentUserContext();
            var loginIds = profile.LoginIds;

            var dmerDocumentTypeCode = _configuration["Constants:DmerDocumentTypeCode"];
            var request = new GetDocumentsByTypeForUsersRequest { DocumentTypeCode = dmerDocumentTypeCode, LoginIds = { loginIds } };
            var reply = _documentManagerClient.GetDocumentsByTypeForUsers(request);
            if (reply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                var caseDocuments = _mapper.Map<IEnumerable<CaseDocument>>(reply.Items);
                return Ok(caseDocuments);
            }
            else
            {
                _logger.LogError($"{nameof(GetMyDocumentsByType)} error: unable to get documents by type - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }
        }

        [HttpGet("GetDriverAndCaseDocuments")]
        [ProducesResponseType(typeof(IEnumerable<Document>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetDriverAndCaseDocuments")]
        public async Task<IActionResult> GetDriverAndCaseDocuments([FromRoute] string caseId)
        {
            var profile = await _userService.GetCurrentUserContext();
            var loginIds = profile.LoginIds;

            // TODO # Change the loginId to claim.loginid 
            var request = new GetDriverAndCaseDocumentsRequest { CaseId = caseId, LoginId = loginIds.FirstOrDefault() };

            var reply = _documentManagerClient.GetDriverAndCaseDocuments(request);
            if (reply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                // This includes all the documents except Open Required, Issued, Sent documents on Submission History Tab
                var replyItemsWithDocuments = reply.Items
                    .Where(i => !string.IsNullOrEmpty(i.DocumentUrl))
                    .Where(i => i.SubmittalStatus != "Open-Required" && i.SubmittalStatus != "Issued" && i.SubmittalStatus != "Sent");
                var result = _mapper.Map<List<CaseDocument>>(replyItemsWithDocuments);

                // Sort The documents

                if (result.Count > 0)
                {
                    result = result.OrderByDescending(doc => doc.CreatedOn).ToList();
                }

                return Json(result);
            }
            else
            {

                _logger.LogError($"{nameof(GetDriverAndCaseDocuments)} error: unable to get documents for this case - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }

        }

        /// <summary>
        /// Get all documents for a given driver but filter out documents without a url
        /// </summary>
        /// <returns>IEnumerable&lt;Document&gt;</returns>
        [HttpGet("{driverId}/AllDocuments")]
        [ProducesResponseType(typeof(IEnumerable<CaseDocument>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetAllDocuments")]
        public async Task<ActionResult> GetAllDocuments([FromRoute] string driverId)
        {
            var profile = await _userService.GetCurrentUserContext();

            var driverIdRequest = new DriverIdRequest() { Id = driverId };
            var reply = _cmsAdapterClient.GetDriverDocumentsById(driverIdRequest);
            if (reply != null && reply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                // This includes all the documents except Open Required, Issued, Sent documents on Submission History Tab
                var replyItemsWithDocuments = reply.Items;
             
                var result = _mapper.Map<List<CaseDocument>>(replyItemsWithDocuments);

                // sort the documents
                if (result.Count > 0)
                {
                    result = result.OrderByDescending(cs => cs.CreatedOn).ToList();
                }

                return Json(result);
            }
            else
            {
                _logger.LogError($"{nameof(GetAllDocuments)} failed for driverId: {driverId}", reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }
        }

        /// <summary>
        /// Upload Document Content
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpPost("upload")]     
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(UploadDriverDocument))]
        // NOTE an operation filter was started but the generated code was not completed
        // to save time, we opted to just write this api call manually on the frontend
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> UploadDriverDocument([FromForm] IFormFile file, [FromForm] int documentSubTypeId, [FromForm] string driverId)
        {
            // sanity check paramters
            if (string.IsNullOrEmpty(file?.FileName) || file.Length == 0)
            {
                return BadRequest();
            }

            // validate document sub type
            var documentIdRequest = new DocumentIdRequest();
            documentIdRequest.Id = documentSubTypeId;
            var documentSubTypeGuidReply = _documentManagerClient.GetDocumentSubTypeGuid(documentIdRequest);
            if (documentSubTypeGuidReply.ResultStatus != Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                _logger.LogError("ERROR getting document sub type id.\n {0}", documentSubTypeGuidReply.ErrorDetail);
                return StatusCode(500, documentSubTypeGuidReply.ErrorDetail);
            }

            var profile = await _userService.GetCurrentUserContext();

            // read file stream into byte array
            var fileStream = new MemoryStream((int)file.Length);
            file.CopyTo(fileStream);
            var bytes = fileStream.ToArray();

            // verify mimetype from byte array
            var mimeTypes = new MimeTypes();
            var mimeType = mimeTypes.GetMimeType(bytes);
            if (!DocumentUtils.IsAllowedMimeType(mimeType.Name))
            {
                _logger.LogError($"ERROR in uploading file due to invalid mime type {mimeType?.Name}");
                return BadRequest("Invalid file type.");
            }

            // add the document
            var request = new UploadFileRequest()
            {
                ContentType = file.ContentType,
                Data = ByteString.CopyFrom(bytes),
                EntityName = "dfp_driver",
                FileName = file.FileName,
                FolderName = driverId,
            };
            var fileReply = _documentStorageAdapterClient.UploadFile(request);
            if (fileReply.ResultStatus != Pssg.DocumentStorageAdapter.ResultStatus.Success)
            {
                return StatusCode(500, fileReply.ErrorDetail);
            }

            // create document and then link to driver
            var driver = new Driver();
            driver.Id = driverId;

            var document = _documentFactory.Create(driver, profile.Id, fileReply.FileName, "Submitted Document", _configuration["DRIVER_DOCUMENT_TYPE_CODE"]);
            document.DocumentSubTypeId = documentSubTypeGuidReply.Id.ToString();
            var result = _cmsAdapterClient.CreateUnsolicitedDocumentOnDriver(document);
            if (result.ResultStatus != Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                return StatusCode(500, result.ErrorDetail);
            }

            return Ok();
        }


    }
}
