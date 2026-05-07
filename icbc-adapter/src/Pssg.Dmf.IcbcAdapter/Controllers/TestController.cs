using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.ViewModels;
using Pssg.Interfaces.IcbcModels;
using Pssg.Interfaces.ViewModelExtensions;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.IcbcAdapter.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DriverHistoryController> _logger;
        private readonly IIcbcClient _icbcClient;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;

        public TestController(ILogger<DriverHistoryController> logger, IConfiguration configuration, IIcbcClient icbcClient, CaseManager.CaseManagerClient cmsAdapterClient)
        {
            _configuration = configuration;
            _logger = logger;
            _icbcClient = icbcClient;
            _cmsAdapterClient = cmsAdapterClient;
        }

        /// <summary>
        /// CreateCadidate is used to create a candidate in the system.  
        /// It will fetch driver details and use that to add a canadiate to the case management system.
        /// </summary>
        /// <param name="dlNumber">The Drivers License Number to fetch</param>
        /// <returns>True if successful</returns>
        [HttpGet("CreateCandidate")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Get))]
        public ActionResult CreateCandidate (string dlNumber)
        {
            CLNT client = _icbcClient.GetDriverHistory(dlNumber);

            LegacyCandidateRequest lcr = new LegacyCandidateRequest()
            {
                LicenseNumber = dlNumber,
                Surname = client.INAM.SURN
            };
            var reply = _cmsAdapterClient.ProcessLegacyCandidate(lcr);

            var result = reply.ResultStatus == CaseManagement.Service.ResultStatus.Success;

            return Ok ( result.ToString());
        }

    }
}
