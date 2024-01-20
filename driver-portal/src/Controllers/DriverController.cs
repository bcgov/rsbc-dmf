using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
using Rsbc.Dmf.DriverPortal.ViewModels;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : Controller
    {
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly IMapper _mapper;
        private readonly ILogger<DriverController> _logger;

        public DriverController(CaseManager.CaseManagerClient cmsAdapterClient, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _cmsAdapterClient = cmsAdapterClient;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<DriverController>();
        }

        /// <summary>
        /// Get case submissions, submission requirements, and letters to driver documents for a given driver
        /// NOTE that this retrieves all documents for the driver and there is no guarantee that a document is linked to a case
        /// </summary>
        /// <param name="driverId">The driver id</param>
        /// <returns>CaseDocuments</returns>
        [HttpGet("{driverId}/Documents")]
        [AuthorizeDriver]
        [ProducesResponseType(typeof(CaseDocuments), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetCaseDocuments")]
        public ActionResult GetCaseDocuments([FromRoute] string driverId)
        {
            var driverIdRequest = new DriverIdRequest() { Id = driverId };
            var reply = _cmsAdapterClient.GetDriverDocumentsById(driverIdRequest);
            if (reply.ResultStatus == ResultStatus.Success)
            {
                var result = new CaseDocuments();

                foreach (var item in reply.Items)
                {
                    if (item.SubmittalStatus != "Uploaded")
                    {
                        var document = _mapper.Map<Document>(item);

                        switch (document.SubmittalStatus)
                        {
                            case "Received":
                                // exclude documents with no key
                                if (!string.IsNullOrEmpty(document.DocumentUrl))
                                {
                                    result.CaseSubmissions.Add(document);
                                }
                                break;
                            case "Open-Required":
                                // this category by design has documents with no key
                                result.SubmissionRequirements.Add(document);
                                break;
                            case "Sent":
                                // exclude documents with no key
                                if (!string.IsNullOrEmpty(document.DocumentUrl))
                                {
                                    result.LettersToDriver.Add(document);
                                }
                                break;
                        }
                    }
                }

                // sort the documents
                if (result.CaseSubmissions.Count > 0)
                {
                    result.CaseSubmissions = result.CaseSubmissions.OrderByDescending(cs => cs.ImportDate).ToList();
                }

                if (result.SubmissionRequirements.Count > 0)
                {
                    result.SubmissionRequirements = result.SubmissionRequirements.OrderByDescending(cs => cs.ImportDate).ToList();
                }

                if (result.LettersToDriver.Count > 0)
                {
                    result.LettersToDriver = result.LettersToDriver.OrderByDescending(cs => cs.ImportDate).ToList();
                }

                return Json(result);
            }
            else
            {
                _logger.LogError($"{nameof(GetCaseDocuments)} failed for driverId: {driverId}", reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }
        }

        /// <summary>
        /// Get all documents for a given driver but filter out documents without a url
        /// </summary>
        /// <param name="driverId">The driver id</param>
        /// <returns>CaseDocuments</returns>
        [HttpGet("{driverId}/AllDocuments")]
        [AuthorizeDriver]
        [ProducesResponseType(typeof(IEnumerable<Document>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetAllDocuments")]
        public ActionResult GetAllDocuments([FromRoute] string driverId)
        {
            var driverIdRequest = new DriverIdRequest() { Id = driverId };
            var reply = _cmsAdapterClient.GetDriverDocumentsById(driverIdRequest);
            if (reply.ResultStatus == ResultStatus.Success)
            {
                var result = new List<Document>();

                foreach (var item in reply.Items)
                {
                    var document = _mapper.Map<Document>(item);
                    if (!string.IsNullOrEmpty(document.DocumentUrl))
                    {
                        result.Add(document);
                    }
                }

                // sort the documents
                if (result.Count > 0)
                {
                    result = result.OrderByDescending(cs => cs.ImportDate).ToList();
                }

                return Json(result);
            }
            else
            {
                _logger.LogError($"{nameof(GetAllDocuments)} failed for driverId: {driverId}", reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }
        }
    }
}