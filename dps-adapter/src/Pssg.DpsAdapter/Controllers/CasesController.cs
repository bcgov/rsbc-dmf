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


        // GET: /DriverHistory
        [HttpGet()]
        public ActionResult GetCase(string driversLicence)
        {
            // get the case
            
            string result = "test";

                        
            return Json (result);
        }

    }
}
