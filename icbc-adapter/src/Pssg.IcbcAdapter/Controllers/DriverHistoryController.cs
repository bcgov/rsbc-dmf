using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pssg.IcbcAdapter.ViewModels;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;

namespace Pssg.IcbcAdapter.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class DriverHistoryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DriverHistoryController> _logger;
        private readonly IcbcClient icbcClient;

        public DriverHistoryController(ILogger<DriverHistoryController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            icbcClient = new IcbcClient(configuration);
        }


        // GET: /DriverHistory
        [HttpGet()]
        public ActionResult GetHistory(string driversLicence)
        {
            // get the history from ICBC
            CLNT result = icbcClient.GetDriverHistory(driversLicence);
                        
            return Json (result);
        }

    }
}
