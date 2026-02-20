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
    public class IcbcNotifactionsUtils
    {

        private IConfiguration _configuration;
        private readonly CaseManager.CaseManagerClient _caseManagerClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient? _documentStorageAdapterClient;

        private readonly IIcbcClient _icbcClient;

        public IcbcNotifactionsUtils(IConfiguration configuration, CaseManager.CaseManagerClient caseManagerClient, IIcbcClient icbcClient, DocumentStorageAdapter.DocumentStorageAdapterClient? documentStorageAdapterClient)
        {
            _configuration = configuration;
            _caseManagerClient = caseManagerClient;
            _icbcClient = icbcClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
        }

        public async Task GetIcbcNotificationsAndUpdateCase()
        {
            var notifactions = await GetIcbcNotifications();
            if (notifactions.NotificationFiles?.Count > 0)
            {
                foreach (var notification in notifactions.NotificationFiles)
                {

                    var cases = await ParseIcbcNotication(notification);

                    await CreateOrUpdateCases(cases);
                }
                await RemoveFilesFromIcbcS3Bucket(notifactions.ServerRelativeUrl);
            }

        }

        internal async Task CreateOrUpdateCases(List<DRVILS> cases)
        {
            try
            {
                foreach (DRVILS dmf_case in cases)
                {
                    var caseToCreate = new CreateCaseRequest()
                    {
                        DriverLicenseNumber = dmf_case.LNUM,
                        CaseTypeCode = "REM",
                        TriggerType = dmf_case.CAND_CAUSE_CD,
                        Owner = "Remedial"
                    };

                    await _caseManagerClient.CreateCaseAsync(caseToCreate);
                }
                Log.Logger.Information($"Successfully added {cases.Count} cases ");
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Error creating/updating cases: " + ex.Message);
            }
            

        }

        public async Task RemoveFilesFromIcbcS3Bucket(IEnumerable<string> ServerRelativeUrl)
        {
            Log.Logger.Information("Removing files from icbc S3 bucket");
            var request = new DeleteFilesInFolderRequest { BucketConfigName = "ICBC_NOTIFICATIONS_BUCKET" };
            request.ServerRelativeUrl.AddRange(ServerRelativeUrl);
            var result = await _documentStorageAdapterClient.DeleteFilesInFolderAsync(request);
            if (result.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
            {
                Log.Logger.Information("Successfully Removed files from icbc S3 bucket");
            }
        }

        public async Task<List<DRVILS>> ParseIcbcNotication(IFormFile file)
        {
            Log.Logger.Information("Parsing ICBC Notification dat file...");
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            var records = new List<DRVILS>();

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
                        Log.Logger.Warning($"Record was not added: " + record.ToString() + "\n Invalid values: " + validationErrors);
                    }

                    else
                    {
                        records.Add(record);
                    }
                }
            }

            return records;
        }


        private string ValidateRecord(DRVILS record)
        {
            string errors = null;
            if (record.LNUM.Contains(" ") || record.LNUM == null || record.LNUM =="")
            {
                errors += "\nLNUM: " + record.LNUM;
            }
            if (record.CAND_CAUSE_CD == null || record.CAND_CAUSE_CD == "")
            {
                errors += "\nCAND_CAUSE_CD: " + record.CAND_CAUSE_CD;
            }

            return errors;
        }

        private async Task<IcbcNotificationsFileResult> GetIcbcNotifications()
        {
            var result = new IcbcNotificationsFileResult();
            result.NotificationFiles = new List<IFormFile>();
            var files = await _documentStorageAdapterClient.DownloadFolderAsync(
            new DownloadFolderRequest { BucketConfigName = "ICBC_NOTIFICATIONS_BUCKET" });
            Log.Logger.Information("Fetching ICBC Notifications dat file..");
            if (files.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
            {
                foreach (var fileBytes in files.Files)
                {
                    var stream = new MemoryStream(fileBytes.Data.ToByteArray());

                    result.NotificationFiles.Add(new FormFile(stream, 0, stream.Length, "file", "ICBC_Notifactions")
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "application/octet-stream"
                    });
                }
                result.ServerRelativeUrl = files.Files.Select(x=> x.ServerRelativeUrl);
                Log.Logger.Information($"Successfully Fetched {result.NotificationFiles.Count} files from icbc S3 bucket");
                return result;
            }
            else
            {
                return null;
            }

                
        }
    }
}
