using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.Dmf.IcbcAdapter.Client;
using Rsbc.Dmf.IcbcAdapter;
using Rsbc.Dmf.PartnerPortal.ViewModels;
using System.Net;

[Route("api/[controller]")]
[ApiController]
public class DriverController : Controller
{
    private readonly ICachedIcbcAdapterClient _icbcAdapterClient;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public DriverController(ICachedIcbcAdapterClient icbcAdapterClient, ILogger logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet("info/{driverLicenceNumber}")]
    [ProducesResponseType(typeof(IEnumerable<Document>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ActionName(nameof(GetHistory))]
    public async Task<ActionResult<DriverInfoReply>> GetHistory([FromRoute]string driverLicenceNumber)
    {
        try
        {
            var request = new DriverInfoRequest();
            request.DriverLicence = driverLicenceNumber;
            var reply = await _icbcAdapterClient.GetDriverInfoAsync(request);

            return Json(reply);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetHistory)} failed.");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
