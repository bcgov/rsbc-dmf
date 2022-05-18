using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.LegacyAdapter.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CommentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CommentsController> _logger;

        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        
        public CommentsController(ILogger<CommentsController> logger, IConfiguration configuration, CaseManager.CaseManagerClient cmsAdapterClient)
        {
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;
            _logger = logger;
        }

        /// <summary>
        /// DoesCaseExist
        /// </summary>
        /// <param name="licenseNumber"></param>
        /// <param name="surcode"></param>
        /// <returns>True if the case exists</returns>
        // GET: /Comments/Exist
        [HttpGet("Exist")]
        public ActionResult DoesCaseExist(string licenseNumber, string surcode)
        {
            bool result = false;
            // get the case                                                
            return Json (result);
        }

        /// <summary>
        /// Get Comments for a case
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        // GET: /Comments/Exist
        [HttpGet("{commentId}")]
        [ProducesResponseType(typeof(List<ViewModels.Comment>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetComment")]
        public ActionResult GetComment([FromRoute] string commentId)
        
        {
            // get the comment
            ViewModels.Comment result = new ViewModels.Comment();
                        
            return Json(result);
        }

        

        


    }
}
