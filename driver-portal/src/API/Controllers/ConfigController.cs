using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    /// <summary>
    /// Configuration endpoint
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ConfigController : Controller
    {
        private readonly ILogger<ConfigController> _logger;
        private readonly IHostEnvironment env;
        private readonly IConfiguration configuration;

        public ConfigController(ILogger<ConfigController> logger, IHostEnvironment env, IConfiguration configuration)
        {
            _logger = logger;
            this.env = env;
            this.configuration = configuration;
        }

        /// <summary>
        /// Get the client configuration for this environment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Configuration), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult<Configuration> Get()
        {
            var config = new Configuration
            {
                Environment = env.EnvironmentName,
                OidcConfiguration = configuration.GetSection("auth:oidc").Get<OidcOptions>()
            };

            return Ok(config);
        }

        /// <summary>
        /// Client configuration settings
        /// </summary>
        public class Configuration
        {
            public string Environment { get; set; }
            public OidcOptions OidcConfiguration { get; set; }
        }

        public class OidcOptions
        {
            public string Issuer { get; set; }
            public string Scope { get; set; }
            public string ClientId { get; set; }
        }
    }
}