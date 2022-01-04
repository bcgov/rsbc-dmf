using System.Net;
using System.Net.Http;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pssg.Interfaces.Icbc.Models;

namespace Pssg.Interfaces
{
    public class IcbcClient
    {
        private readonly IConfiguration Configuration;
        private HttpClient _Client;
        
        private string IcbcLookupServiceUri { get; set; }
        private string IcbcLookupServiceUsername { get; set; }
        private string IcbcLookupServicePassword { get; set; }

        public IcbcClient(IConfiguration configuration)
        {

            Configuration = configuration;
            _Client = new HttpClient();

            // check that we have the right settings.
            if (Configuration["ICBC_LOOKUP_SERVICE_URI"] != null && Configuration["ICBC_LOOKUP_SERVICE_USERNAME"] != null &&
                Configuration["ICBC_LOOKUP_SERVICE_PASSWORD"] != null )
            {
                // ICBC configuration settings.
                IcbcLookupServiceUri = Configuration["ICBC_LOOKUP_SERVICE_URI"];
                IcbcLookupServiceUsername = Configuration["ICBC_LOOKUP_SERVICE_USERNAME"];
                IcbcLookupServicePassword = Configuration["ICBC_LOOKUP_SERVICE_PASSWORD"];                
            }            
        }
        
        public CLNT GetDriverHistory(string dlNumber)
        {
            string serviceUrl = IcbcLookupServiceUri
                    + "?sUserid=" + IcbcLookupServiceUsername
                    + "&sPassword=" + IcbcLookupServicePassword
                    + "&sLicenceNumber=" + dlNumber
                    + "&sNameCode=PAK";
            // do a basic HTTP request
            var request = new HttpRequestMessage(HttpMethod.Get, serviceUrl);
            var response = _Client.SendAsync(request).GetAwaiter().GetResult();
            string rawXml = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rawXml);

            // convert to json

            string jsonText = JsonConvert.SerializeXmlNode(xmlDoc.GetElementsByTagName("CLNT")[0]);

            // and to the model.

            var result = JsonConvert.DeserializeObject<ClientResult>(jsonText);

            return result.CLNT;
        }

        private class ClientResult
        {
            public CLNT CLNT { get; set;}
        }
    }

}