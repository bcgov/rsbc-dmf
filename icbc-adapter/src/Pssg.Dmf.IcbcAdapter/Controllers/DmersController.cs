using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.IdentityModel.Tokens;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.HttpSys;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Grpc.Core;
using Microsoft.AspNetCore.StaticFiles;
using Swashbuckle.AspNetCore.Annotations;
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

        /// <summary>Creates a DMER case for an existing DMER document and links the document to it.</summary>
        [HttpPost("{dmerId}/add_case")]
        [SwaggerOperation(Summary = "Create a case for an existing DMER document")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AddDmerCaseResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
        public async Task<ActionResult> AddCase([FromRoute] Guid dmerId, [FromBody] AddDmerCaseRequest request, CancellationToken cancellationToken)
        {
            if (!string.Equals(request.CaseType, "DMER", StringComparison.OrdinalIgnoreCase))
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "case_type must be DMER.");
            }
            if (!string.Equals(request.DocumentType, "DMER", StringComparison.OrdinalIgnoreCase))
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "document_type must be DMER.");
            }
            if (string.IsNullOrWhiteSpace(request.Driver?.DriverId))
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: "driver.driver_id is required.");
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var caseManagerClient = scope.ServiceProvider.GetRequiredService<CaseManager.CaseManagerClient>();
            CreateCaseForDmerDocumentReply reply;

            try
            {
                using var call = caseManagerClient.CreateCaseForDmerDocumentAsync(
                    new CreateCaseForDmerDocumentRequest
                    {
                        DocumentId = dmerId.ToString(),
                        DriverLicenseNumber = request.Driver.DriverId.Trim(),
                        Comments = request.Comments ?? string.Empty
                    },
                    cancellationToken: cancellationToken);
                reply = await call.ResponseAsync;
            }
            catch (RpcException exception) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(exception, "CMS adapter create case for DMER request failed.");
                var statusCode = exception.StatusCode switch
                {
                    Grpc.Core.StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
                    Grpc.Core.StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
                    _ => StatusCodes.Status502BadGateway
                };
                return Problem(statusCode: statusCode, title: "Unable to create the case.");
            }

            switch (reply.ResultStatus)
            {
                case CreateCaseForDmerDocumentReply.Types.CreateCaseForDmerDocumentStatus.Success:
                    return StatusCode(StatusCodes.Status201Created,
                        new AddDmerCaseResponse(reply.CaseId, dmerId.ToString(), "Created"));
                case CreateCaseForDmerDocumentReply.Types.CreateCaseForDmerDocumentStatus.DocumentNotFound:
                    return Problem(statusCode: StatusCodes.Status404NotFound, title: "The DMER document was not found.");
                case CreateCaseForDmerDocumentReply.Types.CreateCaseForDmerDocumentStatus.DriverNotFound:
                    return Problem(statusCode: StatusCodes.Status404NotFound, title: "The driver was not found.");
                    /// just go create driver is not found?
                case CreateCaseForDmerDocumentReply.Types.CreateCaseForDmerDocumentStatus.InvalidDocumentType:
                    return Problem(statusCode: StatusCodes.Status409Conflict, title: "The document is not a DMER.");
                case CreateCaseForDmerDocumentReply.Types.CreateCaseForDmerDocumentStatus.InvalidSubmittalStatus:
                    return Problem(statusCode: StatusCodes.Status409Conflict, title: "The document is not in Uploaded status.");
                case CreateCaseForDmerDocumentReply.Types.CreateCaseForDmerDocumentStatus.AlreadyLinked:
                    return Problem(statusCode: StatusCodes.Status409Conflict, title: "The document is already linked to a case.");
                case CreateCaseForDmerDocumentReply.Types.CreateCaseForDmerDocumentStatus.DriverMismatch:
                    return Problem(statusCode: StatusCodes.Status409Conflict, title: "The document belongs to a different driver.");
                case CreateCaseForDmerDocumentReply.Types.CreateCaseForDmerDocumentStatus.ActiveCaseExists:
                    return Problem(statusCode: StatusCodes.Status409Conflict, title: "The driver already has an active DMF case.",
                        detail: $"Active case: {reply.CaseId}");
                default:
                    _logger.LogError("CMS adapter could not create a case for DMER {DmerId}: {ErrorDetail}", dmerId, reply.ErrorDetail);
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Unable to create the case.");
            }
        }

    }






}
