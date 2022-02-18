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
using Pssg.Interfaces.Icbc.ViewModels;

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
            CLNT data = icbcClient.GetDriverHistory(driversLicence);

            Driver result = new Driver()
            {
                Surname = data.INAM?.SURN,
                GivenName = data.INAM?.GIV1,                
                Weight = data.WGHT,
                Sex = data.SEX,
                BirthDate = data.BIDT,
                Height = data.HGHT,
                SecurityKeyword = data.SECK
            };

            // handle two middle names, or just one.
            if (data.INAM?.GIV2 != null && data.INAM?.GIV3 != null)
            {
                result.MiddleName = $"{data.INAM.GIV2} {data.INAM.GIV3}";
            }
            else if (data.INAM?.GIV2 != null)
            {
                result.MiddleName = $"{data.INAM.GIV2}";
            }
            else if (data.INAM?.GIV3 != null)
            {
                result.MiddleName = $"{data.INAM.GIV3}";
            }           

            result.DriverMasterStatus = new DriverMasterStatus()
            {
                MasterStatusCode = data.DR1MST?.MSCD,
                RestrictionCodes = new List<int>(), // TODO
                LicenceExpiryDate = data.DR1MST?.RRDT,
                LicenceNumber = data.DR1MST?.LNUM,
                DriverStatus = new DriverStatus(), // TODO
                LicenceClass = data.DR1MST?.LCLS,
                DriverMedicals = new List<DriverMedical>() //data.DR1MST?.DR1MEDN,
            };

            return Json (result);
        }

    }
}
