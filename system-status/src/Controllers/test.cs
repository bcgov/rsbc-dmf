using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SystemStatus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            
            return Ok("TEST");
        }

        [HttpPost]
        public IActionResult Post([FromBody] string data) {
            
            // log to tmp
            string fname = "/tmp/" + DateTime.Now.Ticks.ToString() + ".txt";
            System.IO.File.WriteAllText(fname, data);

            return Ok();

            }
    }
}
