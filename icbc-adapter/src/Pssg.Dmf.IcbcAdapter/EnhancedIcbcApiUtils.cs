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

namespace Rsbc.Dmf.IcbcAdapter
{
    public class EnhancedIcbcApiUtils
    {
        
        private IConfiguration _configuration { get; }
        private readonly CaseManager.CaseManagerClient _caseManagerClient;


        public EnhancedIcbcApiUtils(IConfiguration configuration, CaseManager.CaseManagerClient caseManagerClient)
        {
            _configuration = configuration;
            _caseManagerClient = caseManagerClient;
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
                    LogStatement(hangfireContext, $"{unsentItem.CaseId} {unsentItem.Driver?.DriverLicenseNumber} - sending update");
                    var request = new HttpRequestMessage(HttpMethod.Post, "medical-disposition/update");

                    request.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

                    var response = client.SendAsync(request).GetAwaiter().GetResult();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // mark it as sent
                        MarkMedicalUpdateSent(unsentItem.CaseId);
                    }
                }

         
            }

            LogStatement(hangfireContext, "End of SendMedicalUpdates.");
        }

        private void MarkMedicalUpdateSent (string caseId)
        {            
            var idListRequest = new IdListRequest();
            idListRequest.IdList.Add(caseId);
            _caseManagerClient.MarkMedicalUpdatesSent(idListRequest);
        }

        public IcbcMedicalUpdate GetMedicalUpdateData (DmerCase item)
        {


            // Start by getting the current status for the given driver.  If the medical disposition matches, do not proceed.
                
            if (item.Driver != null)
            {
                var newUpdate = new IcbcMedicalUpdate()
                {
                    DlNumber = item.Driver.DriverLicenseNumber,
                    LastName = item.Driver.Surname,
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

