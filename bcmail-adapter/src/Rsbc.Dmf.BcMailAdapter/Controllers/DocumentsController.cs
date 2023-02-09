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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Newtonsoft.Json.Serialization;
using Serilog.Core;
using System.Net;

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
        private readonly HttpClient _Client;

        private string CdgsServiceUri { get; set; }


        /// <summary>
        ///  Documents Controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public DocumentsController(ILogger<DocumentsController> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            Logger = logger;

            //Set up the client
            _Client = new HttpClient();

            CdgsServiceUri = Configuration["CDGS_SERVICE_URI"];

            _Client.BaseAddress = new Uri(CdgsServiceUri);

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
        public async Task<FileResult> BcMailDocumentPreview([FromBody] ViewModels.BcMail bcmail)

        {
            //Step 1: Read JSon payload data

            // Step 2 " get the token and do communication with cdgs service 

            string token = await GetCdgsToken();

            // Step 3: call cdogs service using paylod json data

            using (var httpClient = new HttpClient())

            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                httpClient.BaseAddress = new Uri(CdgsServiceUri);

                LetterGenerationRequestModel letterGenerationRequest = new LetterGenerationRequestModel
                {
                    Data = new Data
                    {
                        /*FirstName = "Test1",
                        LastName = "LAstNAme",
                        Title = "Hello"*/
                    },
                    /* Formatters = "",*/
                    Options = new Options
                    {
                        ConvertTo = "pdf",
                        Overwrite = true,
                        ReportName = bcmail.Attachments[0].FileName
                    },
                    Template = new Template()
                    {
                        Content = bcmail.Attachments[0].Body,
                        EncodingType = "base64",
                        FileType = bcmail.Attachments[0].ContentType
                    }

                };

                var payload = JsonConvert.SerializeObject(letterGenerationRequest, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                HttpContent body = new StringContent(payload, Encoding.UTF8, "application/json");

                var responseContent = await httpClient.PostAsync("template/render", body);

                var responseStream = await responseContent.Content.ReadAsStreamAsync();

                return File(responseStream, "application/pdf", fileDownloadName: bcmail.Attachments[0].FileName);


            }
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
