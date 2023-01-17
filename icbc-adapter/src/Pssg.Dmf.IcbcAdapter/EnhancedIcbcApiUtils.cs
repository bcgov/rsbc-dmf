using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using Serilog;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using System.IO;
using Pssg.SharedUtils;
using FileHelpers;
using Rsbc.Dmf.CaseManagement.Service;
using System.Linq;
using Rsbc.Dmf.IcbcAdapter.ViewModels;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http;
using Newtonsoft.Json;
using Google.Protobuf.WellKnownTypes;
using Pssg.Interfaces;
using Newtonsoft.Json.Serialization;
using Pssg.Interfaces.Icbc.Models;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;
using Rsbc.Dmf.IcbcAdapter.IcbcModels;
using IcbcClient = Rsbc.Dmf.IcbcAdapter.IcbcModels.IcbcClient;
using Grpc.Core;

namespace Rsbc.Dmf.IcbcAdapter
{
    public class EnhancedIcbcApiUtils
    {
        
        private IConfiguration _configuration { get; }
        private readonly CaseManager.CaseManagerClient _caseManagerClient;
        private readonly IIcbcClient _icbcClient;
       

        public EnhancedIcbcApiUtils(IConfiguration configuration, CaseManager.CaseManagerClient caseManagerClient, IIcbcClient icbcClient)
        {
            _configuration = configuration;
            _caseManagerClient = caseManagerClient;
            _icbcClient = icbcClient;
 
        }



        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task SendMedicalUpdates(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting SendMedicalUpdates");

            var unsentItems = _caseManagerClient.GetUnsentMedicalUpdates(new CaseManagement.Service.EmptyRequest());

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_configuration["ICBC_API_URI"]);

            foreach (var unsentItem in unsentItems.Items)
            {
                var item = GetMedicalUpdateData(unsentItem);

                if (item != null)
                {
                    
                    var request = new HttpRequestMessage(HttpMethod.Put, "medical-disposition/update");
                    string payload = JsonConvert.SerializeObject(item, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                    LogStatement(hangfireContext, $"{unsentItem.CaseId} {unsentItem.Driver?.DriverLicenseNumber} - sending update {payload}");
                    request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                    var response = client.SendAsync(request).GetAwaiter().GetResult();
                    string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        LogStatement(hangfireContext, $"HTTP Status was OK {unsentItem.CaseId} {unsentItem.Driver?.DriverLicenseNumber} {response.Content.ReadAsStringAsync().GetAwaiter().GetResult()} - marking as sent.");                                                                       

                        if (responseContent.Contains("SUCCESS"))
                        {
                            // mark it as sent
                            MarkMedicalUpdateSent(hangfireContext, unsentItem.CaseId);

                        }
                        else
                        {
                            var bringForwardRequest = new BringForwardRequest
                            {
                                CaseId = unsentItem.CaseId,
                                Subject = "ICBC Error",
                                Description = responseContent,
                                Assignee = string.Empty,
                                Priority = BringForwardPriority.Normal
                            
                            };
                            
                            _caseManagerClient.CreateBringForward(bringForwardRequest);

                            // Mark ICBC error 

                            var icbcError = new IcbcErrorRequest
                            {
                                ErrorMessage = "ICBC Error"
                            };

                            _caseManagerClient.MarkMedicalUpdateError(icbcError);

                            LogStatement(hangfireContext, $"ICBC ERROR {responseContent}");
                        }
                        
                    }
                    else
                    {
                        LogStatement(hangfireContext, $"ICBC ERROR {response.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
                    }
                }
                else
                {
                    LogStatement(hangfireContext, $"Null received from GetMedicalUpdateData for {unsentItem.CaseId} {unsentItem.Driver?.DriverLicenseNumber}");
                }

            }

            LogStatement(hangfireContext, "End of SendMedicalUpdates.");
        }

        /// <summary>
        ///  Mark Medical Update Sent
        /// </summary>
        /// <param name="hangfireContext"></param>
        /// <param name="caseId"></param>
        private void MarkMedicalUpdateSent (PerformContext hangfireContext, string caseId)
        {            
            var idListRequest = new IdListRequest();
            idListRequest.IdList.Add(caseId);
            var result = _caseManagerClient.MarkMedicalUpdatesSent(idListRequest);

            LogStatement(hangfireContext, $"Mark Medical Update Sent {caseId} status is  {result.ResultStatus} {result.ErrorDetail}");
        }

        /// <summary>
        /// Get Medical Update Data
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IcbcMedicalUpdate GetMedicalUpdateData (DmerCase item)
        {

            // Start by getting the current status for the given driver.  If the medical disposition matches, do not proceed.
                
            if (item.Driver != null)
            {
                string licenseNumber = item.Driver.DriverLicenseNumber;
                try
                {
                    var driver = _icbcClient.GetDriverHistory(licenseNumber);
                    if (driver != null && driver.INAM?.SURN != null)
                    {
                        var newUpdate = new IcbcMedicalUpdate()
                        {
                            DlNumber = licenseNumber,
                            LastName = driver.INAM.SURN,
                        };

                        var firstDecision = item.Decisions.OrderByDescending(x => x.CreatedOn).FirstOrDefault();

                        if (firstDecision != null)
                        {
                            if (firstDecision.Outcome == DecisionItem.Types.DecisionOutcomeOptions.FitToDrive)
                            {
                                newUpdate.MedicalDisposition = "P";
                            }
                            else
                            {
                                newUpdate.MedicalDisposition = "J";
                            }
                        }
                        else
                        {
                            newUpdate.MedicalDisposition = "J";
                        }

                        // get most recent Medical Issue Date from the driver.

                        DateTimeOffset adjustedDate = GetMedicalIssueDate(driver); // DateUtility.FormatDateOffsetPacific(GetMedicalIssueDate(driver)).Value;

                        newUpdate.MedicalIssueDate = adjustedDate;

                        return newUpdate;
                    }
                    else
                    {
                        Log.Logger.Error("Error getting driver from ICBC.");
                    }
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "Error getting driver from ICBC.");
                }

                
            }
            else
            {
                Log.Logger.Information($"Case {item.CaseId} {item.Title} has no Driver..");
            }
             
            return null;
        }

        /// <summary>
        /// Get Medical IssueDate
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        private DateTime GetMedicalIssueDate(CLNT driver)
        {
            DateTime result = DateTime.MinValue;
            if (driver.DR1MST != null && driver.DR1MST.DR1MEDN != null)
            {
                foreach (var item in driver.DR1MST.DR1MEDN)
                {
                    if (item.MIDT != null && item.MIDT > result)
                    {
                        result = item.MIDT.Value;
                    }
                }
            }
            

            return result;
        }

        /// <summary>
        /// Get Driver History
        /// </summary>
        /// <param name="dlNumber"></param>
        /// <returns></returns>
        public CLNT GetDriverHistory(string dlNumber)
        {
            // Get base URL
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_configuration["ICBC_API_URI"]);

            // do a basic HTTP request
            var request = new HttpRequestMessage(HttpMethod.Get, "tombstone/"+dlNumber);

            // Get the JSON ICBC Response
            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            var response = client.SendAsync(request).GetAwaiter().GetResult();

            string rawData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            IcbcClient icbcClient = JsonConvert.DeserializeObject<IcbcClient>(rawData);

           

            ClientResult result = null;
            if (icbcClient != null)
            {


                result = new ClientResult()
                {
                    CLNT = new CLNT()
                    {
                        // Client Details

                        // Add Client Number
                        SEX = icbcClient.ClientDetails?.Gender,
                        SECK = icbcClient.ClientDetails?.SecurityKeyword,
                        BIDT = icbcClient.ClientDetails?.Birthdate,
                        WGHT = icbcClient.ClientDetails?.Weight,
                        HGHT = icbcClient.ClientDetails?.Height,

                        INAM = new INAM()
                        {
                            SURN = icbcClient.ClientDetails.Name?.Surname,
                            GIV1 = icbcClient.ClientDetails.Name?.GivenName1,
                            GIV2 = icbcClient.ClientDetails.Name?.GivenName2,
                            GIV3 = icbcClient.ClientDetails.Name?.GivenName3,
                        },
                        ADDR = new ADDR()
                        {
                            BUNO = icbcClient.ClientDetails.Address?.BuildingUnitNumber,
                            STNM = icbcClient.ClientDetails.Address?.StreetName,
                            STNO = icbcClient.ClientDetails.Address?.StreetNumber,
                            STDI = icbcClient.ClientDetails.Address?.StreetDirection,
                            STTY = icbcClient.ClientDetails.Address?.StreetType,
                            SITE = icbcClient.ClientDetails.Address?.Site,
                            COMP = icbcClient.ClientDetails.Address?.Comp,
                            RURR = icbcClient.ClientDetails.Address?.RuralRoute,
                            CITY = icbcClient.ClientDetails.Address?.City,
                            PROV = icbcClient.ClientDetails.Address?.ProvinceOrState,
                            CNTY = icbcClient.ClientDetails.Address?.Country,
                            POBX = icbcClient.ClientDetails.Address?.PostalCode,
                            APR1 = icbcClient.ClientDetails.Address?.AddressPrefix1,
                            APR2 = icbcClient.ClientDetails.Address?.AddressPrefix2,
                            EFDT = icbcClient.ClientDetails.Address?.EffectiveDate                            

                        },

                        // Driver Details

                        DR1MST = new DR1MST()
                        {
                            LNUM = icbcClient.DriversDetails?.LicenceNumber,
                            LCLS = icbcClient.DriversDetails?.LicenceClass,
                            RRDT = icbcClient.DriversDetails?.LicenceExpiryDate,
                            MSCD = icbcClient.DriversDetails?.MasterStatusCode,

                            // Restrictions need to add discription
                            
                            RSCD = icbcClient.DriversDetails.Restrictions?.Count >0 ? icbcClient.DriversDetails.Restrictions
                            .Select( restriction => restriction.RestrictionCode)
                            .ToList(): null,

                            
                            // Expanded Status
                            DR1STAT = icbcClient.DriversDetails.ExpandedStatuses?.Count > 0 ? icbcClient.DriversDetails.ExpandedStatuses
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
                            DR1MEDN = icbcClient.DriversDetails.Medicals?.Count > 0 ? icbcClient.DriversDetails.Medicals
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

        /// <summary>
        /// Client Result
        /// </summary>
        public class ClientResult
        {
            public CLNT CLNT { get; set; }
        }

        /// <summary>
        /// Log Statement
        /// </summary>
        /// <param name="hangfireContext"></param>
        /// <param name="message"></param>
        private void LogStatement(PerformContext hangfireContext, string message)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine(message);
            }
            // emit to Serilog.
            Log.Logger.Information(message);
        }

    }
}

