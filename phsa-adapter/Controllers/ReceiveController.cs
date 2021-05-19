using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Rsbc.Dmf.PhsaAdapter.Controllers
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
            var jsonString = JsonSerializer.Serialize(data);
            _logger.LogInformation(jsonString);
            return Ok();
        }
    }
}