using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PhsaAdapter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReceiveController : ControllerBase
    {

        private readonly ILogger<ReceiveController> _logger;

        public ReceiveController(ILogger<ReceiveController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [BasicAuthorization]
        public IActionResult Post([FromBody] object data)
        {
            string jsonString = JsonSerializer.Serialize(data);
            _logger.LogInformation(jsonString);
            return Ok();
        }
    }
}
