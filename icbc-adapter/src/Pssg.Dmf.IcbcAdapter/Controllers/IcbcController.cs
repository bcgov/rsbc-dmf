using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.ViewModels;
using Pssg.Interfaces.ViewModelExtensions;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace Rsbc.Dmf.IcbcAdapter.Controllers
{

    public class NewCandidate
    {
        /// <summary>
        /// Driver's License Number
        /// </summary>
        [SwaggerSchema("Driver's License Number")]
        public string DlNumber { get; set; }
        [SwaggerSchema("Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Date that the requirement for a Medical Exam was issued
        /// </summary>
        [SwaggerSchema("Date that the requirement for a Medical Exam was issued")]
        public DateTimeOffset? EffectiveDate { get; set; }

        /// <summary>
        /// Birthdate for the Driver
        /// </summary>
        [SwaggerSchema("Birthdate for the Driver")]
        public DateTimeOffset? BirthDate { get; set; }

    }

    public class CaseStatus
    {
        public string CaseId { get; set; }
        public string DlNumber { get; set; }
        public string Status { get; set; }
    }

    

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class IcbcController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DriverHistoryController> _logger;
        private readonly IcbcClient icbcClient;

        public IcbcController(ILogger<DriverHistoryController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            icbcClient = new IcbcClient(configuration);
        }
        
        /// <summary>
        /// POST: /Icbc/Candidates
        /// </summary>
        /// <param name="newCandidates">List of Candidates to be added to the case management system</param>
        /// <returns></returns>
        [HttpPost("Candidates")]
        [SwaggerResponse(200, "The candidates were processed correctly")]
        [SwaggerResponse(400, "The format of the provided data was invalid.  Please refer to the model.")]
        [SwaggerResponse(500, "An unexpected server error occurred while processing. Please retry.")]
        [AllowAnonymous]
        public ActionResult CreateCandidates ([FromBody] List<NewCandidate> newCandidates )
        {

            // check for duplicates; if there is an existing case then do not create a new one
            return Ok();
        }

        /// <summary>
        /// POST: /Icbc/Candidates
        /// </summary>
        /// <param name="newCandidates">List of Candidates to be added to the case management system</param>
        /// <returns></returns>
        [HttpPost("CandidatesError")]
        [SwaggerResponse(200, "The candidates were processed correctly")]
        [SwaggerResponse(400, "The format of the provided data was invalid.  Please refer to the model.")]
        [SwaggerResponse(500, "An unexpected server error occurred while processing. Please retry.")]
        [AllowAnonymous]
        public ActionResult CreateCandidatesError([FromBody] List<NewCandidate> newCandidates)
        {
            throw new Exception("Sample Error.");
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("Cases")]
        public CaseStatus GetCaseStatus(string caseId)
        {
            return new CaseStatus();
        }



    }
}
