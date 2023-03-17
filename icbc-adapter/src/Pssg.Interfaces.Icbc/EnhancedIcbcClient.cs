using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.IcbcModels;
using System.Net.Http.Headers;
using Castle.Core.Logging;

namespace Pssg.Interfaces
{
    public class EnhancedIcbcClient: IIcbcClient
    {
        private readonly IConfiguration Configuration;
        private HttpClient _Client;
        
        private string IcbcLookupServiceUri { get; set; }
        private string IcbcLookupServiceUsername { get; set; }
        private string IcbcLookupServicePassword { get; set; }

        public EnhancedIcbcClient(IConfiguration configuration)
        {

            Configuration = configuration;
            _Client = new HttpClient();
            

            // check that we have the right settings.
            if (Configuration["ICBC_SERVICE_URI"] != null)
            {
                // ICBC configuration settings.

                IcbcLookupServiceUri = Configuration["ICBC_SERVICE_URI"];
                IcbcLookupServiceUsername = Configuration["ICBC_SERVICE_USERNAME"];
                IcbcLookupServicePassword = Configuration["ICBC_SERVICE_PASSWORD"];

                _Client.BaseAddress = new Uri(IcbcLookupServiceUri);
                
                // set the authentication

                if (! string.IsNullOrEmpty(IcbcLookupServiceUsername))
                {
                    string userPass = $"{IcbcLookupServiceUsername}:{IcbcLookupServicePassword}";
                    byte[] userPassBytes = Encoding.ASCII.GetBytes(userPass);
                    _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(userPassBytes));
                }
            }            
        }

        public string SendMedicalUpdate(IcbcMedicalUpdate item)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "medical-disposition/update");
            string payload = JsonConvert.SerializeObject(item, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = _Client.SendAsync(request).GetAwaiter().GetResult();
            string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return responseContent;
        }

        public CLNT GetDriverHistory(string dlNumber)
        {
            // Get base URL


            // do a basic HTTP request
            var request = new HttpRequestMessage(HttpMethod.Get, "tombstone/" + dlNumber);

            // Get the JSON ICBC Response
            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            var response = _Client.SendAsync(request).GetAwaiter().GetResult();

            string rawData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            IcbcModels.IcbcClient icbcClient  = null;

            try
            {
                icbcClient = JsonConvert.DeserializeObject<IcbcModels.IcbcClient>(rawData);
            }
            catch (Exception e)
            {
                icbcClient = null;
            }
            

            ClientResult result = null;
            if (icbcClient != null)
            {


                result = new ClientResult()
                {
                    CLNT = new CLNT()
                    {
                        // Client Details

                        // Add Client Number
                        SEX = icbcClient.ClientDetails?.Gender ?? string.Empty,
                        SECK = icbcClient.ClientDetails?.SecurityKeyword ?? string.Empty,
                        BIDT = icbcClient.ClientDetails?.Birthdate,
                        WGHT = icbcClient.ClientDetails?.Weight,
                        HGHT = icbcClient.ClientDetails?.Height,

                        INAM = new INAM()
                        {
                            SURN = icbcClient.ClientDetails?.Name?.Surname,
                            GIV1 = icbcClient.ClientDetails?.Name?.GivenName1,
                            GIV2 = icbcClient.ClientDetails?.Name?.GivenName2,
                            GIV3 = icbcClient.ClientDetails?.Name?.GivenName3,
                        },
                        ADDR = new ADDR()
                        {
                            BUNO = icbcClient.ClientDetails?.Address?.BuildingUnitNumber,
                            STNM = icbcClient.ClientDetails?.Address?.StreetName,
                            STNO = icbcClient.ClientDetails?.Address?.StreetNumber,
                            STDI = icbcClient.ClientDetails?.Address?.StreetDirection,
                            STTY = icbcClient.ClientDetails?.Address?.StreetType,
                            SITE = icbcClient.ClientDetails?.Address?.Site,
                            COMP = icbcClient.ClientDetails?.Address?.Comp,
                            RURR = icbcClient.ClientDetails?.Address?.RuralRoute,
                            CITY = icbcClient.ClientDetails?.Address?.City,
                            PROV = icbcClient.ClientDetails?.Address?.ProvinceOrState,
                            CNTY = icbcClient.ClientDetails?.Address?.Country,
                            POST = icbcClient.ClientDetails?.Address?.PostalCode,
                            POBX = icbcClient.ClientDetails?.Address?.PostOfficeBox,
                            APR1 = icbcClient.ClientDetails?.Address?.AddressPrefix1,
                            APR2 = icbcClient.ClientDetails?.Address?.AddressPrefix2,
                            EFDT = icbcClient.ClientDetails?.Address?.EffectiveDate
                        },

                        // Driver Details

                        DR1MST = new DR1MST()
                        {
                            LNUM = icbcClient.DriversDetails?.LicenceNumber,
                            LCLS = icbcClient.DriversDetails?.LicenceClass,
                            RRDT = icbcClient.DriversDetails?.LicenceExpiryDate,
                            MSCD = icbcClient.DriversDetails?.MasterStatusCode,

                            // Restrictions need to add discription

                            RSCD = icbcClient.DriversDetails?.Restrictions != null ? icbcClient.DriversDetails.Restrictions
                            .Select(restriction => restriction.RestrictionCode)
                            .ToList() : null,


                            // Expanded Status
                            DR1STAT = icbcClient.DriversDetails?.ExpandedStatuses != null ? icbcClient.DriversDetails.ExpandedStatuses
                            .Select(status => new DR1STAT()
                            {
                                SECT = status?.StatusSection,
                                EFDT = status?.EffectiveDate,
                                EXDS = status?.ExpandedStatus,
                                SRDT = status?.ReviewDate,
                                NECD = status?.StatusDescription,
                                NMCD = status?.MasterStatus


                            }).ToList() : null,

                            // Medicals
                            DR1MEDN = icbcClient.DriversDetails?.Medicals != null ? icbcClient.DriversDetails.Medicals
                            .Select(medicals => new DR1MEDNITEM()
                            {
                                MIDT = medicals?.IssueDate,
                                ISOF = medicals?.IssuingOffice,
                                ISOFDESC = medicals?.IssuingOfficeDescription,
                                PGN1 = medicals?.PhysiciansGuide1,
                                PGN2 = medicals?.PhysiciansGuide2,
                                MEDT = medicals?.ExamDate,
                                MDSP = medicals?.MedicalDisposition,
                                MDSPDESC = medicals?.DispositionDescription,

                            }).ToList() : null
                        }
                    }
                };
            }

            return result.CLNT;

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