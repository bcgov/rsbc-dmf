using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter.Client;
using Rsbc.Dmf.PartnerPortal.Api.Services;
using Rsbc.Dmf.PartnerPortal.Api.ViewModels;
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
        private readonly ICachedIcbcAdapterClient _icbcAdapterClient;

        public CasesController(
            IConfiguration configuration,
            CaseManager.CaseManagerClient cmsAdapterClient,
            DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient,
            IUserService userService,
            IMapper mapper,
            ILoggerFactory loggerFactory,
            ICachedIcbcAdapterClient icbcAdapterClient
        )
        {
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _userService = userService;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<CasesController>();
            _icbcAdapterClient = icbcAdapterClient;
        }

        // Get closed documents for a given driver
        [HttpGet("Closed")]
        //[Authorize(Policy = Policy.Driver)]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.CaseDetail>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName("GetClosedCases")]
        public async Task<ActionResult> GetClosedCases()
        {
           try
           {
                var user = _userService.GetDriverInfo();
                var caseStatusRequest = new CaseStatusRequest() { DriverId = user.DriverId, Status = EntityState.Inactive };
                var reply = _cmsAdapterClient.GetCases(caseStatusRequest);
                if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
                {
                    var result = new List<ViewModels.CaseDetail>();
                    result = _mapper
                        .Map<IEnumerable<ViewModels.CaseDetail>>(reply.Items)
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
                    _logger.LogError($"{nameof(GetClosedCases)} failed for driverId: {user.DriverId}", reply.ErrorDetail);
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
        [ProducesResponseType(typeof(ViewModels.CaseDetail), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("MostRecent")]
        public async Task<ActionResult> GetMostRecentCase([FromQuery] string programArea)
        {
            var result = new ViewModels.CaseDetail();
            var profile =  _userService.GetDriverInfo();

            var request = new DriverIdRequest { Id = profile.DriverId };

            if (!string.IsNullOrEmpty(programArea))
            {
                request.CaseFilter = new CaseFilterRequest { ProgramArea = programArea };
            }
            //var request = new DriverIdRequest { Id = profile.DriverId, CaseFilter = new CaseFilterRequest { ProgramArea = null } };

            var c = await _cmsAdapterClient.GetMostRecentCaseDetailAsync(request);

            //var c = _cmsAdapterClient.GetMostRecentCaseDetail(new DriverIdRequest { Id = profile.DriverId, CaseFilter = { ProgramArea = "Remedial" } });
            if (c != null && c.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                result = _mapper.Map<ViewModels.CaseDetail>(c.Item);
            }
            else
            {
                if (c.ResultStatus == CaseManagement.Service.ResultStatus.Fail)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, c.ErrorDetail);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NotFound, "Most recent case not found.");
                }
            }

            return Json(result);
        }

        [HttpGet("search/{idCode}")]
        [ProducesResponseType(typeof(CaseSearch), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("SearchCaseByIdCode")]
        public async Task<ActionResult> SearchCaseByIdCode([FromRoute] string idCode)
        {
            var profile = _userService.GetDriverInfo();

            var result = new ViewModels.CaseSearch();

            if (string.IsNullOrEmpty(idCode) || idCode == Guid.Empty.ToString())
            {
                return BadRequest("The Case Number was invalid.");
            }

            // get case by id code
            var getCaseByIdCodeRequest = new GetCaseByIdCodeRequest { IdCode = idCode };
            var @case = await _cmsAdapterClient.GetCaseByIdCodeAsync(getCaseByIdCodeRequest);
            if (@case?.Item?.CaseId == null)
            {
                return NotFound();
            }
            if (@case.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Fail)
            {
                _logger.LogError($"Error getting case by id code: {0}", @case.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to get case.");
            }


            if (@case != null && @case.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                result = new ViewModels.CaseSearch();

                result.CaseId = @case.Item.CaseId;
                result.DriverLicenseNumber = @case.Item.DriverLicenseNumber;
                result.IdCode = @case.Item.IdCode;
                result.DriverId = @case.Item.DriverId;
             
                // get driver info from ICBC
                if (@case.Item.DriverLicenseNumber != null)
                {
                    result.DriverLicenseNumber = @case.Item.DriverLicenseNumber;
                    var request = new IcbcAdapter.DriverInfoRequest();
                    request.DriverLicence = @case.Item.DriverLicenseNumber;
                    var driverInfoReply = await _icbcAdapterClient.GetDriverInfoAsync(request);
                    if (driverInfoReply.ResultStatus == Rsbc.Dmf.IcbcAdapter.ResultStatus.Fail)
                    {
                        _logger.LogError($"Failed to get icbc driver info details: {0}", driverInfoReply.ErrorDetail);
                        return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to get icbc driver info details.");
                    }
                    result.FirstName = driverInfoReply.GivenName;
                    result.LastName = driverInfoReply.Surname;
                    
                }
            }

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
