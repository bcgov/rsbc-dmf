using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;

[Route("api/[controller]")]
[ApiController]
public class ApplicationVersionInfoController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ApplicationVersionInfoController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult<ApplicationVersionInfo> GetApplicationVersionInfo()
    {
        var assembly = GetType().GetTypeInfo().Assembly;
        var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        string fileVersion = fvi.FileVersion;

        var avi = new ApplicationVersionInfo
        {
            FileVersion = fileVersion,
        };

        return new JsonResult(avi);
    }
}
