using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
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

        [HttpGet("create")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(Create))]
        public async Task<IActionResult> Create([FromBody] BringForwardRequest callback)
        {
            var profile = await _userService.GetCurrentUserContext();

            // security check
            var @case = _caseManagerClient.GetMostRecentCaseDetail(new DriverIdRequest { Id = profile.DriverId });
            callback.CaseId = @case.Item.CaseId;

            // create callback
            var reply = _caseManagerClient.CreateBringForward(callback);
            if (reply.ResultStatus != ResultStatus.Success)
            {
                return StatusCode(500, reply.ErrorDetail ?? $"{nameof(Create)} failed.");
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

        [HttpGet("cancel")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(Cancel))]
        public async Task<IActionResult> Cancel([FromBody] CallbackIdRequest callback)
        {
            var profile = await _userService.GetCurrentUserContext();

            // get the driver's callbacks
            var driverIdRequest = new DriverIdRequest { Id = profile.DriverId };
            var driverCallbacks = _callbackManagerClient.GetDriverCallbacks(driverIdRequest);
            if (driverCallbacks.ResultStatus != ResultStatus.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, driverCallbacks?.ErrorDetail ?? $"{nameof(GetDriverCallbacks)} failed.");
            }

            // security check
            if (!driverCallbacks.Items.Any(c => c.Id == callback.Id))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Callback not found.");
            }

            // cancel callback
            var reply = _callbackManagerClient.Cancel(callback);
            if (reply.ResultStatus != ResultStatus.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail ?? $"{nameof(Cancel)} failed.");
            }
            else
            {
                return Ok();
            }
        }
    }
}