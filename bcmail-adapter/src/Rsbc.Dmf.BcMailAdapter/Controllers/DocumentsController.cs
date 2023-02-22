using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Rsbc.Interfaces;
using Rsbc.Interfaces.CdgsModels;
using System.IO;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.Collections.Generic;
using Org.BouncyCastle.Utilities.Zlib;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using System.Text;
using MariGold.OpenXHTML;
using Rsbc.Dmf.BcMailAdapter.ViewModels;
using DocumentFormat.OpenXml.Wordprocessing;

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
            try
            {
                string fileName;
                LetterGenerationRequest letterGenerationRequest;
                if (bcmail?.Attachments != null && bcmail.Attachments.Count > 0)
                {

                    List<byte[]> srcPdfs = new List<byte[]>();

                    foreach (var attachment in bcmail.Attachments)
                    {

                        if (attachment.ContentType == "html")
                        {
                           var docx = CreateDocumentUtils.CreateDocument(attachment.Body, attachment.Header, attachment.Footer);

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
                                    ReportName = attachment.FileName ?? string.Empty
                                },
                                Template = new Template()

                                {
                                    //Content = attachment.Body ?? string.Empty,
                                   // Content = new String(Encoding.UTF8.GetString(docx)),
                                    Content = Convert.ToBase64String(docx),
                                    EncodingType = "base64",
                                    FileType = attachment.ContentType ?? string.Empty
                                 
                                }
                            };
                            var responsestream = await _cdgsClient.PreviewBcMailDocument(letterGenerationRequest);
                            srcPdfs.Add(responsestream.ReadAllBytes());
                        }
                        // Checks wether it is PDF file
                        else
                        {
                            byte[] byteArray = Convert.FromBase64String(attachment.Body);
                            srcPdfs.Add(byteArray);
                        }
                    };

                    fileName = bcmail.Attachments[0].FileName;

                    // Merge into one PDF 
                    byte[] mergedFiles = this.CombinePDFs(srcPdfs);

                   return File(mergedFiles, "application/pdf",fileDownloadName: bcmail.Attachments[0].FileName);

                    string content = "application/octet-stream";
                    string body = mergedFiles.Length > 0 ? Convert.ToBase64String(mergedFiles) : String.Empty;
                    

                    var res = new PdfResponse()
                    {
                        FileName = fileName,
                        ContentType = content,
                        Body = body,
                    };

                    return new JsonResult(res);

                }
            }
            catch(Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

            return new JsonResult(new PdfResponse());

        }

        /// <summary>
        /// Combine PDF's
        /// </summary>
        /// <param name="srcPDFs"></param>
        /// <returns></returns>
        public byte[] CombinePDFs(List<byte[]> srcPDFs)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var resultPDF = new PdfDocument())
                {
                    foreach (var pdf in srcPDFs)
                    {
                        using (var src = new MemoryStream(pdf))
                        {
                            using (var srcPDF = PdfReader.Open(src, PdfDocumentOpenMode.Import))
                            {
                                for (var i = 0; i < srcPDF.PageCount; i++)
                                {
                                    resultPDF.AddPage(srcPDF.Pages[i]);
                                }
                            }
                        }
                    }
                var resposeStream = new MemoryStream();
                resultPDF.Save(resposeStream);
                return resposeStream.ReadAllBytes();
                
                }
            
        }

      

    }
}
