using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
            ClientResult result;
            try
            {
                result = JsonConvert.DeserializeObject<ClientResult>(jsonText);
            }
            catch (Exception)
            {
                try
                {
                    var tempResult = JsonConvert.DeserializeObject<ClientResult2>(jsonText);
                    result = new ClientResult()
                    {
                        // transfer items over
                        CLNT = new CLNT()
                        {
                            ADDR = tempResult.CLNT.ADDR,
                            BIDT = tempResult.CLNT.BIDT,
                            DR1MST = new DR1MST()
                            {
                                DR1MEDN = tempResult.CLNT.DR1MST?.DR1MEDN,
                                DR1STAT = tempResult.CLNT.DR1MST?.DR1STAT,
                                LCLS = tempResult.CLNT.DR1MST?.LCLS,
                                LNUM = tempResult.CLNT.DR1MST?.LNUM,
                                MSCD = tempResult.CLNT.DR1MST?.MSCD,
                                RRDT = tempResult.CLNT.DR1MST?.RRDT,
                            },
                            HGHT = tempResult.CLNT.HGHT,
                            INAM = tempResult.CLNT.INAM,
                            SECK = tempResult.CLNT.SECK,
                            SEX = tempResult.CLNT.SEX,
                            WGHT = tempResult.CLNT.WGHT
                        }
                    };

                    // convert RSCD to list.
                    if (tempResult.CLNT.DR1MST?.RSCD != null)
                    {
                        result.CLNT.DR1MST.RSCD = new System.Collections.Generic.List<int>();
                        result.CLNT.DR1MST.RSCD.Add((tempResult.CLNT.DR1MST.RSCD).Value);
                    }

                }
                catch (Exception)
                {
                    try
                    {
                        var tempResult = JsonConvert.DeserializeObject<ClientResult3>(jsonText);
                        result = new ClientResult()
                        {
                            // transfer items over
                            CLNT = new CLNT()
                            {
                                ADDR = tempResult.CLNT.ADDR,
                                BIDT = tempResult.CLNT.BIDT,
                                DR1MST = new DR1MST()
                                {
                                    DR1MEDN = tempResult.CLNT.DR1MST?.DR1MEDN,
                                    RSCD = tempResult.CLNT.DR1MST?.RSCD,
                                    LCLS = tempResult.CLNT.DR1MST?.LCLS,
                                    LNUM = tempResult.CLNT.DR1MST?.LNUM,
                                    MSCD = tempResult.CLNT.DR1MST?.MSCD,
                                    RRDT = tempResult.CLNT.DR1MST?.RRDT,
                                },
                                HGHT = tempResult.CLNT.HGHT,
                                INAM = tempResult.CLNT.INAM,
                                SECK = tempResult.CLNT.SECK,
                                SEX = tempResult.CLNT.SEX,
                                WGHT = tempResult.CLNT.WGHT
                            }
                        };
                        // convert DR1STAT to list.
                        if (tempResult.CLNT.DR1MST?.DR1STAT != null)
                        {
                            result.CLNT.DR1MST.DR1STAT = new List<DR1STAT>();
                            result.CLNT.DR1MST.DR1STAT.Add(tempResult.CLNT.DR1MST.DR1STAT);
                        }
                    }
                    catch (Exception)
                    {
                        var tempResult = JsonConvert.DeserializeObject<ClientResult4>(jsonText);
                        result = new ClientResult()
                        {
                            // transfer items over
                            CLNT = new CLNT()
                            {
                                ADDR = tempResult.CLNT.ADDR,
                                BIDT = tempResult.CLNT.BIDT,
                                DR1MST = new DR1MST()
                                {
                                    DR1MEDN = tempResult.CLNT.DR1MST?.DR1MEDN,
                                    LCLS = tempResult.CLNT.DR1MST?.LCLS,
                                    LNUM = tempResult.CLNT.DR1MST?.LNUM,
                                    MSCD = tempResult.CLNT.DR1MST?.MSCD,
                                    RRDT = tempResult.CLNT.DR1MST?.RRDT,
                                },
                                HGHT = tempResult.CLNT.HGHT,
                                INAM = tempResult.CLNT.INAM,
                                SECK = tempResult.CLNT.SECK,
                                SEX = tempResult.CLNT.SEX,
                                WGHT = tempResult.CLNT.WGHT
                            }
                        };
                        // convert DR1STAT to list.
                        if (tempResult.CLNT.DR1MST?.DR1STAT != null)
                        {
                            result.CLNT.DR1MST.DR1STAT = new List<DR1STAT>();
                            foreach (var stat in tempResult.CLNT.DR1MST?.DR1STAT)
                            {
                                result.CLNT.DR1MST.DR1STAT.Add(stat);
                            }
                            
                        }
                        if (tempResult.CLNT.DR1MST?.RSCD != null)
                        {
                            result.CLNT.DR1MST.RSCD = new List<int>();
                            result.CLNT.DR1MST.RSCD.Add(tempResult.CLNT.DR1MST.RSCD);
                        }

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