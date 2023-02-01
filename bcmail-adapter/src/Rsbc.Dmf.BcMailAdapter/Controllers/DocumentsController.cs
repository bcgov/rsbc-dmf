using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Rsbc.Dmf.BcMailAdapter.ViewModels;
using Newtonsoft.Json;
using IdentityModel.Client;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Reflection.Metadata.Ecma335;
using System.Net.Http.Headers;

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
        private readonly IConfiguration Configuration;
        private readonly ILogger<DocumentsController> Logger;


        /// <summary>
        ///  Documents Controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public DocumentsController(ILogger<DocumentsController> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            Logger = logger;
        }




        /// <summary>
        /// Mail a document
        /// </summary>
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

        /// <summary>
        /// Bc Mail Document Preview
        /// </summary>
        /// <param name="bcmail"></param>
        /// <returns></returns>

        // POST: /Documents/BcMailPreview}
        [HttpPost("BcMailPreview")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> BcMailDocumentPreview([FromBody] ViewModels.BcMail bcmail)

        {
            //Step 1: Read JSon payload data

            // Step 2 " get the token and do communication with cdgs service 

            string token = await GetCdgsToken();

            // Step 3: call cdogs service using paylod json data
            List<BcMail> bcmaildocument = new List<BcMail>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var response = await httpClient.GetAsync(Configuration["CDGS_SERVICE"]))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    // bcmaildocument = JsonConvert.DeserializeObject<List<BcMail>>(apiResponse);
                }
            }

            // step 4: preview PDF 

            // step 5 : send the response to dynamics




            return Ok(bcmail);
        }

        /// <summary>
        /// Get CDGS Token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetCdgsToken()
        {
            string accessToken = null;

            try
            {
                var client = new HttpClient();
                var clientCredentials = new ClientCredentialsTokenRequest
                {

                    Address = Configuration["CDGS_TOKEN_ENDPOINT"],
                    ClientId = Configuration["CDGS_OAUTH_CLIENT_ID"],
                    ClientSecret = Configuration["CDGS_OAUTH_CLIENT_SECRET"],

                };

                var tokenResponse = await client.RequestClientCredentialsTokenAsync(clientCredentials);
                accessToken = tokenResponse.AccessToken;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Token is not found");
            }

            return accessToken;

        }
    }
}
