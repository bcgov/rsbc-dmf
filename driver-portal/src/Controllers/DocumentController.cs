using AutoMapper;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
using System.Net;
using Winista.Mime;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    // DOMAIN documents, document storage

    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : Controller
    {
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentManager.DocumentManagerClient _documentManagerClient;
        private readonly DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly IUserService _userService;
        private readonly DocumentFactory _documentFactory;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(CaseManager.CaseManagerClient cmsAdapterClient, DocumentManager.DocumentManagerClient documentManagerClient, DocumentStorageAdapterClient documentStorageAdapterClient, IUserService userService, DocumentFactory documentFactory, IMapper mapper, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _cmsAdapterClient = cmsAdapterClient;
            _documentManagerClient = documentManagerClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _userService = userService;
            _documentFactory = documentFactory;
            _mapper = mapper;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<DocumentController>();
        }

        /// <summary>
        /// Download Document Content
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpGet("{documentId}")]
        [Authorize(Policy = Policy.Driver)]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(DownloadDocumentFile))]
        public async Task<ActionResult> DownloadDocumentFile([FromRoute] string documentId)
        {
            // call the back end
            var reply = _cmsAdapterClient.GetLegacyDocument(new LegacyDocumentRequest() { DocumentId = documentId });

            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // check driver is authorized to view document
                var profile = await _userService.GetCurrentUserContext();
                if (reply.Document.Driver?.Id != profile.DriverId)
                {
                    return StatusCode((int)HttpStatusCode.Unauthorized, $"Not authorized - you are not authorized to view document {documentId}");
                }
                
                if (!string.IsNullOrEmpty(reply.Document?.DocumentUrl))
                {
                    // fetch the file from S3
                    var downloadFileRequest = new DownloadFileRequest()
                    {
                        ServerRelativeUrl = reply.Document.DocumentUrl
                    };
                    var documentReply = _documentStorageAdapterClient.DownloadFile(downloadFileRequest);
                    if (documentReply.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
                    {
                        byte[] fileContents = documentReply.Data.ToByteArray();
                        string fileName = Path.GetFileName(reply.Document.DocumentUrl);
                        string mimetype = DocumentUtils.GetMimeType(fileName);
                        Response.Headers.ContentDisposition = new Microsoft.Extensions.Primitives.StringValues($"inline; filename={fileName}");
                        return new FileContentResult(fileContents, mimetype)
                        {
                            FileDownloadName = $"{fileName}"
                        };
                    }
                    else
                    {
                        _logger.LogError($"Unexpected error - unable to fetch file");
                        return StatusCode((int)HttpStatusCode.InternalServerError, "Unexpected error - unable to fetch file from storage");
                    }
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NotFound, $"Not found - document id {documentId} URL is missing from document object");
                }
            }
            else
            {
                _logger.LogError($"Unexpected error - unable to get document meta-data for id {documentId} - {reply.ErrorDetail}");
                return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail);
            }
        }

        /// <summary>
        /// Upload Document Content
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        [Authorize(Policy = Policy.Driver)]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(UploadDriverDocument))]
        // NOTE an operation filter was started but the generated code was not completed
        // to save time, we opted to just write this api call manually on the frontend
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> UploadDriverDocument([FromForm] IFormFile file, [FromForm] int documentSubTypeId)
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
            if (documentSubTypeGuidReply.ResultStatus != CaseManagement.Service.ResultStatus.Success)
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
                FolderName = profile.DriverId,
            };
            var fileReply = _documentStorageAdapterClient.UploadFile(request);
            if (fileReply.ResultStatus != Pssg.DocumentStorageAdapter.ResultStatus.Success)
            {
                return StatusCode(500, fileReply.ErrorDetail);
            }

            // create document and then link to driver
            var driver = new Driver();
            driver.Id = profile.DriverId;

            var document = _documentFactory.Create(driver, profile.Id, fileReply.FileName, "Submitted Document", _configuration["DRIVER_DOCUMENT_TYPE_CODE"]);
            document.DocumentSubTypeId = documentSubTypeGuidReply.Id.ToString();
            var result = _cmsAdapterClient.CreateUnsolicitedDocumentOnDriver(document);
            if (result.ResultStatus != CaseManagement.Service.ResultStatus.Success)
            {
                return StatusCode(500, result.ErrorDetail);
            }

            return Ok();
        }
    }
}
