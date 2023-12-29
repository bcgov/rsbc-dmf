using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.ViewModels;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : Controller
    {
        private readonly ILogger<DriversController> _logger;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;

        public DriversController(CaseManager.CaseManagerClient cmsAdapterClient, ILoggerFactory loggerFactory)
        {
            _cmsAdapterClient = cmsAdapterClient;
            _logger = loggerFactory.CreateLogger<DriversController>();
        }

        /// <summary>
        /// Get documents for a given driver
        /// </summary>
        /// <param name="licenseNumber">The drivers licence</param>
        /// <returns></returns>
        [HttpGet("{driverId}/Documents")]
        [ProducesResponseType(typeof(CaseDocuments), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetDocuments")]
        public ActionResult GetDocuments([FromRoute] string driverId)
        {
            var driverIdRequest = new DriverIdRequest() { Id = driverId };
            var reply = _cmsAdapterClient.GetDriverDocumentsById(driverIdRequest);
            if (reply.ResultStatus == ResultStatus.Success)
            {
                // get the comments
                var result = new CaseDocuments();

                foreach (var item in reply.Items)
                {
                    if (item.SubmittalStatus != "Uploaded")
                    {
                        //// todo - get the driver details from ICBC, get the MedicalIssueDate from Dynamics
                        //ViewModels.Driver driver = new ViewModels.Driver()
                        //{
                        //    Id = Guid.Parse(driverId),
                        //    Flag51 = false,
                        //    LoadedFromICBC = false,
                        //    MedicalIssueDate = DateTimeOffset.Now
                        //};

                        bool isBcMailSent = false;
                        if (item.DocumentType != null && item.DocumentType == "Letter Out BCMail" && item.ImportDate != null)
                        {
                            isBcMailSent = true;
                        }

                        //string caseId = string.Empty;
                        //if (item.CaseId != null && item.CaseId != "none")
                        //{
                        //    caseId = item.CaseId;
                        //}

                        TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                        DateTimeOffset importDate = DateTimeOffset.Now;
                        try
                        {
                            if (item.ImportDate != null)
                            {
                                importDate = item.ImportDate.ToDateTimeOffset();
                                if (importDate.Offset == TimeSpan.Zero)
                                {
                                    importDate = TimeZoneInfo.ConvertTimeFromUtc(importDate.DateTime, pacificZone);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation(ex, "Error parsing import date");
                            importDate = DateTimeOffset.Now;
                        }

                        DateTimeOffset faxReceivedDate = DateTimeOffset.Now;
                        try
                        {
                            if (item.FaxReceivedDate != null)
                            {
                                faxReceivedDate = item.FaxReceivedDate.ToDateTimeOffset();

                                if (faxReceivedDate < new DateTimeOffset(1970, 2, 1, 0, 0, 0, TimeSpan.Zero))
                                {
                                    faxReceivedDate = DateTimeOffset.Now;
                                }

                                if (faxReceivedDate.Offset == TimeSpan.Zero)
                                {
                                    faxReceivedDate = TimeZoneInfo.ConvertTimeFromUtc(faxReceivedDate.DateTime, pacificZone);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation(ex, "Error parsing faxReceivedDate date");
                        }

                        var newDocument = new Document
                        {
                            //CaseId = caseId,
                            //Driver = driver,
                            ImportDate = importDate,
                            DocumentId = item.DocumentId,
                            DocumentType = item.DocumentType,
                            DocumentTypeCode = item.DocumentTypeCode,
                            BusinessArea = item.BusinessArea,
                            SequenceNumber = item.SequenceNumber,
                            FaxReceivedDate = faxReceivedDate,
                            UserId = item.UserId,
                            BcMailSent = isBcMailSent,
                            CreateDate = item.CreateDate.ToDateTimeOffset(),
                            DueDate = item.DueDate?.ToDateTimeOffset(),
                            Description = item.Description,
                            DocumentUrl = item.DocumentUrl,
                            SubmittalStatus = item.SubmittalStatus
                        };

                        // TODO use switch statement
                        if (newDocument.SubmittalStatus == "Received")
                            result.CaseSubmissions.Add(newDocument);
                        if (newDocument.SubmittalStatus == "Open-Required")
                            result.SubmissionRequirements.Add(newDocument);
                        if (newDocument.SubmittalStatus == "Sent")
                            result.LettersToDriver.Add(newDocument);
                    }
                }

                return Json(result);
            }
            else
            {
                return StatusCode(500, reply.ErrorDetail);
            }
        }
    }
}