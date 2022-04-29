using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Pssg.Dmf.LegacyAdapter.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class DriversController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DriversController> _logger;


        public DriversController(ILogger<DriversController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;

        }

        /// <summary>
        /// DoesCaseExist
        /// </summary>
        /// <param name="driversLicense"></param>
        /// <param name="surcode"></param>
        /// <returns>True if the case exists</returns>
        // GET: /Drivers/Exist
        [HttpGet("Exist")]
        public ActionResult DoesCaseExist(string driversLicense, string surcode)
        {
            bool result = false;
            // get the case                                                
            return Json (result);
        }

        /// <summary>
        /// Get Comments for a case
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        // GET: /Drivers/Exist
        [HttpGet("{driversLicense}/Comments")]
        [ProducesResponseType(typeof(List<ViewModels.Comment>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult GetComments([FromRoute] string driversLicense)
        
        {
            // get the comments
            List<ViewModels.Comment> result = new List<ViewModels.Comment>();
            result.Add (new ViewModels.Comment() { caseId = Guid.NewGuid().ToString(), commentText = "SAMPLE TEXT", commentTypeCode="W", driversLicense = driversLicense, sequenceNumber = 0, userId = "TESTUSER" });
            
            return Json(result);
        }

        /// <summary>
        /// Add a comment to a case
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost("{driversLicense}/Comments")]
        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult CreateComments([FromRoute] string driversLicense, [FromBody] ViewModels.Comment comment )
        {            
            // add the comment
            return CreatedAtAction("Comments", comment);
        }

        [HttpGet("{driversLicense}/Documents")]
        public ActionResult GetDocuments([FromRoute] string driversLicense)
        {
            bool result = false;
            // get the comments
            return Json(result);
        }


        /// <summary>
        /// Add a document to a case
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="driversLicense"></param>
        /// <param name="surcode"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{caseId}/Documents")]
        // allow large uploads
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UpdateCaseDocuments([FromRoute] string caseId, [FromForm] string driversLicense, [FromForm] string surcode,
            [FromForm] IFormFile file)
        {
            return Ok();
        }
        

        


    }
}
