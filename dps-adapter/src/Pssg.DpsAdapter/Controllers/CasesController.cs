using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Pssg.IcbcAdapter.Controllers
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


        // GET: /Cases/Exist
        [HttpGet("Exist")]
        public ActionResult DoesCaseExist(string driversLicense, string surcode)
        {
            bool result = false;
            // get the case
            
                                    
            return Json (result);
        }

        [HttpPost("Documents")]
        // allow large uploads
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UpdateCaseDocuments([FromForm] string driversLicense, [FromForm] string surcodee,
            [FromForm] IFormFile file)
        {
            return Ok();
        }        

    }
}
