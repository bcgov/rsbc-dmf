using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.ViewModels;
using Pssg.Interfaces.ViewModelExtensions;
using System;
using System.Collections.Generic;
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;

namespace Rsbc.Dmf.IcbcAdapter.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DriverHistoryController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DriverHistoryController> _logger;
        private readonly IIcbcClient _icbcClient;
        private readonly EnhancedIcbcApiUtils _enhancedIcbcUtils;


        public DriverHistoryController(ILogger<DriverHistoryController> logger, IConfiguration configuration, IIcbcClient icbcClient, CaseManagerClient caseManagerClient, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _logger = logger;
            _icbcClient = icbcClient;
            _enhancedIcbcUtils = new EnhancedIcbcApiUtils(configuration, caseManagerClient, icbcClient);
        }

        // GET: /DriverHistory
        [HttpGet()]
        public ActionResult GetHistory(string driversLicence)
        {
            // first check that the item is not in the cache.
            CLNT data = null;
            if (!_cache.TryGetValue(driversLicence, out data))
            {
                // get the history from ICBC
                data = _icbcClient.GetDriverHistory(driversLicence);

                // ensure the presentation of the DL matches the calling system.
                data.DR1MST.LNUM = driversLicence;
                // Key not in cache, so get data.
                //cacheEntry = DateTime.Now;
                if (data != null)
                {
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time, reset time if accessed.
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    // Save data in cache.
                    _cache.Set(driversLicence, data, cacheEntryOptions);
                }
                
            }
            

            if (data != null)
            {
                Driver result = new Driver()
                {
                    Surname = data.INAM?.SURN,
                    GivenName = data.INAM?.GIV1,
                    Weight = data.WGHT,
                    Sex = data.SEX,
                    BirthDate = data.BIDT,
                    Height = data.HGHT,
                    SecurityKeyword = data.SECK,
                    City = data.ADDR?.CITY,
                    PostalCode = data.ADDR?.POST,
                    Province = data.ADDR?.PROV,
                    Country = data.ADDR?.CNTY
                };

                // handle address
                if (data.ADDR != null)
                {
                    List<string> addressComponents = new List<string>();

                    if (!string.IsNullOrEmpty(data.ADDR.APR1))
                    {
                        addressComponents.Add($"{data.ADDR.APR1}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.APR2))
                    {
                        addressComponents.Add($"{data.ADDR.APR2}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.APR3))
                    {
                        addressComponents.Add($"{data.ADDR.APR3}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.PSTN))
                    {
                        addressComponents.Add($"STN {data.ADDR.PSTN}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.SITE))
                    {
                        addressComponents.Add($"SITE {data.ADDR.SITE}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.COMP))
                    {
                        addressComponents.Add($"COMP {data.ADDR.COMP}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.RURR))
                    {
                        addressComponents.Add($"RR# {data.ADDR.RURR}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.POBX))
                    {
                        addressComponents.Add($"PO BOX {data.ADDR.POBX}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.STNO))
                    {
                        addressComponents.Add($"{data.ADDR.STNO}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.STNM))
                    {
                        addressComponents.Add($"{data.ADDR.STNM}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.STTY))
                    {
                        addressComponents.Add($"{data.ADDR.STTY}");
                    }

                    if (!string.IsNullOrEmpty(data.ADDR.STDI))
                    {
                        addressComponents.Add($"{data.ADDR.STDI}");
                    }

                    result.AddressLine1 = string.Join(" ", addressComponents.ToArray());
                }

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

                result.DriverMasterStatus = data.DR1MST.ToViewModel();

                return Json(result);
            }
            else
            {
                Serilog.Log.Logger.Error("No response received from ICBC - Network Error");
                return Json(null);

                //
                
                //StatusCode(StatusCodes.Status500InternalServerError, "No response received from ICBC - Network Error");
            }

        }

    }
}
