using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

         public ConfigController(ILogger<ConfigController> logger, IHostEnvironment env)
        {
            _logger = logger;
            this.env = env;
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
                Environment = env.EnvironmentName
            };

            return Ok(config);
        }

        /// <summary>
        /// Client configuration settings
        /// </summary>
        public class Configuration
        {
            public string Environment { get; set; }
        }
    }
}
