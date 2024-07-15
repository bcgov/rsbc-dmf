using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.PartnerPortal.Api.Services;
using System.Net;


namespace Rsbc.Dmf.PartnerPortal.Api.Controllers
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
        )
        {
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _userService = userService;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<CasesController>();
        }

        /// <summary>
        /// Get closed documents for a given driver
        /// </summary>
        /// <returns></returns>
        [HttpGet("Closed")]
       // [Authorize(Policy = Policy.Driver)]
        [ProducesResponseType(typeof(IEnumerable<CaseDetail>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName("GetClosedCases")]
        public async Task<ActionResult> GetClosedCases()
        {
            try
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

                    // sort the documents
                    if (result.Count > 0)
                    {
                        result = result.OrderByDescending(cs => cs.OpenedDate).ToList();
                    }

                    return Json(result);
                }
                else
                {
                    _logger.LogError($"{nameof(GetClosedCases)} failed for driverId: {profile.DriverId}", reply.ErrorDetail);
                    return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetClosedCases)} failed", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get Most Recent Case
        /// </summary>        
        [HttpGet("MostRecent")]
        //[Authorize(Policy = Policy.Driver)]
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
            }

            return Json(result);
        }

    }
}
