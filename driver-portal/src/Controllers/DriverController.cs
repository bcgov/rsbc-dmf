using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
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
        /// Get documents for a given driver
        /// </summary>
        /// <param name="driverId">The driver id</param>
        /// <returns>CaseDocuments</returns>
        [HttpGet("{driverId}/Documents")]
        [AllowAnonymous]
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
                _logger.LogError($"{nameof(GetDocuments)} failed for driverId: {driverId}", reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }
        }
    }
}