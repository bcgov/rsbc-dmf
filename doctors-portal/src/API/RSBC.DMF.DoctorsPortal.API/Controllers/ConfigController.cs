using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace RSBC.DMF.DoctorsPortal.API.Controllers
{
    /// <summary>
    /// Configuration endpoint
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Configuration> Get()
        {
            var config = new Configuration
            {
                Environment = env.EnvironmentName,
                EformsConfiguration = configuration.GetSection("eforms").Get<EFormsOptions>()
            };

            return Ok(config);
        }

        /// <summary>
        /// Client configuration settings
        /// </summary>
        public class Configuration
        {
            public string Environment { get; set; }
            public EFormsOptions EformsConfiguration { get; set; }
        }

        public class EFormsOptions
        {
            public string FormServerUrl { get; set; }

            public string EmrVendorId { get; set; }

            public string FhirServerUrl { get; set; }
            public string FormsMap { get; set; }

            public EFormDetails[] Forms =>
                string.IsNullOrEmpty(FormsMap)
                ? Array.Empty<EFormDetails>()
                : JsonSerializer.Deserialize<EFormDetails[]>(FormsMap, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public class EFormDetails
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}