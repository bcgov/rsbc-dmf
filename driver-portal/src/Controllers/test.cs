using Microsoft.AspNetCore.Mvc;

namespace RSBC.DMF.DriverPortal.API.Controllers
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
