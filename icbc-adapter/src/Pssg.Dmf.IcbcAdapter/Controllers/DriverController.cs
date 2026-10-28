using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using CmsResultStatus = Rsbc.Dmf.CaseManagement.Service.ResultStatus;
using System;

namespace Rsbc.Dmf.IcbcAdapter.Controllers
{


    public class DriverLicenseNumber
    {
        public string driverLicenceNumber { get; set; }
    }

    public class DriverAndDocumentsResponse //for Agentic DMER processing
    {
        public string driver_id { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string licence_number { get; set; }
        public List<DriverDocument> documents { get; set; }
        public DmerDocumentCaseRecord activeCase { get; set; }
    }


    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DriverController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DmersController> _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;


        public DriverController(IConfiguration configuration, ILogger<DmersController> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }


        /// <summary>
        /// Gets a driver, their documents, and their first active case, if any.
        /// </summary>
        /// <returns>Driver metadata and documents. The active case is omitted when none exists.</returns>
        /// <param name="driver_licence_number">Driver licence number to look up.</param>
        [HttpPost("lookup")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DriverAndDocumentsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public ActionResult GetDriverMetaandDocuments([FromBody] DriverLicenseNumber requestBody)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var caseManagerClient = scope.ServiceProvider.GetRequiredService<CaseManager.CaseManagerClient>();
            var driverRequest = new DriverLicenseRequest { DriverLicenseNumber = requestBody.driverLicenceNumber };
            var driverDocumentsReply = caseManagerClient.GetDriverDocuments(driverRequest);
            

            if (driverDocumentsReply.ResultStatus != CmsResultStatus.Success)
            {
                _logger.LogError("CMS adapter driver documents lookup failed: {ErrorDetail}", driverDocumentsReply.ErrorDetail);
                return Problem(statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to get the driver's documents.");
            }

            var driverPersonReply = caseManagerClient.GetDriverPerson(driverRequest);
            if (driverPersonReply.ResultStatus != CmsResultStatus.Success)
            {
                _logger.LogError("CMS adapter driver lookup failed: {ErrorDetail}", driverPersonReply.ErrorDetail);
                return Problem(statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to get the driver.");
            }

            var driverPerson = driverPersonReply.Items.FirstOrDefault();
            if (driverPerson == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    title: "Driver not found.");
            }

            var driverActiveCaseReply = caseManagerClient.GetActiveCases(driverRequest);
            if (driverActiveCaseReply.ResultStatus != CmsResultStatus.Success)
            {
                _logger.LogError("CMS adapter active cases lookup failed: {ErrorDetail}", driverActiveCaseReply.ErrorDetail);
                return Problem(statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to get the driver's active cases.");
            }

            var driverActiveCase = driverActiveCaseReply.Items.FirstOrDefault();

            var documents = driverDocumentsReply.Items
                .Where(document => document.DocumentType == "DMER" && document.SubmittalStatus == "Uploaded")
                .Select(document => new DriverDocument
            {
                FileName = document.DocumentUrl,
                DocumentType = document.DocumentType,
                SubmittalStatus = document.SubmittalStatus,
                UploadedDate = document.CreateDate == null 
                    ? document.FaxReceivedDate.ToDateTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture)
                    : document.CreateDate.ToDateTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture) ,
                DocumentRecordId = document.DocumentId
            }).ToList();

            var driver_case = driverActiveCase == null ? null : new DmerDocumentCaseRecord
            {
                CaseId = driverActiveCase.CaseId,
                CaseTitle = driverActiveCase.Title,
                CaseType = driverActiveCase.CaseType,
                CasePriority = driverActiveCase.Priority,
                CaseOwner = driverActiveCase.AssigneeTitle,
                CaseStatus = driverActiveCase.Status
            };

            var response = new DriverAndDocumentsResponse
            {
                driver_id = driverPerson.Id,
                first_name = driverActiveCase != null ? driverActiveCase.FirstName : String.Empty,   // get driver cannot return first name
                middle_name = driverActiveCase != null ? driverActiveCase.Middlename: String.Empty,  // get driver cannot return middle name
                last_name = driverPerson.Surname,
                licence_number = driverPerson.DriverLicenseNumber,
                documents = documents,
                activeCase = driver_case
            };

            return Json(response);
        }
    }
}
