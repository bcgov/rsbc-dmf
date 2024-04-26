using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

[Route("api/[controller]")]
[ApiController]
public class ApplicationVersionInfoController : ControllerBase
{
    private readonly AppConfig _config;

    public ApplicationVersionInfoController(AppConfig config)
    {
        _config = config;
    }

    [HttpGet]
    public ActionResult<ApplicationVersionInfo> GetApplicationVersionInfo()
    {
        var assembly = GetType().GetTypeInfo().Assembly;
        var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        string fileVersion = fvi.FileVersion;

        var avi = new ApplicationVersionInfo
        {
            Environment = _config.EnvironmentName,
            FileVersion = fileVersion,
            AppConfig = _config,
        };

        return new JsonResult(avi);
    }
}
