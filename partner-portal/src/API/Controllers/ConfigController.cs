using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ConfigController(AppConfiguration appConfiguration) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<AppConfiguration> Config()
    {
        return new JsonResult(appConfiguration);
    }
}
