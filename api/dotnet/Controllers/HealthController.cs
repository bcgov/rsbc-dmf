using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dmft.Api.Services;
using Dmft.Api.Models;

namespace Dmft.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class HealthController : ControllerBase
    {
        #region Variables
        private readonly ILogger<DmerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly MongoService _mongo;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a DmerController class.
        /// </summary>
        /// <param name="mongo"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public HealthController(MongoService mongo, ILogger<DmerController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _mongo = mongo;
        }
        #endregion

        #region Endpoints
        /// <summary>
        /// Get all the parcels for the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("check")]
        public IActionResult HealthCheck()
        {
            return Ok("Success");
        }
        #endregion
    }
}
