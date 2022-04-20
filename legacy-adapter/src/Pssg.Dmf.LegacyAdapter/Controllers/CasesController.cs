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
    public class CasesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CasesController> _logger;


        public CasesController(ILogger<CasesController> logger, IConfiguration configuration)
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
        // GET: /Cases/Exist
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
        // GET: /Cases/Exist
        [HttpGet("{caseId}/Comments")]
        public ActionResult GetComments([FromRoute] string caseId)
        {
            bool result = false;
            // get the comments
            return Json(result);
        }

        /// <summary>
        /// Add a comment to a case
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost("{caseId}/Comments")]
        public ActionResult CreateComments([FromRoute] string caseId, [FromBody] string comment )
        {
            bool result = false;
            // get the comments
            return Json(result);
        }

        [HttpGet("{caseId}/Documents")]
        public ActionResult GetDocuments([FromRoute] string caseId)
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
