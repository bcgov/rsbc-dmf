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

    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class DriversController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DriversController> _logger;


        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;

        public DriversController(ILogger<DriversController> logger, IConfiguration configuration, CaseManager.CaseManagerClient cmsAdapterClient)
        {
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;
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
        /// <param name="caseId"></param>
        /// <returns></returns>
        // GET: /Drivers/Exist
        [HttpGet("{licenseNumber}/Comments")]
        [ProducesResponseType(typeof(List<ViewModels.Comment>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]        
        public ActionResult GetComments([FromRoute] string licenseNumber)
        
        {            
            // call the back end

            var reply = _cmsAdapterClient.GetDriverComments( new DriverLicenseRequest() { DriverLicenseNumber = licenseNumber } );

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

                    result.Add(new ViewModels.Comment
                    {
                        CaseId = item.CaseId,
                        CommentDate = item.CommentDate.ToDateTimeOffset(),
                        CommentId = item.CommentId,
                        CommentText = item.CommentText,
                        CommentTypeCode = item.CommentTypeCode,
                        Driver = driver,
                        SequenceNumber = item.SequenceNumber,
                        UserId = item.UserId
                    });
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
                    result.Add(new ViewModels.Case() { CaseId = item.CaseId});
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
        public ActionResult CreateCommentForDriver([FromRoute] string licenseNumber, [FromBody] ViewModels.Comment comment )
        {
            // add the comment

            var driver = new CaseManagement.Service.Driver()
            {
                DriverLicenseNumber = licenseNumber                
            };

            if (comment.Driver != null)
            {
                driver.Surname = comment.Driver.LastName;
            }            

            var result = _cmsAdapterClient.CreateLegacyCaseComment(new LegacyComment()
            {
                CaseId = comment.CaseId ?? String.Empty,
                CommentText = comment.CommentText ?? String.Empty,
                CommentTypeCode = comment.CommentTypeCode ?? String.Empty,
                SequenceNumber = comment.SequenceNumber,
                UserId = comment.UserId ?? String.Empty,
                CommentDate = Timestamp.FromDateTimeOffset(comment.CommentDate),
                Driver = driver
            });

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
                return StatusCode(500);
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

            var reply = _cmsAdapterClient.GetDriverDocuments(new DriverLicenseRequest() { DriverLicenseNumber = licenseNumber });

            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // get the comments
                List<ViewModels.Document> result = new List<ViewModels.Document>();

                foreach (var item in reply.Items)
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

                    // fetch the file contents
                    Byte[] data = new byte[0];

                    result.Add(new ViewModels.Document
                    {
                        CaseId = item.CaseId,
                        FaxReceivedDate = item.FaxReceivedDate.ToDateTimeOffset(),
                        ImportDate = item.ImportDate.ToDateTimeOffset(),

                        DocumentId = item.DocumentId,
                        FileContents = data,                        
                        Driver = driver,
                        SequenceNumber = item.SequenceNumber,
                        UserId = item.UserId
                    });
                }
                return Json(result);
            }
            else
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// Add a document to a case
        /// </summary>
        /// <param name="licenseNumber">Driver Licence</param>
        /// <param name="document">The document to add</param>
        /// <returns></returns>
        [HttpPost("{licenseNumber}/Documents")]
        // allow large uploads
        [DisableRequestSizeLimit]
        public ActionResult CreateDocumentForDriver([FromRoute] string licenseNumber, [FromBody] ViewModels.Document document)
        {
            // add the document

            var driver = new CaseManagement.Service.Driver()
            {
                DriverLicenseNumber = licenseNumber
            };

            if (document.Driver != null)
            {
                driver.Surname = document.Driver.LastName;
            }

            var result = _cmsAdapterClient.CreateLegacyCaseDocument(new LegacyDocument()
            {
                CaseId = document.CaseId,                
                SequenceNumber = document.SequenceNumber,
                UserId = document.UserId,
                FaxReceivedDate = Timestamp.FromDateTimeOffset(document.FaxReceivedDate),
                ImportDate = Timestamp.FromDateTimeOffset(document.ImportDate),
                Driver = driver

            });

            if (result.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                var actionName = nameof(CreateDocumentForDriver);
                var routeValues = new
                {
                    driversLicence = licenseNumber
                };

                return CreatedAtAction(actionName, routeValues, document);
            }
            else
            {
                return StatusCode(500);
            }
        }

    }
}
