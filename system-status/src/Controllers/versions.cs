using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SystemStatus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionsController : ControllerBase
    {
        private readonly IConfiguration _configuration;


        public VersionsController( IConfiguration configuration)
        {            
            _configuration = configuration;
        }


        [HttpGet("{token}")]
        public async Task<IActionResult> Get(string token)
        {

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            var client = new HttpClient(handler);

            var url = _configuration[$"URL_{token}"];
            var result = await client.GetStringAsync(url);
            return Ok(result);
        }
    }
}
