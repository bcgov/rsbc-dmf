using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.IdentityModel.Tokens;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.HttpSys;

namespace Rsbc.Dmf.IcbcAdapter.Controllers
{

    public enum DmerQueueType
    {
        DPS_GENERAL,
        UNKNOWN_DLS
    }

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DmersController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DmersController> _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;


        public DmersController(IConfiguration configuration, ILogger<DmersController> logger, IServiceScopeFactory serviceScopeFactory)
        {
           _configuration = configuration;
           _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }


    /// <summary>
    /// GET: /dmers?queueType=QueueA&pageSize=50&cursor=0
    /// </summary>
    /// <param name="queue">Queue to pull uploaded DMER files from</param>
    /// <param name="pageSize">Maximum number of records to return</param>
    /// <param name="cursor">Zero-based offset of the first record in the page</param>
    [HttpGet("")]
    [Produces("application/json")]
    public ActionResult GetDmerBatch(
        [FromQuery, Range(1, 500)] int pageSize = 50,
        [FromQuery, Range(0, int.MaxValue)] int cursor = 0)
        {

            using var scope = _serviceScopeFactory.CreateScope();
            var documentManagerClient = scope.ServiceProvider.GetRequiredService<DocumentManager.DocumentManagerClient>();
            var dmerRecordsRequest = new GetDmerRecordsPaginatedRequest {PageSize = pageSize, Cursor = cursor }; 
            var dmerRecordsResults = documentManagerClient.GetDmerRecordsPaginated(dmerRecordsRequest);
            
            if (dmerRecordsResults.ResultStatus.Equals(ResultStatus.Fail))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, dmerRecordsResults);
            }

            return Json(dmerRecordsResults);
        }
    }
}
