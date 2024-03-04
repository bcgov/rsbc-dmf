using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Pssg.DocumentStorageAdapter;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.ViewModels;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.LegacyAdapter.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace Rsbc.Dmf.LegacyAdapter.Controllers
{

    /// <summary>
    /// Decisions Controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class DecisionsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CasesController> _logger;


        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly IIcbcClient _icbcClient;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Cases Controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="cmsAdapterClient"></param>
        /// <param name="documentStorageAdapterClient"></param>
        /// <param name="icbcClient"></param>
        public DecisionsController(ILogger<CasesController> logger, IConfiguration configuration, CaseManager.CaseManagerClient cmsAdapterClient, DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient, IIcbcClient icbcClient,
            IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _logger = logger;
            _icbcClient = icbcClient;
        }


        /// <summary>
        /// Add Decision
        /// </summary>
        /// <param name="decision"></param>
        /// <returns></returns>
        [HttpPost()]
        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult CreateDecision([FromBody] ViewModels.Decision decision)
        {
            var request = new LegacyDecision
            {
                CaseId = decision.CaseId ?? string.Empty,
                DriverId = decision.DriverId ?? string.Empty,
                OutcomeText = decision.OutcomeText ?? string.Empty,
                SubOutcomeText = decision.SubOutcomeText ?? string.Empty,
                StatusDate = Timestamp.FromDateTimeOffset(decision.StatusDate),
            };
            var reply = _cmsAdapterClient.CreateDecision(request);

            return Ok();
        }

    }
}
