using Rsbc.Dmf.IcbcAdapter.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Rsbc.Dmf.IcbcAdapter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // public API
    public class ApplicationVersionInfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ApplicationVersionInfoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Return the version of the running application
        /// </summary>
        /// <returns>The version of the running application</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetApplicationVersionInfo()
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            DateTime creationTime = System.IO.File.GetLastWriteTimeUtc(assembly.Location);
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string fileVersion = fvi.FileVersion;
            string fileDescription = fvi.FileDescription;

            ApplicationVersionInfo avi = new ApplicationVersionInfo
            {
                Environment = _configuration["ASPNETCORE_ENVIRONMENT"],
                FileCreationTime = creationTime.ToString("O"), // Use the round trip format as it includes the time zone.
                FileVersion = fileVersion,
                FileDescription = fileDescription
            };

            return new JsonResult(avi);
        }

    }
}
