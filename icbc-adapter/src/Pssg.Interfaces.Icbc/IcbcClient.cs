using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pssg.Interfaces.Icbc.Models;

namespace Pssg.Interfaces
{
    public class IcbcClient: IIcbcClient
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
            if (Configuration["ICBC_LOOKUP_SERVICE_URI"] != null)
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
            ClientResult result = null;
            if (jsonText != null && jsonText != "null")
            {

                var tempResult = JsonConvert.DeserializeObject<dynamic>(jsonText);

                result = new ClientResult()
                {
                    // transfer items over
                    CLNT = new CLNT()
                    {
                        ADDR = tempResult.CLNT.ADDR == null ? null : tempResult.CLNT.ADDR.ToObject<ADDR>(),
                        BIDT = tempResult.CLNT.BIDT == null ? null : tempResult.CLNT.BIDT,
                        DR1MST = new DR1MST()
                        {                           
                            LCLS = tempResult.CLNT.DR1MST?.LCLS,
                            LNUM = tempResult.CLNT.DR1MST?.LNUM,
                            MSCD = tempResult.CLNT.DR1MST?.MSCD,
                            RRDT = tempResult.CLNT.DR1MST?.RRDT
                        },
                        HGHT = tempResult.CLNT.HGHT,
                        INAM = tempResult.CLNT.INAM == null ? null : tempResult.CLNT.INAM.ToObject<INAM>(),
                        SECK = tempResult.CLNT.SECK,
                        SEX = tempResult.CLNT.SEX,
                        WGHT = tempResult.CLNT.WGHT
                    }
                };

                // normalize dynamic elements

                if (tempResult.CLNT.DR1MST?.DR1MEDN != null)
                {
                    
                    // determine if it is a list.

                    var x = tempResult.CLNT.DR1MST.DR1MEDN;
                    var t = x.GetType();
                    if (t == typeof(JObject))
                    {
                        result.CLNT.DR1MST.DR1MEDN = new List<DR1MEDNITEM>();
                        var c = x.ToObject<DR1MEDNITEM>();
                        result.CLNT.DR1MST.DR1MEDN.Add(c);
                    }
                    else
                    {
                        var c = x.ToObject<List<DR1MEDNITEM>>();
                        result.CLNT.DR1MST.DR1MEDN = c; 
                    }

                }


                if (tempResult.CLNT.DR1MST?.DR1STAT != null)
                {
                    
                    // determine if it is an object or list.

                    var x = tempResult.CLNT.DR1MST.DR1STAT;
                    var t = x.GetType();
                    if (t == typeof(JObject))
                    {
                        var c = x.ToObject<DR1STAT>();
                        result.CLNT.DR1MST.DR1STAT = new List<DR1STAT>();
                        result.CLNT.DR1MST.DR1STAT.Add(c);                       
                    }
                    else
                    {
                        var c = x.ToObject<List<DR1STAT>>();
                        result.CLNT.DR1MST.DR1STAT = c;                       
                    }
                    
                }


                // convert RSCD to list.
                if (tempResult.CLNT.DR1MST?.RSCD != null)
                {
                    var x = tempResult.CLNT.DR1MST.RSCD;
                    var t = x.GetType();
                    if (t == typeof(JValue))
                    {
                        result.CLNT.DR1MST.RSCD = new System.Collections.Generic.List<int>();
                        result.CLNT.DR1MST.RSCD.Add(x.ToObject<int>());
                    }
                    else
                    {
                        result.CLNT.DR1MST.RSCD = x.ToObject<List<int>>();                       
                    }
                }

            }

            return result?.CLNT;
        }

        private class ClientResult
        {
            public CLNT CLNT { get; set;}
        }

        private class ClientResult2
        {
            public CLNT2 CLNT { get; set; }
        }

        private class ClientResult3
        {
            public CLNT3 CLNT { get; set; }
        }

        private class ClientResult4
        {
            public CLNT4 CLNT { get; set; }
        }
    }

}