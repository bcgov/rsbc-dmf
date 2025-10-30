using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter.Client;
using Rsbc.Dmf.PartnerPortal.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rsbc.Dmf.CaseManagement.Service.CommentManager;
using ResultStatus = Rsbc.Dmf.CaseManagement.Service.ResultStatus;

namespace Rsbc.Dmf.PartnerPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemedialController: Controller
    {

        private readonly IConfiguration _configuration;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<RemedialController> _logger;
        private readonly ICachedIcbcAdapterClient _icbcAdapterClient;

        public RemedialController(
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
            _logger = loggerFactory.CreateLogger<RemedialController>();
            _icbcAdapterClient = icbcAdapterClient;
        }


        [HttpGet("getIgnitionInterlock")]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.IgnitionInterlock>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetIgnintionInterlockDetails))]
        public async Task<ActionResult> GetIgnintionInterlockDetails()
        {
            var result = new List<ViewModels.IgnitionInterlock>();

            var profile = _userService.GetDriverInfo();

            var request = new DriverIdRequest { Id = profile.DriverId };
            var getIgnitionInterlockDetails = _cmsAdapterClient.GetIgnitionInterlockDetails(request);

            if (getIgnitionInterlockDetails?.ResultStatus == ResultStatus.Success)
            {
                result = _mapper.Map<List<ViewModels.IgnitionInterlock>>(getIgnitionInterlockDetails.Items);
            }
            else
            {
                return StatusCode(500, getIgnitionInterlockDetails?.ErrorDetail ?? $"{nameof(getIgnitionInterlockDetails)} failed.");
            }

            return Json(result);
        }
    }
}
