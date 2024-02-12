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
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : Controller
    {
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly IUserService _userService;
        private readonly DocumentFactory _documentFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<DriverController> _logger;

        public DocumentController(CaseManager.CaseManagerClient cmsAdapterClient, DocumentStorageAdapterClient documentStorageAdapterClient, IUserService userService, DocumentFactory documentFactory, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _userService = userService;
            _documentFactory = documentFactory;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<DriverController>();
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
        [ActionName(nameof(GetDocument))]
        public async Task<ActionResult> GetDocument([FromRoute] string documentId)
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
                        _logger.LogInformation($"Sending DocumentID {documentId} file {reply.Document.DocumentUrl} data size {fileContents?.Length}");
                        return new FileContentResult(fileContents, mimetype)
                        {
                            FileDownloadName = $"{fileName}"
                        };
                    }
                    else
                    {
                        _logger.LogError($"Unexpected error - unable to fetch file from storage  id - {documentId} - {reply.ErrorDetail}");
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
        public async Task<IActionResult> UploadDriverDocument([FromForm] IFormFile file)
        {
            // sanity check paramters
            if (string.IsNullOrEmpty(file?.FileName) || file.Length == 0)
            {
                return BadRequest();
            }

            var profile = await _userService.GetCurrentUserContext();

            var data = DocumentUtils.GetByteArray(file);
            var mimeTypes = new MimeTypes();
            var mimeType = mimeTypes.GetMimeType(data);
            if (mimeType == null || !(mimeType.Name.Equals("image/png") || mimeType.Name.Equals("image/jpeg") ||
                                     mimeType.Name.Equals("application/pdf")))
            {
                _logger.LogError($"ERROR in uploading file due to invalid mime type {mimeType?.Name}");
                return BadRequest();
            }

            // add the document
            var request = new UploadFileRequest()
            {
                ContentType = file.ContentType,
                Data = ByteString.CopyFrom(data),
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
            var driver = new CaseManagement.Service.Driver();
            driver.Id = profile.DriverId;
            var document = _documentFactory.Create(driver, profile.Id);
            var result = _cmsAdapterClient.CreateDocumentOnDriver(document);
            if (result.ResultStatus != CaseManagement.Service.ResultStatus.Success)
            {
                return StatusCode(500, result.ErrorDetail);
            }

            return Ok();
        }
    }
}
