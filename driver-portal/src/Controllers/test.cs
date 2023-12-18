using Microsoft.AspNetCore.Mvc;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            
            return Ok("TEST");
        }
    }
}
