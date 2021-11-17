using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RSBC.DMF.DoctorsPortal.API.ViewModels;
using System;
using System.Diagnostics;
using System.Reflection;

namespace RSBC.DMF.DoctorsPortal.API.Controllers
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
            string productVersion = fvi.ProductVersion;
            string fileVersion = fvi.FileVersion;
                       
            // The OPENSHIFT_ environment variables are only set during an OpenShift build.

            string sourceCommit = _configuration["OPENSHIFT_BUILD_COMMIT"];
            string sourceRepository = _configuration["OPENSHIFT_BUILD_SOURCE"];
            string sourceReference = _configuration["OPENSHIFT_BUILD_REFERENCE"];

            // Allow version info to be obtained from a Github action build
            var strings = productVersion.Split("!");
            if (strings.Length == 5 )
            {
                sourceRepository = strings[0] + "/" + strings[1];
                sourceReference = strings[2];
                sourceCommit = strings[3];
                fileVersion += "." + strings[4];
            }

            ApplicationVersionInfo avi = new ApplicationVersionInfo
            {
                BaseUri = _configuration["BASE_URI"],
                BasePath = _configuration["BASE_PATH"],
                Environment = _configuration["ASPNETCORE_ENVIRONMENT"],
                SourceCommit = sourceCommit,
                SourceRepository = sourceRepository,
                SourceReference = sourceReference,
                FileCreationTime = creationTime.ToString("O"), // Use the round trip format as it includes the time zone.
                FileVersion = fileVersion
            };

            return new JsonResult(avi);
        }

    }

}
