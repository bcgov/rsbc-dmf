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
        /// DoesCaseExist
        /// </summary>
        /// <param name="licenseNumber"></param>
        /// <param name="surcode"></param>
        /// <returns>True if the case exists</returns>
        // GET: /Drivers/Exist
        [HttpGet("Exist")]
        public ActionResult DoesCaseExist(string licenseNumber, string surcode)
        {
            bool result = false;
            // get the case                                                
            return Json (result);
        }

        /// <summary>
        /// Get Comments for a case
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
                // fetch the file from S3
                DownloadFileRequest downloadFileRequest = new DownloadFileRequest()
                {
                     ServerRelativeUrl = reply.Document.DocumentUrl
                };
                var documentReply = _documentStorageAdapterClient.DownloadFile(downloadFileRequest);

                var fileContents = documentReply.Data.ToByteArray();
                return new FileContentResult(fileContents, "application/octet-stream");
            }
            else
            {
                Serilog.Log.Error(reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }


        }

    }
}
