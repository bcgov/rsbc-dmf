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
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Grpc.Core;
using Microsoft.AspNetCore.StaticFiles;
using Swashbuckle.AspNetCore.Annotations;
using DocumentStorageClient = Pssg.DocumentStorageAdapter.DocumentStorageAdapter.DocumentStorageAdapterClient;

namespace Rsbc.Dmf.IcbcAdapter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DriverController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DmersController> _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;


        public DriverController(IConfiguration configuration, ILogger<DmersController> logger, IServiceScopeFactory serviceScopeFactory)
        {
           _configuration = configuration;
           _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }


      
    /// <summary>
    /// GET: /dmers?pageSize=50&amp;cursor=0
    /// </summary>
    /// <returns>DMER items, the requested page_size, and a nextLink for full pages.</returns>
    /// <param name="pageSize">Maximum number of records to return</param>
    /// <param name="cursor">Zero-based offset of the first record in the page</param>
    [HttpGet("{driver_licence_number}")]
    [Produces("application/json")]
    public ActionResult GetDriverMetaandDocuments([FromRoute] string driver_licence_number)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var caseManagerClient = scope.ServiceProvider.GetRequiredService<CaseManager.CaseManagerClient>();
            var driverRequest = new DriverLicenseRequest {DriverLicenseNumber = driver_licence_number};
            var driver = caseManagerClient.GetDriverDocuments(driverRequest);

            Console.WriteLine(driver);
            return Json(driver);
        }
    }
}
