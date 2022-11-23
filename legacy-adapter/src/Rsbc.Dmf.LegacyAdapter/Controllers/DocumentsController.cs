using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;



namespace Rsbc.Dmf.LegacyAdapter.Controllers
{
    /// <summary>
    /// Controller providing data related to a Driver
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")] 
    public class DocumentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentsController> _logger;


        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;

        public DocumentsController(ILogger<DocumentsController> logger, IConfiguration configuration, CaseManager.CaseManagerClient cmsAdapterClient, DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient)
        {
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _logger = logger;
        }



        [HttpGet("{documentId}/mimetype")]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult GetDocumentMimeType([FromRoute] string documentId)

        {
            var reply = _cmsAdapterClient.GetLegacyDocument(new LegacyDocumentRequest() { DocumentId = documentId });

            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                string filename = Path.GetFileName(reply.Document.DocumentUrl);
                string mimetype = MimeUtils.GetMimeType(filename);
                return Json(mimetype);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Delete a document
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        // POST: /Documents/Delete/{DocumentId}
        [HttpPost("Delete/{documentId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult DeleteDocument([FromRoute] string documentId)

        {
            // call the back end
            var reply = _cmsAdapterClient.GetLegacyDocument(new LegacyDocumentRequest() { DocumentId = documentId });

            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // remove it from Dynamics.
                var cmsDeleteReply = _cmsAdapterClient.DeleteLegacyCaseDocument(new LegacyDocumentRequest() { DocumentId = documentId });

                if (cmsDeleteReply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                {                            
                    return Ok();
                }
                else
                {
                    Serilog.Log.Error($"Unexpected error - unable to remove document {cmsDeleteReply.ErrorDetail}");
                    return StatusCode(500, $"Unexpected error - unable to remove document {cmsDeleteReply.ErrorDetail}");
                }
            }
            else
            {
                Serilog.Log.Error($"Unexpected error - unable to get document meta-data - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }
        }


        /// <summary>
        /// Get Document Content
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        // GET: /Drivers/Exist
        [HttpGet("{documentId}")]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]        
        public ActionResult GetDocument([FromRoute] string documentId)        
        {            
            // call the back end
            var reply = _cmsAdapterClient.GetLegacyDocument( new LegacyDocumentRequest() { DocumentId = documentId } );

            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                if (! string.IsNullOrEmpty(reply.Document?.DocumentUrl))
                {
                    // fetch the file from S3
                    DownloadFileRequest downloadFileRequest = new DownloadFileRequest()
                    {
                        ServerRelativeUrl = reply.Document.DocumentUrl
                    };
                    var documentReply = _documentStorageAdapterClient.DownloadFile(downloadFileRequest);
                    if (documentReply.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
                    {
                        byte[] fileContents = documentReply.Data.ToByteArray();
                        string fileName = Path.GetFileName(reply.Document.DocumentUrl);
                        string mimetype = MimeUtils.GetMimeType(fileName);
                        Response.Headers.ContentDisposition = new Microsoft.Extensions.Primitives.StringValues($"inline; filename={fileName}");
                        Serilog.Log.Information($"Sending DocumentID {documentId} file {reply.Document.DocumentUrl} data size {fileContents?.Length}");
                        return new FileContentResult(fileContents, mimetype)
                        {
                            FileDownloadName = $"{fileName}"
                        };                        
                    }
                    else
                    {
                        Serilog.Log.Error($"Unexpected error - unable to fetch file from storage - {reply.ErrorDetail}");
                        return StatusCode(500, "Unexpected error - unable to fetch file from storage");
                    }                    
                }
                else
                {
                    return StatusCode(500, "Unexpected error - document URL is missing from document object");
                }                
            }
            else
            {
                Serilog.Log.Error($"Unexpected error - unable to get document meta-data - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }
        }
    }
}
