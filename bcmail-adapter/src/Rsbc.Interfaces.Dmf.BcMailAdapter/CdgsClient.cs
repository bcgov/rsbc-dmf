using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Rsbc.Interfaces.CdgsModels;


namespace Rsbc.Interfaces
{
    public class CdgsClient : ICdgsClient
    {
        private readonly IConfiguration Configuration;

        private HttpClient _Client;
        
        private string CdgsServiceUri { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public CdgsClient(IConfiguration configuration)
        {

            Configuration = configuration;
            _Client = new HttpClient();

            // check that we have the right settings.

            if (Configuration["CDGS_SERVICE_URI"] != null)
            {
                CdgsServiceUri = Configuration["CDGS_SERVICE_URI"];
            }

            _Client.BaseAddress = new Uri(CdgsServiceUri);
        }


        public async Task<Stream> PreviewBcMailDocument(LetterGenerationRequest request)
        {
            // Step 1 : Get the token and do communication with cdgs service 

            string token = await GetCdgsToken();

            // Step 2: Call cdogs service using paylod json data


            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            HttpContent body = new StringContent(payload, Encoding.UTF8, "application/json");

            var responseContent = await _Client.PostAsync("template/render", body);

            var responseStream = await responseContent.Content.ReadAsStreamAsync();

            return responseStream;
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
                throw;
            }

            return accessToken;

        }




    }

}