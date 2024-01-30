using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
using System.Net;
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
        private readonly IMapper _mapper;
        private readonly ILogger<DriverController> _logger;

        public DocumentController(CaseManager.CaseManagerClient cmsAdapterClient, DocumentStorageAdapterClient documentStorageAdapterClient, IUserService userService, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _userService = userService;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<DriverController>();
        }

        /// <summary>
        /// Get Document Content
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpGet("{documentId}")]
        [Authorize(Policy = Policy.Driver)]
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
                        //Serilog.Log.Information($"Sending DocumentID {documentId} file {reply.Document.DocumentUrl} data size {fileContents?.Length}");
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
    }
}
