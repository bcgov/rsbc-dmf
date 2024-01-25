using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using CaseDetail = Rsbc.Dmf.DriverPortal.ViewModels.CaseDetail;
using Pssg.DocumentStorageAdapter;
using AutoMapper;
using Rsbc.Dmf.DriverPortal.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CasesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<CasesController> _logger;

        public CasesController(
            IConfiguration configuration, 
            CaseManager.CaseManagerClient cmsAdapterClient,
            DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient, 
            IUserService userService,
            IMapper mapper, 
            ILoggerFactory loggerFactory
        ) {
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _userService = userService;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<CasesController>();
        }

        /// <summary>
        /// Get Case
        /// </summary>        
        [HttpGet("{caseId}")]
        [ProducesResponseType(typeof(CaseDetail), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetCase")]
        public ActionResult GetCase([Required] [FromRoute] string caseId)
        {
            var c = _cmsAdapterClient.GetCaseDetail(new CaseIdRequest { CaseId = caseId });

            if (c != null && c.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                var result = _mapper.Map<CaseDetail>(c.Item);
                return Json(result);
            }
            else
            {
                return StatusCode(500, c?.ErrorDetail ?? "GetCaseDetail failed.");
            }
        }

        /// <summary>
        /// Get closed documents for a given driver
        /// </summary>
        /// <returns></returns>
        [HttpGet("Closed")]
        [Authorize(Policy = Policy.Driver)]
        [ProducesResponseType(typeof(IEnumerable<CaseDetail>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetClosedCases")]
        public async Task<ActionResult> GetClosedCases()
        {
            var profile = await _userService.GetCurrentUserContext();

            var caseStatusRequest = new CaseStatusRequest() { DriverId = profile.DriverId, Status = EntityState.Inactive };
            var reply = _cmsAdapterClient.GetCases(caseStatusRequest);
            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                var result = new List<CaseDetail>();
                result = _mapper
                    .Map<IEnumerable<CaseDetail>>(reply.Items)
                    .ToList();

                return Json(result);
            }
            else
            {
                _logger.LogError($"{nameof(GetClosedCases)} failed for driverId: {profile.DriverId}", reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }
        }

        /// <summary>
        /// Get Most Recent Case
        /// </summary>        
        [HttpGet("MostRecent")]
        [ProducesResponseType(typeof(CaseDetail), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("MostRecent")]
        public async Task<ActionResult> GetMostRecentCase()
        {
            var result = new CaseDetail();

            var profile = await _userService.GetCurrentUserContext();

            var c = _cmsAdapterClient.GetMostRecentCaseDetail(new DriverIdRequest { Id = profile.DriverId });
            if (c != null && c.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                result = _mapper.Map<CaseDetail>(c.Item);
                // these properties are needed in other mappings but not here, only show minimal information
                result.DriverId = null;
                result.EligibleLicenseClass = null;
            }
            
            return Json(result);
        }

        /// <summary>
        /// Returns the current user's cases
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [ProducesResponseType(typeof(List<ViewModels.CaseDetail>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetCases")]
        public ActionResult GetCases()
        {
            var result = new List<ViewModels.CaseDetail>();
            result.Add(new CaseDetail() { Title = "Test-Title", CaseId = Guid.Empty.ToString(), DmerType = "DMER" });
            return Json(result);
        }

        /// <summary>
        /// Returns the current Closed Cases
        /// </summary>
        /// <returns></returns>
      /*  [HttpGet()]
        [ProducesResponseType(typeof(List<ViewModels.CaseDetail>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetClosedCases")]
        public ActionResult GetClosedCases()
        {
            var result = new List<ViewModels.CaseDetail>();
            result.Add(new CaseDetail() { 
                CaseId = Guid.Empty.ToString(),
                CaseType = "Driver’s Medical Examination Report",
                LatestDecision =    "Fit To Drive",
                EligibleLicenseClass = "5",
                OpenedDate = DateTime.Now,
                DecisionDate = DateTime.Now.AddDays(5),
            });
            return Json(result);
        }*/
    }

}
