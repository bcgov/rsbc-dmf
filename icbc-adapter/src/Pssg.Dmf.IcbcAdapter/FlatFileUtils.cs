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

            if (string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(key)
                )

            {
                LogStatement(hangfireContext, "No SCP configuration, skipping operation.");
            }
            else
            {
                MemoryStream keyStream = new MemoryStream();
                StreamWriter writer = new StreamWriter(keyStream);
                writer.Write(key);
                writer.Flush();
                keyStream.Position = 0;

                PrivateKeyFile pkf = new PrivateKeyFile(keyStream);

                var connectionInfo = new ConnectionInfo(host,
                                        username,
                                        new PasswordAuthenticationMethod(username, password),
                                        new PrivateKeyAuthenticationMethod(keyUser, pkf));


                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    LogStatement(hangfireContext, "Connected.");

                    var files = client.ListDirectory(".");

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

            if (string.IsNullOrEmpty(host) || 
                string.IsNullOrEmpty(username) || 
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(key)
                )

            {
                LogStatement (hangfireContext, "No SCP configuration, skipping check for work.");
            }
            else
            {
                MemoryStream keyStream = new MemoryStream();
                StreamWriter writer = new StreamWriter(keyStream);
                writer.Write(key);
                writer.Flush();
                keyStream.Position = 0;

                PrivateKeyFile pkf = new PrivateKeyFile(keyStream);

                var connectionInfo = new ConnectionInfo(host,
                                        username,
                                        new PasswordAuthenticationMethod(username, password),
                                        new PrivateKeyAuthenticationMethod(keyUser,  pkf));
                

            using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    LogStatement(hangfireContext, "Connected.");

                    var files = client.ListDirectory(".");

                    foreach (var file in files)
                    {
                        LogStatement(hangfireContext, file.Name);
                        var memoryStream = new MemoryStream();
                        client.DownloadFile(file.FullName, memoryStream);

                        string data = StringUtility.StreamToString(memoryStream);
                        // process records
                        var engine = new FileHelperEngine<NewDriver>();                        
                        var records = engine.ReadString(data);
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
                            _caseManagerClient.ProcessLegacyCandidate(lcr);
                        }

                    }
                }
            }
           
            LogStatement(hangfireContext, "End of check for Candidates.");            
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

            if (string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(key)
                )

            {
                LogStatement(hangfireContext, "No SCP configuration, skipping operation.");
            }
            else
            {
                MemoryStream keyStream = StringUtility.StringToStream(key);

                PrivateKeyFile pkf = new PrivateKeyFile(keyStream);

                var connectionInfo = new ConnectionInfo(host,
                                        username,
                                        new PasswordAuthenticationMethod(username, password),
                                        new PrivateKeyAuthenticationMethod(keyUser, pkf));


                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    LogStatement(hangfireContext, "Connected.");

                    // construct the medical update file
                    string fileName = GetMedicalUpdateFilename();

                    List<MedicalUpdate> updateList = GetMedicalUpdateData();

                    string rawData = GetMedicalUpdateString(updateList);

                    MemoryStream data = StringUtility.StringToStream(rawData);

                    // transfer it.
                    client.UploadFile(data, fileName);
                }
            }

            LogStatement(hangfireContext, "End of SendMedicalUpdates.");
        }

        private List<MedicalUpdate> GetMedicalUpdateData ()
        {
            List<MedicalUpdate> data = new List<MedicalUpdate>();
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
        }

    }
}

