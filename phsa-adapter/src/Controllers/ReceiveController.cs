using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PhsaAdapter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize("BasicAuthentication")]
    public class ReceiveController : ControllerBase
    {
        private readonly ILogger<ReceiveController> _logger;

        public ReceiveController(ILogger<ReceiveController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string body = await reader.ReadToEndAsync();

                // TODO - centralize the receive bundle code.

                _logger.LogInformation(body);
            }

            return Ok();
        }
    }
}