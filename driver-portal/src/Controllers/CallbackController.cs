using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : Controller
    {
        private readonly CallbackManager.CallbackManagerClient _callbackManagerClient;
        private readonly IMapper _mapper;

        public CallbackController(CallbackManager.CallbackManagerClient callbackManagerClient, IMapper mapper)
        {
            _callbackManagerClient = callbackManagerClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Get Callbacks for the driver
        /// </summary>        
        [HttpGet("{driverId}")]
        [Authorize(Policy = Policy.Driver)]
        [ProducesResponseType(typeof(DriverCallbacks), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetDriverCallbacks))]
        public ActionResult GetDriverCallbacks([Required][FromRoute] string driverId)
        {
            var result = new DriverCallbacks();

            var driverIdRequest = new DriverIdRequest { Id = driverId };
            var driverCallbacks = _callbackManagerClient.GetDriverCallbacks(driverIdRequest);
            if (driverCallbacks?.ResultStatus == ResultStatus.Success)
            {
                result.DriverId = driverCallbacks.DriverId;
                result.Callbacks = _mapper.Map<List<ViewModels.Callback>>(driverCallbacks.Items);
            }
            else
            {
                return StatusCode(500, driverCallbacks?.ErrorDetail ?? "GetDriverCallbacks failed.");
            }

            return Json(result);
        }
    }
}