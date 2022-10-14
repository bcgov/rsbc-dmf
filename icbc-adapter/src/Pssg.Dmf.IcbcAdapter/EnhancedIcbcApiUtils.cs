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

            var unsentItems = _caseManagerClient.GetUnsentMedicalUpdates(new EmptyRequest());

            

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_configuration["ICBC_API_URI"]);

            foreach (var unsentItem in unsentItems.Items)
            {
                var item = GetMedicalUpdateData(unsentItem);

                if (item != null)
                {
                    
                    var request = new HttpRequestMessage(HttpMethod.Put, "medical-disposition/update");
                    string payload = JsonConvert.SerializeObject(item);
                    LogStatement(hangfireContext, $"{unsentItem.CaseId} {unsentItem.Driver?.DriverLicenseNumber} - sending update {payload}");
                    request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                    var response = client.SendAsync(request).GetAwaiter().GetResult();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        LogStatement(hangfireContext, $"HTTP Status was OK {unsentItem.CaseId} {unsentItem.Driver?.DriverLicenseNumber} {response.Content.ReadAsStringAsync().GetAwaiter().GetResult()} - marking as sent.");
                        // mark it as sent
                        MarkMedicalUpdateSent(hangfireContext, unsentItem.CaseId);
                    }
                    else
                    {
                        LogStatement(hangfireContext, $"HTTP Status was not OK {response.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
                    }
                }
                else
                {
                    LogStatement(hangfireContext, $"Null received from GetMedicalUpdateData for {unsentItem.CaseId} {unsentItem.Driver?.DriverLicenseNumber}");
                }

            }

            LogStatement(hangfireContext, "End of SendMedicalUpdates.");
        }

        private void MarkMedicalUpdateSent (PerformContext hangfireContext, string caseId)
        {            
            var idListRequest = new IdListRequest();
            idListRequest.IdList.Add(caseId);
            var result = _caseManagerClient.MarkMedicalUpdatesSent(idListRequest);

            LogStatement(hangfireContext, $"Mark Medical Update Sent {caseId} status is  {result.ResultStatus} {result.ErrorDetail}");
        }

        public IcbcMedicalUpdate GetMedicalUpdateData (DmerCase item)
        {

            // Start by getting the current status for the given driver.  If the medical disposition matches, do not proceed.
                
            if (item.Driver != null)
            {
                string licenseNumber = item.Driver.DriverLicenseNumber;
                try
                {
                    var driver = _icbcClient.GetDriverHistory(licenseNumber);
                    if (driver != null)
                    {
                        var newUpdate = new IcbcMedicalUpdate()
                        {
                            DlNumber = item.Driver.DriverLicenseNumber,
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

                        DateTimeOffset adjustedDate = DateUtility.FormatDateOffsetPacific(DateTimeOffset.UtcNow).Value;

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

