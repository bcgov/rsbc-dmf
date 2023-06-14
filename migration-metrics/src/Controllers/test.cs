using Microsoft.AspNetCore.Mvc;

namespace MigrationMetrics.Controllers
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
