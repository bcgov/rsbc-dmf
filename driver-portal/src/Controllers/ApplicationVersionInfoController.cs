using Rsbc.Dmf.DriverPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // public API
    public class ApplicationVersionInfoController : Controller
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
        [ProducesResponseType(typeof(ViewModels.ApplicationVersionInfo), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetCase")]
        public ActionResult GetApplicationVersionInfo()
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            DateTime creationTime = System.IO.File.GetLastWriteTimeUtc(assembly.Location);
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string fileVersion = fvi.FileVersion;
            string productVersion = fvi.ProductVersion;

            ApplicationVersionInfo avi = new ApplicationVersionInfo
            {
                Environment = _configuration["ASPNETCORE_ENVIRONMENT"],
                FileCreationTime = creationTime.ToString("O"), // Use the round trip format as it includes the time zone.
                FileVersion = fileVersion,
                ProductVersion = productVersion
            };

            return Json(avi);
        }

    }
}
