using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : Controller
    {
        private readonly CallbackManager.CallbackManagerClient _callbackManagerClient;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CallbackController(CallbackManager.CallbackManagerClient callbackManagerClient, IUserService userService, IMapper mapper)
        {
            _callbackManagerClient = callbackManagerClient;
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get Callbacks for the driver
        /// </summary>        
        [HttpGet("driver")]
        [Authorize(Policy = Policy.Driver)]
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
    }
}