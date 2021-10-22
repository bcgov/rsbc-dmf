using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pssg.DocumentStorageAdapter.ViewModels;

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
            try
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

                string fileUrl = await _S3.UploadFile(upload.FileName, listTitle, $"{upload.EntityId}", data, upload.ContentType, metaData);
                ViewModels.Download result = new ViewModels.Download() { FileUrl = fileUrl };
                return new JsonResult(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during file download.");
                throw e;
            }
        }

        /// <summary>
        /// POST: /file/download
        /// To test - use curl -v -d "{""fileUrl"":""test""}" -H "Content-Type: application/json" http://localhost:5000/file/download
        /// </summary>
        [HttpPost("download")]
        public async Task<ActionResult> Download([FromBody] ViewModels.Download download)
        {
            try
            {
                var _S3 = new S3(_configuration);
                Dictionary<string, string> metaData = new Dictionary<string, string>();
                var fileContents = _S3.DownloadFile(download.FileUrl, ref metaData);
                if (fileContents == null)
                {
                    _logger.LogError($"Error during file download for file {download.FileUrl} - no file data.");
                    return new BadRequestResult();
                }
                //return new FileContentResult(fileContents, "application/octet-stream");

                string fileName = download.FileUrl;
                string contentType = "application/octet-stream";
                string body = fileContents.Length > 0 ? Convert.ToBase64String(fileContents) : String.Empty;
                string entityName =
                    metaData != null && metaData.ContainsKey(S3.METADATA_KEY_ENTITY) &&
                    metaData[S3.METADATA_KEY_ENTITY] != null
                        ? metaData[S3.METADATA_KEY_ENTITY]
                        : String.Empty;
                Guid entityId = metaData != null && metaData.ContainsKey(S3.METADATA_KEY_ENTITY_ID) &&
                                  metaData[S3.METADATA_KEY_ENTITY_ID] != null
                    ? Guid.Parse(metaData[S3.METADATA_KEY_ENTITY_ID])
                    : new Guid();
                string tag1 =
                    metaData != null && metaData.ContainsKey(S3.METADATA_KEY_TAG1) &&
                    metaData[S3.METADATA_KEY_TAG1] != null
                        ? metaData[S3.METADATA_KEY_TAG1]
                        : String.Empty;
                string tag2 =
                    metaData != null && metaData.ContainsKey(S3.METADATA_KEY_TAG2) &&
                    metaData[S3.METADATA_KEY_TAG2] != null
                        ? metaData[S3.METADATA_KEY_TAG2]
                        : String.Empty;
                string tag3 =
                    metaData != null && metaData.ContainsKey(S3.METADATA_KEY_TAG3) &&
                    metaData[S3.METADATA_KEY_TAG3] != null
                        ? metaData[S3.METADATA_KEY_TAG3]
                        : String.Empty;

                var result = new Upload()
                {
                    FileName = fileName,
                    ContentType = contentType,
                    Body = body,
                    EntityName = entityName,
                    EntityId = entityId,
                    Tag1 = tag1,
                    Tag2 = tag2,
                    Tag3 = tag3,
                };

                return new JsonResult(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error during file download for file {download.FileUrl}.");
                throw e;
            }
            

        }

    }
}
