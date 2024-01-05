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
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly IMapper _mapper;

        public CallbackController(CaseManager.CaseManagerClient cmsAdapterClient, IMapper mapper)
        {
            _cmsAdapterClient = cmsAdapterClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Get Case
        /// </summary>        
        [HttpGet("{driverId}")]
        [ProducesResponseType(typeof(DriverCallbacks), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetDriverCallbacks")]

        // TODO REMOVE THIS
        [AllowAnonymous]

        public ActionResult GetDriverCallbacks([Required][FromRoute] string driverId)
        {
            var result = new DriverCallbacks();

            var driverIdRequest = new DriverIdRequest { Id = driverId };
            var driverCallbacks = _cmsAdapterClient.GetDriverCallbacks(driverIdRequest);
            if (driverCallbacks?.ResultStatus == ResultStatus.Success)
            {
                result.DriverId = driverCallbacks.DriverId;
                result.Callbacks = new List<ViewModels.Callback>();
                foreach (var callback in driverCallbacks.Items)
                {
                    var callbackViewModel = _mapper.Map<ViewModels.Callback>(callback);
                    result.Callbacks.Add(callbackViewModel);
                }
            }
            else
            {
                return StatusCode(500, driverCallbacks?.ErrorDetail ?? "GetDriverCallbacks failed.");
            }

            return Json(result);
        }
    }
}