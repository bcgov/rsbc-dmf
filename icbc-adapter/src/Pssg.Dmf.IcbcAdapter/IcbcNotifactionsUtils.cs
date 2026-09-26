using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pssg.DocumentStorageAdapter;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.IcbcModels;
using Pssg.Interfaces.Models;
using Rsbc.Dmf.CaseManagement.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;

namespace Rsbc.Dmf.IcbcAdapter
{
    public class RehabNotifactionsUtils
    {

        private IConfiguration _configuration;
        private readonly CaseManager.CaseManagerClient _caseManagerClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient? _documentStorageAdapterClient;


        public RehabNotifactionsUtils(IConfiguration configuration, CaseManager.CaseManagerClient caseManagerClient, DocumentStorageAdapter.DocumentStorageAdapterClient? documentStorageAdapterClient)
        {
            _configuration = configuration;
            _caseManagerClient = caseManagerClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
        }

        public async Task GetRehabNotificationsAndUpdateCase()
        {
            var notifactions = await GetRehabNotifications();
            if (notifactions.NotificationFiles?.Count > 0)
            {
                foreach (var notification in notifactions.NotificationFiles.Values)
                {

                    var parseResult = await ParseRehabNotication(notification);
                    if (parseResult != null)
                    {
                        await CreateOrUpdateCases(parseResult.Records, parseResult.Errors, notification.FileName);
                    }
                }
                await RemoveFilesFromRehabFolder(notifactions.NotificationFiles.Keys);
            }

        }

        internal async Task CreateOrUpdateCases(List<DRVILS> cases, int errors, string fileName)
        {
            var total = 0;
            foreach (DRVILS dmf_case in cases)
            {
                try
                {
                    total++;
                    var caseToCreate = new CreateCaseRequest()
                    {
                        DriverLicenseNumber = dmf_case.LNUM,
                        CaseTypeCode = "REM",
                        TriggerType = dmf_case.CAND_CAUSE_CD,
                        Owner = "Remedial",
                        DriverDateOfBirth = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Parse(dmf_case.BIRTH_DT), DateTimeKind.Utc)),
                        DriverSurname = dmf_case.SURNAME
                    };

                    await _caseManagerClient.CreateCaseAsync(caseToCreate);
                }
                catch (Exception ex)
                {
                    errors++;
                    Log.Logger.Error("Error processing record in file " + fileName +": " + ex.Message);
                    
                }
            }
            Log.Logger.Information($"Successfully processed {total} cases with {errors} errors. See cms logs for more details");
        }

        public async Task RemoveFilesFromRehabFolder(IEnumerable<string> ServerRelativeUrl)
        {
            Log.Logger.Information("Removing files from Rehab folder");
            var request = new DeleteFilesInFolderRequest { BucketConfigName = "ICBC_NOTIFICATIONS_BUCKET" };
            request.ServerRelativeUrl.AddRange(ServerRelativeUrl);
            var result = await _documentStorageAdapterClient.DeleteFilesInFolderAsync(request);
            if (result.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
            {
                Log.Logger.Information("Successfully Removed files from Rehab folder");
            }
        }

        public async Task<ParseResult> ParseRehabNotication(IFormFile file)
        {
            var result = new ParseResult();

            Log.Logger.Information("Parsing Rehab Notification dat file " + file.FileName);
            if (file == null || file.Length == 0) { 
                Log.Logger.Information("File is empty or null.");
                return null;
            }
            result.Errors = 0;
            result.Records = new List<DRVILS>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var record = new DRVILS
                    {
                        LNUM = line.Length >= 8 ? line.Substring(0, 8).Trim() : null,
                        CLNO = line.Length >= 16 ? line.Substring(8, 9).Trim() : null,
                        SURNAME = line.Length >= 51 ? line.Substring(17, 35).Trim() : null,
                        GENDER = line.Length >= 52 ? line.Substring(52, 1).Trim() : null,
                        CAND_CAUSE_CD = line.Length >= 57 ? line.Substring(53, 5).Trim() : null,
                        BIRTH_DT = line.Length >= 65 ? line.Substring(58, 10) : null,
                        LIC_EXPIRY_DT = line.Length >= 73 ? line.Substring(68, 10) : null,
                        LAST_EXAM_DT = line.Length >= 81 ? line.Substring(78, 10) : null,
                        ADDR_DOCMNT_DT = line.Length >= 89 ? line.Substring(88, 10) : null,
                        MASTER_STATUS_CD = line.Length >= 90 ? line.Substring(98, 1).Trim() : null,
                        LIC_CLASS = line.Length >= 93 ? line.Substring(99, 3).Trim() : null,
                        CAND_SENT_DT = line.Length >= 102 ? line.Substring(102, 10) : null
                    };

                    string validationErrors = ValidateRecord(record);

                    if (validationErrors != null)
                    {

                        if (!line.Contains("RUN DATE"))
                        {
                            result.Errors++;
                            Log.Logger.Warning($"Failed to Parse File" + file.FileName + " Record was not added " + "\n Invalid values: " + validationErrors);
                        }
                    }

                    else
                    {
                        result.Records.Add(record);
                    }
                }
            }

            return result;
        }


        private string ValidateRecord(DRVILS record)
        {
            string errors = null;
            if (record.CAND_CAUSE_CD == null || record.CAND_CAUSE_CD == "")
            {
                errors += "\nMedical Type: " + record.CAND_CAUSE_CD;
            }

            return errors;
        }

        private async Task<RehabNotificationsFileResult> GetRehabNotifications()
        {
            var result = new RehabNotificationsFileResult();
            result.NotificationFiles = new Dictionary<string, IFormFile>();

            var files = await _documentStorageAdapterClient.DownloadFolderAsync(
                new DownloadFolderRequest { BucketConfigName = "ICBC_NOTIFICATIONS_BUCKET" });

            var topLevelFiles = files.Files.Where(f => !f.ServerRelativeUrl.Contains("/"));

            var fileNames = topLevelFiles.Select(f => f.ServerRelativeUrl).ToList();

            Log.Logger.Information("Fetching Rehab Notifications dat file(s):" + string.Join(",", fileNames));
            if (files.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
            {
                foreach (var fileBytes in topLevelFiles)
                {
                    var stream = new MemoryStream(fileBytes.Data.ToByteArray());

                    result.NotificationFiles[fileBytes.ServerRelativeUrl] = new FormFile(stream, 0, stream.Length, "file", fileBytes.ServerRelativeUrl)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "application/octet-stream"
                    };
                }
                Log.Logger.Information($"Successfully Fetched {result.NotificationFiles.Count} files from Rehab folder");
                return result;
            }
            else
            {
                return null;
            }

                
        }
    }
    public class ParseResult
    {
        public List<DRVILS> Records { get; set; }
        public int Errors { get; set; }
    }
}
