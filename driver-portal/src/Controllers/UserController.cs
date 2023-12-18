using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // public API
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Return the version of the running application
        /// </summary>
        /// <returns>The version of the running application</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetCurrentUser()
        {
            return Ok();
        }

    }
}
