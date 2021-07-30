using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pssg.DocumentStorageAdapter.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class FileController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }


        // POST: /file/upload
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromBody] ViewModels.Upload upload)
        {
            var _S3 = new S3(_configuration);

            // convert the base64 string to a byte array.
            byte[] data = Convert.FromBase64String(upload.Body);

            Dictionary<string, string> metaData = new Dictionary<string, string>()
            {
                {S3.METADATA_KEY_ENTITY, upload.EntityName},
                {S3.METADATA_KEY_ENTITY_ID, $"{upload.EntityId}"},
                {S3.METADATA_KEY_TAG1, upload.Tag1},
                {S3.METADATA_KEY_TAG2, upload.Tag2},
                {S3.METADATA_KEY_TAG3, upload.Tag3}
            };

            var listTitle = _S3.GetDocumentListTitle(upload.EntityName);

            string fileUrl = await _S3.UploadFile(upload.FileName, listTitle, upload.EntityId.ToString(), data, upload.ContentType, metaData);
            ViewModels.Download result = new ViewModels.Download() {FileUrl = fileUrl};
            return new JsonResult(result);
        }

        /// <summary>
        /// POST: /file/download
        /// To test - use curl -v -d "{""fileUrl"":""test""}" -H "Content-Type: application/json" http://localhost:5000/file/download
        /// </summary>
        [HttpPost("download")]
        public async Task<ActionResult> Download([FromBody] ViewModels.Download download)
        {
            var _S3 = new S3(_configuration);
            var fileContents = await _S3.DownloadFile (download.FileUrl);
            if (fileContents == null)
            {
                return new BadRequestResult();
            }
            return new FileContentResult(fileContents, "application/octet-stream");
            
            
        }

    }
}
