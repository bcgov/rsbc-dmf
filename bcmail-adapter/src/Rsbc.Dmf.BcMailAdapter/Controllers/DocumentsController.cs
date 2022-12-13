using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.IO;
using System.Linq;
using System.Threading.Tasks;



namespace Rsbc.Dmf.BcMailAdapter.Controllers
{
    /// <summary>
    /// Controller providing data related to a Driver
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")] 
    public class DocumentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentsController> _logger;


        public DocumentsController(ILogger<DocumentsController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }



        
        /// <summary>
        /// Mail a document
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        // POST: /Documents/BcMail}
        [HttpPost("BcMail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public ActionResult BcMailDocument()  // add a model here for the payload

        {

            // this could put the HTML file and attachments in a particular location.

            return Ok("Success");
        }


    }
}
