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
using SharedUtils.Gov.Lclb.Cllb.Public.Utils;
using SharedUtils;
using FileHelpers;
using Pssg.Interfaces.FlatFileModels;
using Pssg.Interfaces.Icbc.FlatFileModels;
using Rsbc.Dmf.CaseManagement.Service;
using System.Linq;

namespace Rsbc.Dmf.IcbcAdapter
{
    public enum ChangeNameType
    {
        ChangeName = 1,
        Transfer = 2,
        ThirdPartyOperator = 3
    }
    public class FlatFileUtils
    {
        

        private IConfiguration _configuration { get; }
        private readonly CaseManager.CaseManagerClient _caseManagerClient;


        public FlatFileUtils(IConfiguration configuration, CaseManager.CaseManagerClient caseManagerClient)
        {
            _configuration = configuration;
            _caseManagerClient = caseManagerClient;
        }

        private bool CheckScpSettings(string host, string username, string password, string key)
        {
            return string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(key);
        }

        private ConnectionInfo GetConnectionInfo (string host, string username, string password, string keyUser, string key)
        {
            // note - key must be in RSA format.  If your key is in OpenSSH format, use this to convert it:
            // ssh-keygen -p -P "" -N "" -m pem -f \path\to\key\file
            // (above command will overwrite your key file)

            byte[] keyData = Encoding.UTF8.GetBytes(key);

            PrivateKeyFile pkf = null;

            using (var privateKeyStream = new MemoryStream(keyData))
            {
                pkf = new PrivateKeyFile(privateKeyStream);
            }

            var connectionInfo = new ConnectionInfo(host,
                                    username,                                     
                                    new PrivateKeyAuthenticationMethod(keyUser, pkf));

            return connectionInfo;
        }

        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckConnection(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting CheckConnection.");

            // Attempt to connect to a SCP server.

            string username = _configuration["SCP_USER"];
            string password = _configuration["SCP_PASS"];
            string host = _configuration["SCP_HOST"];
            string keyUser = _configuration["SCP_KEY_USER"];
            string key = _configuration["SCP_KEY"];

            if (CheckScpSettings(host, username, password, key))
            {
                LogStatement(hangfireContext, "No SCP configuration, skipping operation.");
            }
            else
            {                
                var connectionInfo = GetConnectionInfo (host, username, password, keyUser, key);

                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    LogStatement(hangfireContext, "Connected.");

                    var files = client.ListDirectory(client.WorkingDirectory);

                    foreach (var file in files)
                    {
                        LogStatement(hangfireContext, file.Name);
                    }

                }

            }



            LogStatement(hangfireContext, "End of CheckConnection.");
        }

        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForCandidates(PerformContext hangfireContext)
        {            
            LogStatement(hangfireContext, "Starting check for Candidates.");

            // Attempt to connect to a SCP server.

            string username = _configuration["SCP_USER"];
            string password = _configuration["SCP_PASS"];
            string host = _configuration["SCP_HOST"];
            string keyUser = _configuration["SCP_KEY_USER"];
            string key = _configuration["SCP_KEY"];

            string folder = _configuration["SCP_FOLDER"];

            if (CheckScpSettings(host, username, password, key))
            {
                LogStatement (hangfireContext, "No SCP configuration, skipping check for work.");
            }
            else
            {
                var connectionInfo = GetConnectionInfo(host, username, password, keyUser, key);

                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    LogStatement(hangfireContext, "Connected.");

                    if (string.IsNullOrEmpty(folder))
                    {
                        folder = client.WorkingDirectory;
                    }

                    var files = client.ListDirectory(folder);

                    foreach (var file in files)
                    {
                        LogStatement(hangfireContext, file.Name);
                        var memoryStream = new MemoryStream();
                        client.DownloadFile(file.FullName, memoryStream);

                        string data = StringUtility.StreamToString(memoryStream);
                        LogStatement(hangfireContext, data);
                        ProcessCandidates(hangfireContext, data);

                    }
                }
            }
           
            LogStatement(hangfireContext, "End of check for Candidates.");            
        }

        public void ProcessCandidates(PerformContext hangfireContext, string data)
        {
            // process records
            var engine = new FileHelperEngine<NewDriver>();
            engine.Options.IgnoreLastLines = 1;
            var records = engine.ReadString(data);
            
            LogStatement(hangfireContext, $"{records.Length} records were found.");
            
            foreach (var record in records)
            {
                LogStatement(hangfireContext, $"Found record {record.LicenseNumber} {record.Surname}");
                // Add / Update cases
                LegacyCandidateRequest lcr = new LegacyCandidateRequest()
                {
                    LicenseNumber = record.LicenseNumber,
                    Surname = record.Surname,
                    ClientNumber = record.ClientNumber,
                };
                //_caseManagerClient.ProcessLegacyCandidate(lcr);
            }
        }

        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task SendMedicalUpdates(PerformContext hangfireContext)
        {
            LogStatement(hangfireContext, "Starting SendMedicalUpdates");

            // Attempt to connect to a SCP server.

            string username = _configuration["SCP_USER"];
            string password = _configuration["SCP_PASS"];
            string host = _configuration["SCP_HOST"];
            string keyUser = _configuration["SCP_KEY_USER"];
            string key = _configuration["SCP_KEY"];

            if (CheckScpSettings(host, username, password, key))
            {
                LogStatement(hangfireContext, "No SCP configuration, skipping operation.");
            }
            else
            {
                var connectionInfo = GetConnectionInfo(host, username, password, keyUser, key);

                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    LogStatement(hangfireContext, "Connected.");

                    // construct the medical update file
                    string fileName = GetMedicalUpdateFilename();

                    var unsentItems = _caseManagerClient.GetUnsentMedicalUpdates(new EmptyRequest());

                    var updateList = GetMedicalUpdateData(unsentItems);

                    string rawData = GetMedicalUpdateString(updateList);

                    MemoryStream data = StringUtility.StringToStream(rawData);

                    // transfer it.
                    client.UploadFile(data, fileName);

                    // mark as sent.
                    MarkMedicalUpdatesSent(unsentItems);
                }
            }

            LogStatement(hangfireContext, "End of SendMedicalUpdates.");
        }

        private void MarkMedicalUpdatesSent (SearchReply unsentItems)
        {
            List<string> idList = new List<string>();
            foreach (var item in unsentItems.Items)
            {
                idList.Add(item.CaseId);
            }
            //_caseManagerClient
        }

        public List<MedicalUpdate> GetMedicalUpdateData (SearchReply unsentItems)
        {
            List<MedicalUpdate> data = new List<MedicalUpdate>();

            foreach (DmerCase item in unsentItems.Items)
            {
                // Start by getting the current status for the given driver.  If the medical disposition matches, do not proceed.
                
                if (item.Driver != null)
                {
                    var newUpdate = new MedicalUpdate()
                    {
                        LicenseNumber = item.Driver.DriverLicenseNumber,
                        Surname = item.Driver.Surname,
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

                    DateTime? adjustedDate = DateUtility.FormatDatePacific(DateTimeOffset.UtcNow);

                    newUpdate.MedicalIssueDate = adjustedDate.Value.ToString("yyyyMMddHHmmss");

                    data.Add(newUpdate);
                }
                else
                {
                    Log.Logger.Information($"Case {item.CaseId} {item.Title} has no Driver..");
                }
                
            }

            return data;
        }

        string GetMedicalUpdateString(List<MedicalUpdate> data)
        {
            string result = null;
            var engine = new FileHelperEngine<MedicalUpdate>();
            engine.WriteString(data);
            return result;
        }

        private string GetMedicalUpdateFilename(DateTimeOffset? currentTime = null)
        {
            string result = null;
            if (currentTime == null)
            {
                currentTime = DateTimeOffset.UtcNow;
            }

            DateTime? adjustedDate = DateUtility.FormatDatePacific(currentTime);

            string prefix = "RSBCMED-UPDATE";            
            string formattedDate = adjustedDate.Value.ToString("yyyyMMddHHmmss");                            
            string suffix = ".dat";
            result = $"{prefix}{formattedDate}{suffix}";
            return result;
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

