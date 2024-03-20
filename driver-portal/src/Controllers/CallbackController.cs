using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
using SharedUtils;
using System.Net;
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = Policy.Driver)]
    [ApiController]
    public class CallbackController : Controller
    {
        private readonly CallbackManager.CallbackManagerClient _callbackManagerClient;
        // TODO move CreateBringForward to CallbackService and then remove CaseManagerClient from this controller
        private readonly CaseManagerClient _caseManagerClient;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CallbackController(CallbackManager.CallbackManagerClient callbackManagerClient, CaseManager.CaseManagerClient caseManagerClient, IUserService userService, IMapper mapper)
        {
            _callbackManagerClient = callbackManagerClient;
            _caseManagerClient = caseManagerClient;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> Create([FromBody] Callback callback)
        {
            var profile = await _userService.GetCurrentUserContext();

            // security check
            var mostRecentCaseReply = _caseManagerClient.GetMostRecentCaseDetail(new DriverIdRequest { Id = profile.DriverId });
            if (mostRecentCaseReply.ResultStatus != ResultStatus.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, mostRecentCaseReply.ErrorDetail ?? $"{nameof(Cancel)} security failed.");
            }

            callback.CaseId = mostRecentCaseReply.Item.CaseId;
            callback.Origin = (int)UserCode.Portal;
            callback.Priority = CallbackPriority.Normal;

            // create callback
            var reply = _callbackManagerClient.Create(callback);
            if (reply.ResultStatus != ResultStatus.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail ?? $"{nameof(Create)} failed.");
            }
            else
            {
                return Ok();
            }
        }
     
        [HttpGet("driver")]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.Callback>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetDriverCallbacks))]
        public async Task<ActionResult> GetDriverCallbacks()
        {
            var result = new List<ViewModels.Callback>();

            var profile = await _userService.GetCurrentUserContext();

            var driverIdRequest = new DriverIdRequest { Id = profile.DriverId };
            var driverCallbacks = _callbackManagerClient.GetDriverCallbacks(driverIdRequest);
            if (driverCallbacks?.ResultStatus == ResultStatus.Success)
            {
                result = _mapper.Map<List<ViewModels.Callback>>(driverCallbacks.Items);
            }
            else
            {
                return StatusCode(500, driverCallbacks?.ErrorDetail ?? $"{nameof(GetDriverCallbacks)} failed.");
            }

            return Json(result);
        }

        [HttpPut("cancel")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(Cancel))]
        public async Task<IActionResult> Cancel([FromBody] CallbackCancelRequest callback)
        {
            var profile = await _userService.GetCurrentUserContext();

            // security check by using user owned case
            var mostRecentCaseReply = _caseManagerClient.GetMostRecentCaseDetail(new DriverIdRequest { Id = profile.DriverId });
            if (mostRecentCaseReply.ResultStatus != ResultStatus.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, mostRecentCaseReply.ErrorDetail ?? $"{nameof(Cancel)} security failed.");
            }

            // cancel callback
            var callbackCancelRequest = new CallbackCancelRequest
            {
                CaseId = mostRecentCaseReply.Item.CaseId,
                CallbackId = callback.CallbackId,
            };
            var reply = _callbackManagerClient.Cancel(callbackCancelRequest);
            if (reply.ResultStatus != ResultStatus.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail ?? $"{nameof(Cancel)} cancel failed.");
            }
            else
            {
                return Ok();
            }
        }
    }
}