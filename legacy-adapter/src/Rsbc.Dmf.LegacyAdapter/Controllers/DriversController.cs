using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Operators;
using Pssg.DocumentStorageAdapter;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.IcbcModels;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.LegacyAdapter.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Driver = Rsbc.Dmf.CaseManagement.Service.Driver;

namespace Rsbc.Dmf.LegacyAdapter.Controllers
{
    /// <summary>
    /// Controller providing data related to a Driver
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class DriversController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DriversController> _logger;


        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly IIcbcClient _icbcClient;
        private readonly IMemoryCache _cache;

        public DriversController(ILogger<DriversController> logger, IConfiguration configuration, CaseManager.CaseManagerClient cmsAdapterClient, DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient, IIcbcClient icbcClient,
            IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _logger = logger;
            _icbcClient = icbcClient;

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
            return Json(result);
        }

        /// <summary>
        /// Get Comments for a case
        /// </summary>
        /// <param name="licenseNumber"></param>
        /// <param name="filter">Optional numeric sequence number to filter results by.</param>
        /// <param name="sort">Optional Char, one of 'D' - commentDate, 'T' - commentTypeCode, 'U' - userId, 'C' - commentText</param>
        /// <returns></returns>
        // GET: /Drivers/<DL>/Comments
        [HttpGet("{licenseNumber}/Comments")]
        [ProducesResponseType(typeof(List<ViewModels.Comment>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult GetComments([FromRoute] string licenseNumber, [FromQuery] string filter, [FromQuery] char sort)

        {
            // call the back end

            var reply = _cmsAdapterClient.GetDriverComments(new DriverLicenseRequest() { DriverLicenseNumber = licenseNumber });

            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // get the comments
                List<ViewModels.Comment> result = new List<ViewModels.Comment>();

                foreach (var item in reply.Items)
                {
                    // todo - get the driver details from ICBC, get the MedicalIssueDate from Dynamics
                    ViewModels.Driver driver = new ViewModels.Driver()
                    {
                        LicenseNumber = licenseNumber,
                        Flag51 = false,
                        LastName = item.Driver.Surname,
                        LoadedFromICBC = false,
                        MedicalIssueDate = DateTimeOffset.Now
                    };

                    bool addItem = true;
                    Guid filterValue;
                    Guid caseIdGuid;
                    if (!string.IsNullOrEmpty(filter) && Guid.TryParse(filter, out filterValue) && Guid.TryParse(item.CaseId, out caseIdGuid))
                    {
                        addItem = filterValue == caseIdGuid;
                    }

                    if (addItem)
                    {
                        string caseId = string.Empty;
                        if (item.CaseId != null && item.CaseId != "none")
                        {
                            caseId = item.CaseId;
                        }
                        result.Add(new ViewModels.Comment
                        {
                            CaseId = caseId,
                            CommentDate = item.CommentDate.ToDateTimeOffset(),
                            CommentId = item.CommentId,
                            CommentText = item.CommentText,
                            CommentTypeCode = item.CommentTypeCode,
                            Driver = driver,
                            SequenceNumber = Math.Abs(item.SequenceNumber),
                            UserId = string.IsNullOrEmpty(item.SignatureName) ? item.UserId : item.SignatureName // 24-01-12 default to signature name, fallback to UserID if not present.
                        });
                    }
                }

                if (sort != null)
                {
                    switch (sort)
                    {

                        case 'T': // - commentTypeCode
                            result = result.OrderBy(x => x.CommentTypeCode).ToList();
                            break;
                        case 'U': // - userId
                            result = result.OrderBy(x => x.UserId).ToList();
                            break;
                        case 'C': // - commentText
                            result = result.OrderBy(x => x.CommentText).ToList();
                            break;

                        case 'D': // - commentDate
                        default:
                            result = result.OrderByDescending(x => x.CommentDate).ToList();
                            break;
                    }
                }
                else
                {
                    result = result.OrderByDescending(x => x.CommentDate).ToList();
                }
                return Json(result);
            }
            else
            {
                return StatusCode(500);
            }
            /*
            result.Add (new ViewModels.Comment() { CaseId = Guid.NewGuid().ToString(), CommentText = "SAMPLE TEXT", CommentTypeCode="W",  CommentDate = DateTime.Now, CommentId = Guid.NewGuid().ToString(),
                Driver = new ViewModels.Driver() { Flag51 = false, LastName = "LASTNAME", LicenseNumber = "01234567", LoadedFromICBC = false, MedicalIssueDate = DateTimeOffset.Now }, 
                SequenceNumber = 0, UserId = "TESTUSER" });
            */

        }

        /// <summary>
        /// Get Comments for a driver
        /// </summary>
        /// <param name="licenseNumber"></param>
        /// <param name="filter">Optional numeric sequence number to filter results by.</param>
        /// <param name="sort">Optional Char, one of 'D' - commentDate, 'T' - commentTypeCode, 'U' - userId, 'C' - commentText</param>
        /// <returns></returns>
        // GET: /Drivers/<DL>/Comments
        [HttpGet("{licenseNumber}/AllComments")]
        [ProducesResponseType(typeof(List<ViewModels.Comment>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult GetAllComments([FromRoute] string licenseNumber, [FromQuery] string filter, [FromQuery] char sort)

        {
            // call the back end

            var reply = _cmsAdapterClient.GetAllDriverComments(new DriverLicenseRequest() { DriverLicenseNumber = licenseNumber });

            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // get the comments
                List<ViewModels.Comment> result = new List<ViewModels.Comment>();

                foreach (var item in reply.Items)
                {
                    // todo - get the driver details from ICBC, get the MedicalIssueDate from Dynamics
                    ViewModels.Driver driver = new ViewModels.Driver()
                    {
                        LicenseNumber = licenseNumber,
                        Flag51 = false,
                        LastName = item.Driver.Surname,
                        LoadedFromICBC = false,
                        MedicalIssueDate = DateTimeOffset.Now
                    };

                    bool addItem = true;
                    Guid filterValue;
                    Guid caseIdGuid;
                    if (!string.IsNullOrEmpty(filter) && Guid.TryParse(filter, out filterValue) && Guid.TryParse(item.CaseId, out caseIdGuid))
                    {
                        addItem = filterValue == caseIdGuid;
                    }

                    if (addItem)
                    {
                        string caseId = string.Empty;
                        if (item.CaseId != null && item.CaseId != "none")
                        {
                            caseId = item.CaseId;
                        }
                        result.Add(new ViewModels.Comment
                        {
                            CaseId = item.CaseId,
                            CommentDate = item.CommentDate.ToDateTimeOffset(),
                            CommentId = item.CommentId,
                            CommentText = item.CommentText,
                            CommentTypeCode = item.CommentTypeCode,
                            Driver = driver,
                            SequenceNumber = Math.Abs(item.SequenceNumber),
                            UserId = string.IsNullOrEmpty(item.SignatureName) ? item.UserId : item.SignatureName
                        });
                    }
                }

                if (sort != null)
                {
                    switch (sort)
                    {
                        case 'D': // - commentDate
                            result = result.OrderByDescending(x => x.CommentDate).ToList();
                            break;
                        case 'T': // - commentTypeCode
                            result = result.OrderBy(x => x.CommentTypeCode).ToList();
                            break;
                        case 'U': // - userId
                            result = result.OrderBy(x => x.UserId).ToList();
                            break;
                        case 'C': // - commentText
                            result = result.OrderBy(x => x.CommentText).ToList();
                            break;
                    }
                }
                return Json(result);
            }
            else
            {
                return StatusCode(500);
            }
            /*
            result.Add (new ViewModels.Comment() { CaseId = Guid.NewGuid().ToString(), CommentText = "SAMPLE TEXT", CommentTypeCode="W",  CommentDate = DateTime.Now, CommentId = Guid.NewGuid().ToString(),
                Driver = new ViewModels.Driver() { Flag51 = false, LastName = "LASTNAME", LicenseNumber = "01234567", LoadedFromICBC = false, MedicalIssueDate = DateTimeOffset.Now }, 
                SequenceNumber = 0, UserId = "TESTUSER" });
            */

        }

        [HttpGet("test")]
        [AllowAnonymous]
        public ActionResult TestDoc()
        {
            var result = new ViewModels.Document();

            result.FileContents = Encoding.ASCII.GetBytes("THIS IS A TEST");

            return Json(result);
        }


        [HttpGet("{licenseNumber}/Cases")]
        [ProducesResponseType(typeof(List<ViewModels.Case>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult GetCases([FromRoute] string licenseNumber)
        {
            var reply = _cmsAdapterClient.Search(new SearchRequest { DriverLicenseNumber = licenseNumber });
            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                List<ViewModels.Case> result = new List<ViewModels.Case>();
                foreach (var item in reply.Items)
                {
                    result.Add(new ViewModels.Case() { CaseId = item.CaseId });
                }
                return Json(result);
            }
            else
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Add a comment to a case
        /// </summary>
        /// <param name="licenseNumber"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost("{licenseNumber}/Comments")]
        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult CreateCommentForDriver([FromRoute] string licenseNumber, [FromBody] ViewModels.Comment comment)
        {
            

            licenseNumber = _icbcClient.NormalizeDl(licenseNumber, _configuration);


            CLNT icbcDriver = null;
            if (!_cache.TryGetValue(licenseNumber, out icbcDriver))
            {
                // get the history from ICBC
                icbcDriver = _icbcClient.GetDriverHistory(licenseNumber);
                // Key not in cache, so get data.
                //cacheEntry = DateTime.Now;
                if (icbcDriver != null)
                {
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time, reset time if accessed.
                        .SetSlidingExpiration(TimeSpan.FromHours(6));

                    // Save data in cache.
                    _cache.Set(licenseNumber, icbcDriver, cacheEntryOptions);
                }

            }

            if (icbcDriver != null && !string.IsNullOrEmpty(icbcDriver.INAM?.SURN) && comment.Driver.LastName != icbcDriver.INAM?.SURN)
            {
                comment.Driver.LastName = icbcDriver.INAM?.SURN;
                // ensure Dynamics has the most recent data.
                _cmsAdapterClient.UpdateDriver(new CaseManagement.Service.Driver
                {
                    DriverLicenseNumber = licenseNumber,
                    BirthDate = Timestamp.FromDateTimeOffset(icbcDriver.BIDT ?? DateTime.Now),
                    GivenName = icbcDriver.INAM?.GIV1 ?? string.Empty,
                    Surname = icbcDriver.INAM?.SURN ?? string.Empty
                });
            }

            //Serilog.Log.Logger.Information (JsonConvert.SerializeObject(comment));
            // add the comment

            if (comment.CommentText.Length > 1900)
            {
                comment.CommentText = comment.CommentText.Substring(0, 1900);
                Serilog.Log.Error("Encountered comment longer than 1900 chars");
                DebugUtils.SaveDebug("DriversCreateCommentForDriver", licenseNumber + " " + JsonConvert.SerializeObject(comment));
            }

            var driver = new CaseManagement.Service.Driver()
            {
                DriverLicenseNumber = licenseNumber,
                Surname = string.Empty,
                Address = new CaseManagement.Service.Address { City = string.Empty, Line1 = string.Empty, Line2 = string.Empty, Postal = string.Empty },
                BirthDate = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                GivenName = string.Empty,
                Height = 0.0,
                Id = string.Empty,
                Middlename = string.Empty,
                Name = string.Empty,
                Seck = string.Empty,
                Sex = string.Empty,
                Weight = 0.0
            };

            if (comment.Driver != null)
            {
                driver.Surname = comment.Driver.LastName ?? string.Empty;
            }

            DateTimeOffset commentDate = comment.CommentDate ?? DateTimeOffset.Now.AddMinutes(-1);
            // Dynamics has a minimum value for a date.
            if (commentDate.Year < 1753)
            {
                commentDate = DateTimeOffset.Now.AddMinutes(-1);
            }
            try
            {

                string caseId = string.Empty;
                if (comment.CaseId != null && comment.CaseId != "none" && comment.CaseId != "null")
                {
                    caseId = comment.CaseId;
                }
                else // handle situations where the CaseID is not supplied.
                {
                    if (comment.SequenceNumber != null)
                    {
                        // fetch it from the sequence number.
                        caseId = _cmsAdapterClient.GetCaseId(comment.Driver.LicenseNumber, comment.Driver.LastName, (int)comment.SequenceNumber.Value);
                    }

                    if (caseId == null) // try just the DL and Surname.
                    {
                        caseId = _cmsAdapterClient.GetCaseId(comment.Driver.LicenseNumber, comment.Driver.LastName);
                    }

                }

                var payload = new LegacyComment()
                {
                    CaseId = caseId ?? string.Empty,
                    CommentText = comment.CommentText ?? string.Empty,
                    CommentTypeCode = comment.CommentTypeCode ?? string.Empty,
                    SequenceNumber = Math.Abs(comment.SequenceNumber ?? 1),
                    UserId = comment.UserId ?? string.Empty,
                    CommentDate = Timestamp.FromDateTimeOffset(commentDate),
                    Driver = driver,
                    CommentId = comment.CommentId ?? string.Empty,
                    Assignee = string.Empty
                };

                var result = _cmsAdapterClient.CreateLegacyCaseComment(payload);

                if (result.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                {
                    var actionName = nameof(CreateCommentForDriver);
                    var routeValues = new
                    {
                        driversLicence = licenseNumber
                    };

                    return CreatedAtAction(actionName, routeValues, comment);
                }
                else
                {
                    _logger.LogError($"Error in create comment - {result.ErrorDetail}");

                    DebugUtils.SaveDebug("DriversCreateCommentForDriver", licenseNumber + " " + JsonConvert.SerializeObject(comment));

                    return StatusCode(500, result.ErrorDetail);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error in create comment");

                DebugUtils.SaveDebug("DriversCreateCommentForDriver", licenseNumber + " " + JsonConvert.SerializeObject(comment));

                return StatusCode(500, "Error in create comment");
            }

        }

        /// <summary>
        /// Get documents for a given driver
        /// </summary>
        /// <param name="licenseNumber">The drivers licence</param>
        /// <returns></returns>
        [HttpGet("{licenseNumber}/Documents")]
        [ProducesResponseType(typeof(List<ViewModels.Document>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult GetDocuments([FromRoute] string licenseNumber)
        {
            // call the back end

            if (string.IsNullOrEmpty(licenseNumber))
            {
                Serilog.Log.Error("Request to Driver Get Documents with no DL");
                return StatusCode(400, "Bad Request");
            }
            else
            {
                var reply = _cmsAdapterClient.GetDriverDocuments(new DriverLicenseRequest() { DriverLicenseNumber = licenseNumber });

                if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                {
                    // get the comments
                    List<ViewModels.Document> result = new List<ViewModels.Document>();

                    foreach (var item in reply.Items)
                    {

                        if (item.SubmittalStatus != "Uploaded" && !string.IsNullOrEmpty(item.DocumentUrl))
                        {

                            // todo - get the driver details from ICBC, get the MedicalIssueDate from Dynamics
                            ViewModels.Driver driver = new ViewModels.Driver()
                            {
                                LicenseNumber = licenseNumber,
                                Flag51 = false,
                                LastName = "LASTNAME",
                                LoadedFromICBC = false,
                                MedicalIssueDate = DateTimeOffset.Now
                            };

                            bool isBcMailSent = false;

                            if (item.DocumentType != null && item.DocumentType == "Letter Out BCMail" && item.ImportDate != null)
                            {
                                isBcMailSent = true;
                            }

                            string caseId = string.Empty;
                            if (item.CaseId != null && item.CaseId != "none")
                            {
                                caseId = item.CaseId;
                            }

                            TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

                            DateTimeOffset faxReceivedDate = DateTimeOffset.Now;

                            try
                            {
                                if (item.ImportDate != null)
                                {
                                    faxReceivedDate = item.ImportDate.ToDateTimeOffset();

                                    if (faxReceivedDate.Offset == TimeSpan.Zero)
                                    {
                                        faxReceivedDate = TimeZoneInfo.ConvertTimeFromUtc(faxReceivedDate.DateTime, pacificZone);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Information(ex, "Error parsing import date");
                                faxReceivedDate = DateTimeOffset.Now;
                            }

                            DateTimeOffset importDate = DateTimeOffset.Now;

                            try
                            {
                                if (item.FaxReceivedDate != null)
                                {
                                    importDate = item.FaxReceivedDate.ToDateTimeOffset();

                                    if (importDate < new DateTimeOffset(1970, 2, 1, 0, 0, 0, TimeSpan.Zero))
                                    {
                                        importDate = DateTimeOffset.Now;
                                    }

                                    if (importDate.Offset == TimeSpan.Zero)
                                    {
                                        importDate = TimeZoneInfo.ConvertTimeFromUtc(importDate.DateTime, pacificZone);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Information(ex, "Error parsing faxReceivedDate date");
                            }



                            var newDocument = new ViewModels.Document
                            {
                                CaseId = caseId,
                                ImportDate = importDate,
                                DocumentId = item.DocumentId,
                                DocumentType = item.DocumentType,
                                DocumentTypeCode = item.DocumentTypeCode,
                                BusinessArea = item.BusinessArea,
                                Driver = driver,
                                SequenceNumber = item.SequenceNumber,
                                FaxReceivedDate = faxReceivedDate,
                                UserId = item.UserId,
                                BcMailSent = isBcMailSent
                            };

                            result.Add(newDocument);

                        }
                    }

                    // sort the result by date.

                    var sortedResult = result.OrderByDescending(x => x.ImportDate);

                    return Json(sortedResult);
                }
                else
                {
                    return StatusCode(500, reply.ErrorDetail);
                }
            }

        }



        public static string SanitizeKeyFilename(string data)
        {

            var invalidCharacters = Path.GetInvalidFileNameChars().ToList();
            invalidCharacters.Add(' ');
            invalidCharacters.Add('/');
            invalidCharacters.Add('\\');

            string result = new string(data
                .Where(x => !invalidCharacters.Contains(x))
                .ToArray());

            return result;
        }

        /// <summary>
        /// Add a document to a case
        /// </summary>
        /// <param name="licenseNumber">Driver Licence</param>
        /// <param name="document">The document to add</param>
        /// <returns></returns>
        [HttpPost("{licenseNumber}/Documents")]
        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]

        public ActionResult CreateDriverDocument([FromRoute] string licenseNumber, [FromBody] ViewModels.Document document)
        {
            // first get the driver.

            var driverRequest = new DriverLicenseRequest() { DriverLicenseNumber = licenseNumber };
            var driverReply = _cmsAdapterClient.GetDriver(driverRequest);

            string filename = SanitizeKeyFilename(document.DocumentType);

            if (driverReply.ResultStatus == CaseManagement.Service.ResultStatus.Success && driverReply.Items != null && driverReply.Items.Count > 0)
            {
                var driverId = driverReply.Items.FirstOrDefault()?.Id;


                var driver = new Driver()
                {
                    DriverLicenseNumber = licenseNumber
                };

                if (document.Driver != null)
                {
                    driver.Surname = document.Driver.LastName;
                }
                DateTimeOffset importDate = document.ImportDate ?? DateTimeOffset.Now;
                DateTimeOffset faxReceivedDate = document.FaxReceivedDate ?? DateTimeOffset.Now;



                TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

                if (importDate.Offset == TimeSpan.Zero)
                {
                    importDate = TimeZoneInfo.ConvertTimeToUtc(importDate.DateTime, pacificZone);
                }
                if (faxReceivedDate.Offset == TimeSpan.Zero)
                {
                    faxReceivedDate = TimeZoneInfo.ConvertTimeToUtc(faxReceivedDate.DateTime, pacificZone);
                }

                long sequenceNumber = document.SequenceNumber ?? 0;
                string caseId = string.Empty;
                if (document.CaseId != null && document.CaseId != "none")
                {
                    caseId = document.CaseId;
                }
                var newDocument = new LegacyDocument()
                {
                    CaseId = caseId,
                    SequenceNumber = sequenceNumber,
                    UserId = document.UserId,
                    FaxReceivedDate = Timestamp.FromDateTimeOffset(faxReceivedDate),
                    ImportDate = Timestamp.FromDateTimeOffset(importDate),
                    Driver = driver,
                    DocumentTypeCode = document.DocumentTypeCode,
                    DocumentType = document.DocumentType,
                    BusinessArea = document.BusinessArea,
                    Priority = string.Empty,
                    Owner = string.Empty,
                    BatchId = string.Empty,
                    ValidationMethod = string.Empty,
                    ValidationPrevious = string.Empty,
                    ImportId = string.Empty,
                    OriginatingNumber = string.Empty,
                    SubmittalStatus = "Sent",
                };

                string importDateString = importDate.ToString("yyyyMMddHHmmss");
                string fileKey = DocumentUtils.SanitizeKeyFilename($"D{importDateString}-{filename}");


                // add the document
                UploadFileRequest pdfData = new UploadFileRequest()
                {
                    ContentType = "application/pdf",
                    Data = ByteString.CopyFrom(document.FileContents),
                    EntityName = "dfp_driver",
                    FileName = $"{fileKey}.pdf",
                    FolderName = driverId,
                };

                var fileReply = _documentStorageAdapterClient.UploadFile(pdfData);

                if (fileReply.ResultStatus != Pssg.DocumentStorageAdapter.ResultStatus.Success)
                {
                    return StatusCode(500, fileReply.ErrorDetail);
                }

                newDocument.DocumentUrl = fileReply.FileName;

                var result = _cmsAdapterClient.CreateLegacyCaseDocument(newDocument);

                if (result.ResultStatus != CaseManagement.Service.ResultStatus.Success)
                {
                    return StatusCode(500, result.ErrorDetail);
                }

                /*

                UpdateCaseRequest updateCaseRequest = new UpdateCaseRequest()
                {
                    CaseId = document.CaseId,
                    IsCleanPass = false,
                    DataFileKey = string.Empty,
                    DataFileSize = 0,
                    PdfFileKey = fileReply.FileName,
                    PdfFileSize = document.FileContents.Length,

                };

                // may need to add flags here.

                var caseResult = _cmsAdapterClient.UpdateCase(updateCaseRequest);
                */

                var actionName = nameof(CreateDriverDocument);
                var routeValues = new
                {
                    driversLicence = licenseNumber
                };

                // reduce the data sent back.
                document.FileContents = null;

                return CreatedAtAction(actionName, routeValues, document);
            }

            else

            {
                return StatusCode(500, driverReply.ErrorDetail);
            }
        }
    }
}
