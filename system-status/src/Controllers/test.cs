using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SystemStatus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            
            return Ok("TEST");
        }

    }
}
