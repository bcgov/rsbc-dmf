
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Cms;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.BcMailAdapter.ViewModels;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Interfaces;

using Serilog.Core;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;
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
        private readonly ILogger<DocumentsController> _logger;
        private readonly IConverter Converter;
        private readonly CaseManager.CaseManagerClient _caseManagerClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="cdgsClient"></param>
        public DocumentsController(ILogger<DocumentsController> logger, IConfiguration configuration, IConverter converter, CaseManager.CaseManagerClient caseManagerClient, DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient)
        {

            Configuration = configuration;
            _caseManagerClient = caseManagerClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _logger = logger;
            Converter = converter;
            Converter.Error += Converter_Error;
        }


        /// <summary>
        /// Mail a document
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpGet("test")]
        public ActionResult TestBcMail()
        {
            SftpUtils sfg = new SftpUtils(Configuration, null, null);
            sfg.CheckConnection();
            return Ok("Test complete.");

        }

        [AllowAnonymous]
        [HttpGet("{documentId}/GetPageCount")]
        public int GetPageCount([FromRoute] Guid documentId)
        {
            int result = -1;
            var documentResponse = _caseManagerClient.GetLegacyDocument(new LegacyDocumentRequest { DocumentId = documentId.ToString() });

            if (documentResponse.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // get the URL
                string serverRelativeUrl = documentResponse.Document.DocumentUrl;

                // fetch it

                var fileResult = _documentStorageAdapterClient.DownloadFile(new DownloadFileRequest()
                {
                    // ServerRelativeUrl = doc.PdfDocumentId
                    // Are we storing the document url in pdfDocument 
                    ServerRelativeUrl = serverRelativeUrl,
                });

                if (fileResult.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
                {
                    // get the size of the PDF.

                    var src = new MemoryStream(fileResult.Data.ToByteArray());

                    using (var srcPDF = PdfReader.Open(src, PdfDocumentOpenMode.Import))
                    {
                        result = srcPDF.PageCount;
                    }

                }
            }

            return result;
        }
        
        [HttpPost("{documentId}/Split")]
        public ActionResult SplitDocument([FromBody] SplitDetails splitDetails, [FromRoute] Guid documentId)
        {

            var documentResponse = _caseManagerClient.GetLegacyDocument(new LegacyDocumentRequest { DocumentId = documentId.ToString() });

            if (documentResponse.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // get the URL
                string serverRelativeUrl = documentResponse.Document.DocumentUrl;

                string originalEntity = serverRelativeUrl.Substring(0, serverRelativeUrl.IndexOf("/"));

               

                string filename = serverRelativeUrl.Substring(serverRelativeUrl.LastIndexOf('/') + 1);

                string originalId = serverRelativeUrl.Substring(originalEntity.Length + 1 , serverRelativeUrl.Length - originalEntity.Length - filename.Length -2);

                string newExtension = Path.GetExtension(filename);

                string newFilename;

                if (newExtension != null)
                {
                    newFilename = filename.Substring(0, filename.Length - newExtension.Length);
                }
                else
                {
                    newFilename = filename;
                }

                newFilename += "-2";

                if (newExtension != null)
                {
                    newFilename += newExtension;
                }

                // fetch it

                var fileResult = _documentStorageAdapterClient.DownloadFile(new DownloadFileRequest()
                {
                    // ServerRelativeUrl = doc.PdfDocumentId
                    // Are we storing the document url in pdfDocument 
                    ServerRelativeUrl = serverRelativeUrl,
                });

                if (fileResult.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
                {
                    // split the PDF.

                    var keepPdf = new PdfSharpCore.Pdf.PdfDocument(); // the parts of the PDF that we will keep
                    var newPdf = new PdfSharpCore.Pdf.PdfDocument(); // the new document.

                    var src = new MemoryStream(fileResult.Data.ToByteArray());

                    int newPageCount = 0;

                    using (var srcPDF = PdfReader.Open(src, PdfDocumentOpenMode.Import))
                    {
                        for (int i = 0; i < srcPDF.PageCount; i++)
                        {
                            // get this page.
                            var pageData = srcPDF.Pages[i];

                            int pageNumber = i + 1;

                            if (splitDetails.PagesToRemove.Contains(pageNumber))
                            {
                                newPdf.Pages.Add(pageData);
                                newPageCount++;
                            }
                            else
                            {
                                keepPdf.Pages.Add(pageData);
                            }
                        }

                        // save the new document.  only touch the documents if a change has been made.

                        if (newPageCount > 0)
                        {
                            using (var newStream = new MemoryStream())
                            {
                                newPdf.Save(newStream, false);

                                // add the page data
                                var newFileRequest = new UploadFileRequest
                                {
                                    ContentType = "application/pdf",
                                    Data = ByteString.CopyFrom(newStream.ToArray()),
                                    EntityName = "dfp_driver",
                                    FileName = newFilename,
                                    FolderName = documentResponse.Document.Driver.Id
                                };

                                var newFileResult = _documentStorageAdapterClient.UploadFile(newFileRequest);

                                // now create a document for this.

                                if (newFileResult.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
                                {
                                    var newLegacyDocument = new LegacyDocument
                                    {
                                        BatchId = documentResponse.Document.BatchId,
                                        DocumentUrl = newFileResult.FileName,
                                        Driver = documentResponse.Document.Driver,
                                        DocumentType = "Unclassified",
                                        FaxReceivedDate = documentResponse.Document.FaxReceivedDate,
                                        ImportDate = documentResponse.Document.ImportDate, 
                                        Origin = documentResponse.Document.Origin,
                                        ValidationMethod = documentResponse.Document.ValidationMethod,
                                        ValidationPrevious = documentResponse.Document.ValidationPrevious,
                                        BusinessArea = documentResponse.Document.BusinessArea,
                                        SubmittalStatus = "Uploaded",
                                        Queue = documentResponse.Document.Queue,
                                        Priority = documentResponse.Document.Priority, 
                                        Owner = "Team - Intake"
                                    };

                                    DateTimeOffset importDate = documentResponse.Document.ImportDate.ToDateTimeOffset();
                                    DateTimeOffset faxReceivedDate = documentResponse.Document.FaxReceivedDate.ToDateTimeOffset();
                                    TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

                                    if (importDate.DateTime != DateTimeOffset.MinValue)
                                    {
                                        if (importDate.Offset == TimeSpan.Zero)
                                        {
                                            importDate = TimeZoneInfo.ConvertTimeToUtc(importDate.DateTime, pacificZone);
                                        }
                                        newLegacyDocument.ImportDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(importDate);
                                    }
                                    else
                                    {
                                        newLegacyDocument.ImportDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue);
                                    }


                                    if (faxReceivedDate.DateTime != DateTimeOffset.MinValue)
                                    {
                                        if (faxReceivedDate.Offset == TimeSpan.Zero)
                                        {
                                            faxReceivedDate = TimeZoneInfo.ConvertTimeToUtc(faxReceivedDate.DateTime, pacificZone);
                                        }
                                        newLegacyDocument.FaxReceivedDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(faxReceivedDate);
                                    }
                                    else
                                    {
                                        newLegacyDocument.FaxReceivedDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue);
                                    }

                                    _caseManagerClient.CreateDocumentOnDriver(newLegacyDocument);
                                }

                            }

                            // save the keep document.

                            using (var keepStream = new MemoryStream())
                            {
                                keepPdf.Save(keepStream, false);

                            // update the page data
                            var keepFileRequest = new UploadFileRequest
                            {
                                ContentType = "application/pdf",
                                Data = ByteString.CopyFrom(keepStream.ToArray()),
                                EntityName = originalEntity,
                                FileName = filename,
                                FolderName = originalId
                            };

                            var keepSaveResult = _documentStorageAdapterClient.UploadFile(keepFileRequest);

                            }
                        }

                        

                        
                    }
                }
            }

            return Ok();

        }

        /// <summary>
        /// Merge documents
        /// </summary>
        /// <param name="documentsToMerge"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpPost("{documentId}/Merge")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult MergeDocuments([FromBody] List<Guid> documentsToMerge, [FromRoute] Guid documentId)
        {
            List<Guid> documentIds = new List<Guid>();
            documentIds.Add(documentId);

            foreach (var id in documentsToMerge)
            {
                documentIds.Add(id);
            }

            var newPdf = new PdfSharpCore.Pdf.PdfDocument(); // the new document.

            foreach (var id in documentIds)
            {
                var mergeDocumentResponse = _caseManagerClient.GetLegacyDocument(new LegacyDocumentRequest { DocumentId = id.ToString() });

                if (mergeDocumentResponse.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                {                    
                    // fetch it
                    var fileResult = _documentStorageAdapterClient.DownloadFile(new DownloadFileRequest()
                    {
                        // ServerRelativeUrl = doc.PdfDocumentId
                        // Are we storing the document url in pdfDocument 
                        ServerRelativeUrl = mergeDocumentResponse.Document.DocumentUrl,
                    });

                    var src = new MemoryStream(fileResult.Data.ToByteArray());

                    int newPageCount = 0;

                    using (var srcPDF = PdfReader.Open(src, PdfDocumentOpenMode.Import))
                    {
                        for (int i = 0; i < srcPDF.PageCount; i++)
                        {
                            // get this page.
                            var pageData = srcPDF.Pages[i];

                            newPdf.Pages.Add(pageData);
                        }
                    }
                }
            }

            // save the document
            var documentResponse = _caseManagerClient.GetLegacyDocument(new LegacyDocumentRequest { DocumentId = documentId.ToString() });
            // get the details for the original
            string serverRelativeUrl = documentResponse.Document.DocumentUrl;
            string originalEntity = serverRelativeUrl.Substring(0, serverRelativeUrl.IndexOf("/"));

            int firstSlashPos = serverRelativeUrl.IndexOf('/') + 1;
            int lastSlashPos = serverRelativeUrl.LastIndexOf('/') + 1;

            string folderName = serverRelativeUrl.Substring(firstSlashPos, lastSlashPos - firstSlashPos - 1);
            string filename = serverRelativeUrl.Substring(lastSlashPos);

            using (var newStream = new MemoryStream())
            {
                newPdf.Save(newStream, false);

                // add the page data
                var newFileRequest = new UploadFileRequest
                {
                    ContentType = "application/pdf",
                    Data = ByteString.CopyFrom(newStream.ToArray()),
                    EntityName = originalEntity,
                    FileName = filename,
                    FolderName = documentResponse.Document.Driver.Id
                };
                var newFileResult = _documentStorageAdapterClient.UploadFile(newFileRequest);
            }

            // now remove the other documents.
            foreach (var deleteId in documentsToMerge)
            {
                _caseManagerClient.DeleteLegacyCaseDocument(new LegacyDocumentRequest { DocumentId = deleteId.ToString() });
            }

            return Ok(newPdf);
        }

        // POST: /Documents/BcMail}
        [HttpPost("BcMail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult BcMailDocument([FromBody] ViewModels.BcMail bcmail)  
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

        private static void Converter_Error(object sender, WkHtmlToPdfDotNet.EventDefinitions.ErrorArgs e)
        {
            Serilog.Log.Error("[WKHTML ERROR] {0}", e.Message);
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

            Serilog.Log.Information(JsonSerializer.Serialize(bcmail, options));
            /*
            try
            {
            */
            string fileName;
            //CdgsRequest cdgsRequest;
            if (bcmail?.Attachments != null && bcmail.Attachments.Count > 0)
            {

                List<byte[]> srcPdfs = new List<byte[]>();

                foreach (var attachment in bcmail.Attachments)
                {

                    if (attachment?.ContentType == "html")
                    {

                        string decodedbody = ParseByteArrayToString(attachment.Body);


                        if (!String.IsNullOrEmpty(Configuration["VALIDATE_MARK_TAGS"]))

                        {
                            if (decodedbody.Contains("<mark>"))
                            {
                                //generate pdf with error message content
                                _logger.LogError("Manual Entry Fields which should be replaced have not all been removed from this issuance. Please review the content of all attachments before issuing to BCMail.");
                                return BadRequest("Manual Entry Fields which should be replaced have not all been removed from this issuance. Please review the content of all attachments before issuing to BCMail.");

                            }
                        }

                        //string decodedHeader = ParseByteArrayToString(attachment.Header);
                        //string decodedFooter = ParseByteArrayToString(attachment.Footer);

                        string tempPrefix = Guid.NewGuid().ToString();
                        string headerFilename = System.IO.Path.GetTempPath() + tempPrefix + "-header.html";
                        string pdfFileName = System.IO.Path.GetTempPath() + tempPrefix + ".pdf";
                        string footerFilename = System.IO.Path.GetTempPath() + tempPrefix + "-footer.html";
                        string stylesheetFileName = System.IO.Path.GetTempPath() + tempPrefix + "-stylesheet.css";

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
                                    margins.Unit = WkHtmlToPdfDotNet.Unit.Inches;
                                    break;
                                case "cm":
                                    margins.Unit = WkHtmlToPdfDotNet.Unit.Centimeters;
                                    break;
                                case "mm":
                                    margins.Unit = WkHtmlToPdfDotNet.Unit.Millimeters;
                                    break;
                                default:
                                    margins.Unit = WkHtmlToPdfDotNet.Unit.Inches;
                                    break;
                            }

                        }
                        else
                        {
                            margins = new MarginSettings() { Top = 3, Bottom = 3, Left = 0.5, Right = 0.5, Unit = WkHtmlToPdfDotNet.Unit.Inches };
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
                            LoadSettings = { BlockLocalFileAccess = false ,  LoadErrorHandling = ContentErrorHandling.Abort },
                            PagesCount = true,
                            HtmlContent = decodedbody,
                            WebSettings = { DefaultEncoding = "utf-8",},
                        }

                        }
                        };

                        // Set Stylesheets

                        if (attachment.Css != null && attachment.Css.Length > 0)
                        {
                            string decodedStyle = ParseByteArrayToString(attachment.Css);
                            System.IO.File.WriteAllText(stylesheetFileName, decodedStyle, Encoding.UTF8);
                            var stylesettings = new WebSettings() { UserStyleSheet = $"file:///{stylesheetFileName}" };
                            Serilog.Log.Logger.Information(stylesettings.UserStyleSheet);
                            doc.Objects[0].WebSettings.UserStyleSheet = stylesettings.UserStyleSheet;
                        }

                        if (attachment.Header != null && attachment.Header.Length > 0)
                        {
                            string decodedHeader = ParseByteArrayToString(attachment.Header);
                            decodedHeader = "<!doctype html>\n<html><body><header>\n" + decodedHeader + "\n</header></body></html>";


                            System.IO.File.WriteAllText(headerFilename, decodedHeader, Encoding.UTF8);
                            var headerSettings = new HeaderSettings() { HtmlUrl = $"file:///{headerFilename}" };
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

                            System.IO.File.WriteAllText(footerFilename, decodedFooter, Encoding.UTF8);
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
                        else
                        {
                            _logger.LogError("Rendered PDF is empty");
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
        private byte[] CombinePDFs(List<byte[]> srcPDFs)
        {
            byte[] result;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var resultPDF = new PdfSharpCore.Pdf.PdfDocument())
            {
                foreach (var pdf in srcPDFs)
                {
                    using (var src = new MemoryStream(pdf))
                    {
                        try
                        {
                            using (var srcPDF = PdfReader.Open(src, PdfDocumentOpenMode.Import))
                            {
                                for (var i = 0; i < srcPDF.PageCount; i++)
                                {
                                    resultPDF.AddPage(srcPDF.Pages[i]);

                                }

                                // If pdf document has odd pages then add an empty page
                                if (srcPDF.PageCount % 2 != 0)
                                {
                                    resultPDF.AddPage(new PdfPage());
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Error reading from PDF stream");
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
