using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Pssg.Dmf.LegacyAdapter.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CommentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CommentsController> _logger;


        public CommentsController(ILogger<CommentsController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;

        }

        /// <summary>
        /// DoesCaseExist
        /// </summary>
        /// <param name="driversLicense"></param>
        /// <param name="surcode"></param>
        /// <returns>True if the case exists</returns>
        // GET: /Comments/Exist
        [HttpGet("Exist")]
        public ActionResult DoesCaseExist(string driversLicense, string surcode)
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
