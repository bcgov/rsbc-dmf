using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentStorageClient = Pssg.DocumentStorageAdapter.DocumentStorageAdapter.DocumentStorageAdapterClient;

namespace Rsbc.Dmf.IcbcAdapter.Controllers
{
    /// <summary>Identifies a file to download from document storage.</summary>
    public class DownloadRequestBody
    {
        /// <summary>
        /// Storage path of the file.
        /// </summary>
        [Required]
        public string FileUrl { get; init; }
    }

    public class SetDmerAiStatusRequestBody
    {
        /// <summary>
        /// The DMER ID.
        /// </summary>
        [Required]
        public string DmerId { get; init; }
        /// <summary>
        /// The new status to set.
        /// </summary>
        [Required]
        public int NewStatus { get; init; }
    }

    public enum DmerAIProccessedStatus
    {
        NA = 100000000,
        InProgress = 100000001,
        Proccessed = 100000002,
        Error = 100000003
    }

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DmersController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DmersController> _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;


        public DmersController(IConfiguration configuration, ILogger<DmersController> logger, IServiceScopeFactory serviceScopeFactory)
        {
           _configuration = configuration;
           _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }


      
    /// <summary>
    /// GET: /dmers?pageSize=50&amp;cursor=0
    /// </summary>
    /// <returns>DMER items, the requested page_size, and a nextLink for full pages.</returns>
    /// <param name="pageSize">Maximum number of records to return</param>
    /// <param name="cursor">Zero-based offset of the first record in the page</param>
    [HttpGet("")]
    [Produces("application/json")]
    public ActionResult GetDmerBatch(
        [FromQuery, Range(1, 500)] int pageSize = 50,
        [FromQuery, Range(0, int.MaxValue)] int cursor = 0)
        {

            using var scope = _serviceScopeFactory.CreateScope();
            var documentManagerClient = scope.ServiceProvider.GetRequiredService<DocumentManager.DocumentManagerClient>();
            var dmerRecordsRequest = new GetDmerRecordsPaginatedRequest {PageSize = pageSize, Cursor = cursor }; 
            var dmerRecordsResults = documentManagerClient.GetDmerRecordsPaginated(dmerRecordsRequest);
            
            if (dmerRecordsResults.ResultStatus == CaseManagement.Service.ResultStatus.Fail)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, dmerRecordsResults);
            }
            var results = dmerRecordsResults.Items;
            var nextLink = results.Count == pageSize && cursor <= int.MaxValue - pageSize
                ? Url.Action(nameof(GetDmerBatch), new { pageSize, cursor = cursor + pageSize })
                : null;

            return Json(new DmerBatchResponse(results, pageSize, nextLink));
        }

  /// <summary>Downloads a file using the existing document storage adapter client.</summary>
        [HttpGet("{documentRecordId}/files")]
        [SwaggerOperation(Summary = "Download a document by dmerdocumentid", Description = "Returns the stored file as a download.")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
        public async Task<ActionResult> Download([FromRoute] Guid documentRecordId, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var storageClient = scope.ServiceProvider.GetRequiredService<DocumentStorageClient>();
            var caseManagerClient = scope.ServiceProvider.GetRequiredService<CaseManager.CaseManagerClient>();
            Pssg.DocumentStorageAdapter.DownloadFileReply result;
            string fileUrl;

            // get file path from URL
            try
            {
                using var documentCall = caseManagerClient.GetLegacyDocumentAsync(
                    new LegacyDocumentRequest { DocumentId = documentRecordId.ToString() },
                    cancellationToken: cancellationToken);
                var documentReply = await documentCall.ResponseAsync;

                if (documentReply.ResultStatus != CaseManagement.Service.ResultStatus.Success)
                {
                    _logger.LogError("CMS adapter could not get document {DocumentRecordId}.", documentRecordId);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError,
                        title: "Unable to get the document record.");
                }

                fileUrl = documentReply.Document?.DocumentUrl;
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return Problem(statusCode: StatusCodes.Status404NotFound,
                        title: "The document record has no file path.");
                }
            }
            catch (RpcException exception) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(exception, "CMS adapter document request failed.");
                var statusCode = exception.StatusCode switch
                {
                    Grpc.Core.StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
                    Grpc.Core.StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
                    _ => StatusCodes.Status502BadGateway
                };
                return Problem(statusCode: statusCode, title: "Unable to get the document record.");
            }
            // enforce type = DEMR
            // enforce state = Uploaded

            // get file package from S3
            try
            {
                using var call = storageClient.DownloadFileAsync(
                    new Pssg.DocumentStorageAdapter.DownloadFileRequest { ServerRelativeUrl = fileUrl },
                    cancellationToken: cancellationToken);
                result = await call.ResponseAsync;
            }
            catch (RpcException exception) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(exception, "Document storage adapter download request failed.");
                var statusCode = exception.StatusCode switch
                {
                    Grpc.Core.StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
                    Grpc.Core.StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
                    _ => StatusCodes.Status502BadGateway
                };
                return Problem(statusCode: statusCode, title: "Unable to download the document from storage.");
            }

            if (result.ResultStatus != Pssg.DocumentStorageAdapter.ResultStatus.Success)
            {
                _logger.LogError("Document storage adapter could not download the document.");
                return Problem(statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to download the document from storage.");
            }

            var fileName = Path.GetFileName(fileUrl);
            if (!new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(result.Data.ToByteArray(), contentType, fileName);
        }

        /// <summary>
        /// POST: /Icbc/SetDmerAIProccessedStatus
        /// </summary>
        /// <param name="SetDmerAiStatusRequestBody">The processed status to be set for DMER records</param>
        /// <returns></returns>
        [HttpPost("SetDmerAIProccessedStatus")]
        public ActionResult SetDmerAIProccessedStatus([FromBody] SetDmerAiStatusRequestBody request)
        {
            if (System.Enum.IsDefined(typeof(DmerAIProccessedStatus), request.NewStatus))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Bad Data: Invalid Ai Proccessed Status code.");
            }
            using var scope = _serviceScopeFactory.CreateScope();
            var documentManagerClient = scope.ServiceProvider.GetRequiredService<DocumentManager.DocumentManagerClient>();
            var setDmerAIProccessedStatusRequest = new SetDmerAIProccessedStatusRequest { DmerAiStatus = request.NewStatus, DocumenturlId = request.DmerId };
            var setDmerAIProccessedStatusResult = documentManagerClient.SetDmerAIProccessedStatus(setDmerAIProccessedStatusRequest);

            if(setDmerAIProccessedStatusResult.ResultStatus == CaseManagement.Service.ResultStatus.Fail)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, setDmerAIProccessedStatusResult.ErrorMessage);
            }
            return Ok();
        }

    }
}
