using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Rsbc.Interfaces;
using Rsbc.Interfaces.CdgsModels;
using System.IO;

namespace Rsbc.Dmf.BcMailAdapter.Controllers
{
    /// <summary>
    /// Controller providing data related to a Driver
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class DocumentsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<DocumentsController> Logger;
        private readonly ICdgsClient _cdgsClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="cdgsClient"></param>
        public DocumentsController(ILogger<DocumentsController> logger, IConfiguration configuration, ICdgsClient cdgsClient)
        {
            Configuration = configuration;
            Logger = logger;
            _cdgsClient = cdgsClient;

        }


        /// <summary>
        /// Mail a document
        /// </summary>
        /// <returns></returns>

        // POST: /Documents/BcMail}
        [HttpPost("BcMail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult BcMailDocument()  // add a model here for the payload

        {

            // this could put the HTML file and attachments in a particular location.

            return Ok("Success");
        }

        /// <summary>
        /// Bc Mail Document Preview
        /// </summary>
        /// <param name="bcmail"></param>
        /// <returns></returns>

        // POST: /Documents/BcMailPreview}
        [HttpPost("BcMailPreview")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> BcMailDocumentPreview([FromBody] ViewModels.BcMail bcmail)
        {
            string fileName;
            LetterGenerationRequest letterGenerationRequest;
            if (bcmail?.Attachments != null && bcmail.Attachments.Count > 0)
            {
                fileName = bcmail.Attachments[0].FileName;
                letterGenerationRequest = new LetterGenerationRequest
                {
                    Data = new Data
                    {
                        /*FirstName = "Test1",
                        LastName = "LAstNAme",
                        Title = "Hello"*/
                    },
                    /* Formatters = "",*/
                    Options = new Options
                    {
                        ConvertTo = "pdf",
                        Overwrite = true,
                        ReportName = bcmail.Attachments[0].FileName ?? string.Empty
                    },
                    Template = new Template()
                    {
                        Content = bcmail?.Attachments[0].Body ?? string.Empty,
                        EncodingType = "base64",
                        FileType = bcmail.Attachments[0].ContentType ?? string.Empty
                    }

                };
            }
            else
            {
                fileName = string.Empty;
                letterGenerationRequest = new LetterGenerationRequest();
            }

            

            var responseStream = await _cdgsClient.PreviewBcMailDocument(letterGenerationRequest);
            byte[] filebytes = responseStream.ReadAllBytes();
            //return File(bytes, "application/pdf",fileDownloadName: bcmail.Attachments[0].FileName);
            string contentType = "application/octet-stream";
            string body = filebytes.Length > 0 ? Convert.ToBase64String(filebytes) : String.Empty;
            


            var result = new PdfResponse()
            {
                FileName = fileName,
                ContentType = contentType,
                Body = body,
            };

            return new JsonResult(result);

        }

    }
}
