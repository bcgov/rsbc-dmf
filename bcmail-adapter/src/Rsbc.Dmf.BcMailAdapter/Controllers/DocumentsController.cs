using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Rsbc.Interfaces;
using Rsbc.Interfaces.CdgsModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using JsonSerializer = System.Text.Json.JsonSerializer;
using PaperKind = WkHtmlToPdfDotNet.PaperKind;

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
        private readonly IConverter Converter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="cdgsClient"></param>
        public DocumentsController(ILogger<DocumentsController> logger, IConfiguration configuration, ICdgsClient cdgsClient, IConverter converter)
        {
            Configuration = configuration;
            Logger = logger;
            _cdgsClient = cdgsClient;
            Converter = converter; ;
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
        public ActionResult BcMailDocument([FromBody] ViewModels.BcMail bcmail)  // add a model here for the payload

        {

            // this could put the HTML file and attachments in a particular location.

            return BcMailDocumentPreview(bcmail).GetAwaiter().GetResult();
        }


        private string ParseByteArrayToString(byte[] data)
        {
            string result = string.Empty;
            if (data != null && data.Length > 0)
            {
                result = Encoding.UTF8.GetString(data);
            }
            return result;
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

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            Serilog.Log.Information (JsonSerializer.Serialize(bcmail, options));
            /*
            try
            {
            */
                string fileName;
                CdgsRequest cdgsRequest;
                if (bcmail?.Attachments != null && bcmail.Attachments.Count > 0)
                {

                    List<byte[]> srcPdfs = new List<byte[]>();

                    foreach (var attachment in bcmail.Attachments)
                    {

                        if (attachment?.ContentType == "html")
                        {
                          
                            string decodedbody = ParseByteArrayToString(attachment.Body);
                            //string decodedHeader = ParseByteArrayToString(attachment.Header);
                            //string decodedFooter = ParseByteArrayToString(attachment.Footer);

                        string tempPrefix = Guid.NewGuid().ToString();
                        string headerFilename = System.IO.Path.GetTempPath() + tempPrefix + "-header.html";
                        string pdfFileName = System.IO.Path.GetTempPath() + tempPrefix + ".pdf";
                        string footerFilename = System.IO.Path.GetTempPath() + tempPrefix + "-footer.html";

                        MarginSettings margins = new MarginSettings();

                        if (attachment.Top != null || attachment.Bottom != null || attachment.Left != null || attachment.Right != null)
                        {                            
                            if (attachment.Top != null)
                            {
                                margins.Top = attachment.Top;
                            }
                            if (attachment.Bottom != null)
                            {
                                margins.Bottom = attachment.Bottom;
                            }
                            if (attachment.Left != null)
                            {
                                margins.Left = attachment.Left;
                            }
                            if (attachment.Right != null)
                            {
                                margins.Right = attachment.Right;
                            }
                            switch (attachment.Unit)
                            {
                                case "inch":
                                    margins.Unit = Unit.Inches;
                                    break;
                                case "cm":
                                    margins.Unit = Unit.Centimeters;
                                    break;
                                case "mm":
                                    margins.Unit = Unit.Millimeters;
                                    break;
                                default:
                                    margins.Unit = Unit.Inches;
                                    break;
                            }
                            
                        }
                        else
                        {
                            margins = new MarginSettings() { Top = 3, Bottom = 3, Left = 0.5, Right = 0.5, Unit = Unit.Inches };                            
                        }

                        var doc = new HtmlToPdfDocument()
                        {
                            GlobalSettings = {
                            ColorMode = ColorMode.Color,
                            Orientation = Orientation.Portrait,
                            PaperSize = PaperKind.Letter,
                            DPI = 72,
                            Margins = margins
                        },
                            Objects = {
                        new ObjectSettings() {
                            LoadSettings = { BlockLocalFileAccess = false ,  LoadErrorHandling = ContentErrorHandling.Ignore },
                            PagesCount = true,
                            HtmlContent = decodedbody,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            
                            
                        }
                        }
                        };


                        if (attachment.Header != null && attachment.Header.Length > 0)
                        {
                            string decodedHeader = ParseByteArrayToString(attachment.Header);
                            decodedHeader = "<!doctype html>\n<html><body><header>\n" + decodedHeader + "\n</header></body></html>";


                            System.IO.File.WriteAllText(headerFilename, decodedHeader);
                            var headerSettings = new HeaderSettings() {   HtmlUrl = $"file:///{headerFilename}"  };
                            Serilog.Log.Logger.Information(headerSettings.HtmlUrl);
                            doc.Objects[0].HeaderSettings = headerSettings;
                            
                            //string decodedHeader = ParseByteArrayToString(attachment.Header);
                            //doc.Objects[0].HeaderSettings = new HeaderSettings() { Center = decodedHeader };
                        }

                        if (attachment.Footer != null && attachment.Footer.Length > 0)
                        {
                            //System.IO.File.WriteAllBytes(footerFilename, attachment.Footer);
                            string decodedFooter = ParseByteArrayToString(attachment.Footer);
                            decodedFooter = "<!doctype html>\n<html><body><footer>\n" + decodedFooter + "\n</footer></body></html>";
                            
                            System.IO.File.WriteAllText(footerFilename, decodedFooter);
                            var footerSettings = new FooterSettings() { HtmlUrl = $"file:///{footerFilename}" };
                            doc.Objects[0].FooterSettings = footerSettings;
                            
                            //string decodedFooter = ParseByteArrayToString(attachment.Footer);
                            //doc.Objects[0].FooterSettings = new FooterSettings() { Center = decodedFooter };
                        }

                        byte[] pdfData = Converter.Convert(doc);
                        System.IO.File.WriteAllBytes(pdfFileName, pdfData);

                        if (pdfData.Length > 0)
                        {
                            srcPdfs.Add(pdfData);
                        }
                        

                        if (attachment.Header != null && attachment.Header.Length > 0 && System.IO.File.Exists(headerFilename))
                        {
                            System.IO.File.Delete(headerFilename);
                        }

                        if (attachment.Footer != null && attachment.Footer.Length > 0 && System.IO.File.Exists(footerFilename))
                        {
                            System.IO.File.Delete(footerFilename);
                        }



                        }
                        // add a PDF file
                        else
                        {
                            srcPdfs.Add(attachment.Body);
                        }
                    };

                    fileName = bcmail.Attachments[0].FileName;

                    // Merge into one PDF 
                    byte[] mergedFiles = CombinePDFs(srcPdfs);

                   //return File(mergedFiles, "application/pdf",fileDownloadName: bcmail.Attachments[0].FileName);

                    string content = "application/octet-stream";
                    byte[] body = mergedFiles.Length > 0 ? mergedFiles : new byte[0]; 
                    

                    var res = new PdfResponse()
                    {
                        FileName = fileName,
                        ContentType = content,
                        Body = body,
                    };

                    return new JsonResult(res);

                }
/*
            }
            catch(Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
*/
            return new JsonResult(new PdfResponse());

        }

        /// <summary>
        /// Combine PDF's
        /// </summary>
        /// <param name="srcPDFs"></param>
        /// <returns></returns>
        public byte[] CombinePDFs(List<byte[]> srcPDFs)
        {
            byte[] result;
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
                using (var resposeStream = new MemoryStream())
                {
                    resultPDF.Save(resposeStream, false);
                    result = resposeStream.ToArray();
                }
                
                
                return result;
        }
            
        }

      

    }
}
