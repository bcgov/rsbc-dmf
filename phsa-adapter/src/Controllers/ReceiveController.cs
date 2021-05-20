using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Post()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string body = await reader.ReadToEndAsync();
                _logger.LogInformation(body);

            }

            return Ok();
        }
    }
}