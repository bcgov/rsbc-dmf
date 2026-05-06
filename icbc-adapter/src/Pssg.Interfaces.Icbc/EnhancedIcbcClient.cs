using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.Icbc.Services;
using Pssg.Interfaces.IcbcModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace Pssg.Interfaces
{
    public class EnhancedIcbcClient: IIcbcClient
    {
        private readonly IConfiguration Configuration;
        private HttpClient _Client;
        private readonly IOAuth2TokenService _tokenService;

        // Legacy authentication properties
        private string IcbcLookupServiceUri { get; set; }
        private string IcbcLookupServiceUsername { get; set; }
        private string IcbcLookupServicePassword { get; set; }

        // OAuth2 authentication properties
        private string IcbcOAuthServiceUri { get; set; }


        // Feature flag to determine which authentication method to use
        private bool UseOAuth2Authentication { get; set; }


        /// <summary>
        /// Enhanced Icbc Client - Constructor for OAuth2 authentication
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="tokenService"></param>
        public EnhancedIcbcClient(IConfiguration configuration, IOAuth2TokenService tokenService)
        {
            Configuration = configuration;
            _tokenService = tokenService;
            _Client = new HttpClient();

            InitializeAuthentication();
        }

        /// <summary>
        /// Enhanced Icbc Client - Constructor for legacy authentication or when OAuth2TokenService is not available
        /// </summary>
        /// <param name="configuration"></param>
        public EnhancedIcbcClient(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenService = null;
            _Client = new HttpClient();

            InitializeAuthentication();
        }

        /// <summary>
        /// Enhanced Icbc Client
        /// </summary>
        private void InitializeAuthentication()
        {
            // Check feature flag and defaulting to OAuth2
            UseOAuth2Authentication = Configuration.GetValue<bool>("ICBC_USE_OAUTH2", true);

            // If OAuth2 is enabled but token service is not available, fall back to legacy
            if (UseOAuth2Authentication && _tokenService == null)
            {
                UseOAuth2Authentication = false;
                Serilog.Log.Warning("OAuth2 was requested but IOAuth2TokenService is not available. Falling back to legacy authentication.");
            }

            if (UseOAuth2Authentication)
            {
                // OAuth2 configuration settings
                if (Configuration["ICBC_SERVICE_URI_OAUTH"] != null)
                {
                    IcbcOAuthServiceUri = Configuration["ICBC_SERVICE_URI_OAUTH"];
                    _Client.BaseAddress = new Uri(IcbcOAuthServiceUri);
                }
                else
                {
                    throw new InvalidOperationException("ICBC_SERVICE_URI_OAUTH is required when OAuth2 authentication is enabled");
                }
            }
            else
            {
                // Legacy authentication configuration
                if (Configuration["ICBC_SERVICE_URI"] != null)
                {
                    IcbcLookupServiceUri = Configuration["ICBC_SERVICE_URI"];
                    IcbcLookupServiceUsername = Configuration["ICBC_SERVICE_USERNAME"];
                    IcbcLookupServicePassword = Configuration["ICBC_SERVICE_PASSWORD"];

                    _Client.BaseAddress = new Uri(IcbcLookupServiceUri);

                    // Set Basic authentication for legacy mode
                    if (!string.IsNullOrEmpty(IcbcLookupServiceUsername))
                    {
                        string userPass = $"{IcbcLookupServiceUsername}:{IcbcLookupServicePassword}";
                        byte[] userPassBytes = Encoding.ASCII.GetBytes(userPass);
                        _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(userPassBytes));
                    }

                    if (string.IsNullOrEmpty(IcbcLookupServiceUsername) || string.IsNullOrEmpty(IcbcLookupServicePassword))
                    {
                        throw new InvalidOperationException("ICBC_SERVICE_USERNAME and ICBC_SERVICE_PASSWORD are required when legacy authentication is enabled");
                    }
                }
                else
                {
                    throw new InvalidOperationException("ICBC_SERVICE_URI is required when legacy authentication is enabled");
                }
            }
        }

        public string NormalizeDl(string dlNumber, IConfiguration configuration)
        {
            string result = dlNumber;
            if (!string.IsNullOrEmpty(configuration["ICBC_DL_7_TO_8"]) && dlNumber != null)
            {
                if (dlNumber.Length == 7)
                {
                    result = "0" + dlNumber;
                }
            }
            return result;
        }


        /// <summary>
        /// Send Medical Update with OAuth2 and Legacy authentication support
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string SendMedicalUpdate(IcbcMedicalUpdate item)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "medical-disposition/update");
            string payload = JsonConvert.SerializeObject(item, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Set authentication header based on the authentication method
            if (UseOAuth2Authentication)
            {
                if (_tokenService == null)
                {
                    throw new InvalidOperationException("OAuth2TokenService is required for OAuth2 authentication");
                }

                // Get OAuth2 token and set Bearer authentication
                var accessToken = _tokenService.GetAccessTokenAsync().GetAwaiter().GetResult();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                // Login Header
                request.Headers.Add("LoginUserId","rsbc-icbc-adapter");

            }
            // For legacy authentication, Basic auth is already set in _Client.DefaultRequestHeaders.Authorization

            var response = _Client.SendAsync(request).GetAwaiter().GetResult();
            string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return responseContent;
        }


        /// <summary>
        /// Get Driver History with OAuth2 and Legacy authentication support
        /// </summary>
        /// <param name="dlNumber"></param>
        /// <returns></returns>
        public CLNT GetDriverHistory(string dlNumber)
        {
            if (dlNumber != null && dlNumber == "00000000")
            {
                return new CLNT();
            }

            if (UseOAuth2Authentication)
            {
                return GetDriverHistoryWithOAuth2(dlNumber);
            }
            else
            {
                return GetDriverHistoryWithLegacyAuth(dlNumber);
            }
        }

        private CLNT GetDriverHistoryWithOAuth2(string dlNumber)
        {
            if (_tokenService == null)
            {
                throw new InvalidOperationException("OAuth2TokenService is required for OAuth2 authentication");
            }

            // Get OAuth2 token
            var accessToken = _tokenService.GetAccessTokenAsync().GetAwaiter().GetResult();

            // Create HTTP request with Bearer token
            var request = new HttpRequestMessage(HttpMethod.Get, "tombstone/" + dlNumber);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Headers.Add("LoginUserId", "rsbc-icbc-adapter");

            var response = _Client.SendAsync(request).GetAwaiter().GetResult();
            string rawData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return ProcessDriverHistoryResponse(rawData, dlNumber);
        }

        private CLNT GetDriverHistoryWithLegacyAuth(string dlNumber)
        {
            // Basic HTTP request using existing authentication (already set in HttpClient headers)
            var request = new HttpRequestMessage(HttpMethod.Get, "tombstone/" + dlNumber);
            request.Headers.TryAddWithoutValidation("Accept", "application/json");

            var response = _Client.SendAsync(request).GetAwaiter().GetResult();
            string rawData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return ProcessDriverHistoryResponse(rawData, dlNumber);
        }

        private CLNT ProcessDriverHistoryResponse(string rawData, string dlNumber)
        {
            IcbcModels.IcbcClient icbcClient = null;

            try
            {
                icbcClient = JsonConvert.DeserializeObject<IcbcModels.IcbcClient>(rawData);
            }
            catch (Exception e)
            {
                Serilog.Log.Error($"No response received from ICBC - parsing error on {rawData}");
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

                            // Restrictions need to add description
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
                                .Select(medical => new DR1MEDNITEM()
                                {
                                    MIDT = medical?.IssueDate,
                                    ISOF = medical?.IssuingOffice,
                                    ISOFDESC = medical?.IssuingOfficeDescription,
                                    PGN1 = medical?.PhysiciansGuide1,
                                    PGN2 = medical?.PhysiciansGuide2,
                                    MEDT = medical?.ExamDate,
                                    MDSP = medical?.MedicalDisposition,
                                    MDSPDESC = medical?.DispositionDescription,
                                    DocumentNumber = medical?.DocumentNumber,
                                    MedicalType = medical?.MedicalType,
                                    MedicalLevel = medical?.MedicalLevel
                                }).ToList() : null
                        }
                    }
                };
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