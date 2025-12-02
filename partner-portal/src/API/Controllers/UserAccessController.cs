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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PartnerPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccessController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<CasesController> _logger;
        private readonly ICachedIcbcAdapterClient _icbcAdapterClient;

        public UserAccessController(
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

        [HttpPost("create")]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName(nameof(CreateUser))]
        public async Task<IActionResult> CreateUser([FromBody] CallbackRequest callbackRequest)
        {
            return null;
        }
    }
}
