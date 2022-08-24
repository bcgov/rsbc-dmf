using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
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
            var reply = _cmsAdapterClient.GetLegacyDocument( new GetLegacyDocumentRequest() { DocumentId = documentId } );

            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {

                byte[] fileContents= Array.Empty<byte>();

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
                        fileContents = documentReply.Data.ToByteArray();
                    }
                    
                }
                string filename = Path.GetFileName(reply.Document.DocumentUrl);
                Response.Headers.ContentDisposition = new Microsoft.Extensions.Primitives.StringValues($"inline; filename={filename}");
                Serilog.Log.Information($"Sending DocumentID {documentId} file {reply.Document.DocumentUrl} data size {fileContents?.Length}");
                return new FileContentResult(fileContents, "application/pdf");
            }
            else
            {
                Serilog.Log.Error(reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }
        }
    }
}
