using Google.Protobuf.WellKnownTypes;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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

namespace Rsbc.Dmf.IcbcAdapter
{
    public class EnhancedIcbcApiUtils
    {

        private IConfiguration _configuration;
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

        public async Task SendMedicalUpdates()
        {
            Log.Logger.Error("Starting SendMedicalUpdates");

            // Get Unsent Medical for manual and clean pass

            var unsentItems = _caseManagerClient.GetUnsentMedicalPass(new CaseManagement.Service.EmptyRequest());
            if (unsentItems.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                foreach (var unsentItem in unsentItems.Items)
                {
                   // Log.Logger.Information($"Checking Pass Item {unsentItem.Driver.DriverLicenseNumber}");
                    var item = GetMedicalUpdateDataforPass(unsentItem);

                    if (item != null)
                    {
                       
                            string responseContent = _icbcClient.SendMedicalUpdate(item);

                            // 24-03-27 only try once to send an update.
                            MarkMedicalUpdateSent(unsentItem.CaseId);

                            if (!responseContent.Contains("SUCCESS"))
                            {
                                var bringForwardRequest = new BringForwardRequest
                                {
                                    CaseId = unsentItem.CaseId,
                                    Subject = "ICBC Error",
                                    Description = responseContent,
                                    Assignee = string.Empty,
                                    Priority = CallbackPriority.Normal

                                };

                                // 24-03-27 disable bring forwards
                                //_caseManagerClient.CreateBringForward(bringForwardRequest);

                                // Mark ICBC error 

                                var icbcError = new IcbcErrorRequest
                                {
                                    ErrorMessage = "ICBC Error"
                                };

                                _caseManagerClient.MarkMedicalUpdateError(icbcError);

                                Log.Logger.Error($"ICBC Error");
                            }
                        }
                       
                    else
                    {
                        Log.Logger.Error($"Null received from GetMedicalUpdateData for {unsentItem.CaseId} {unsentItem.Driver?.DriverLicenseNumber}");
                    }

                }

            }

            // create for one more call for GetmedicalAdjudication

            var unsentItemsAdjudication = _caseManagerClient.GetUnsentMedicalAdjudication(new CaseManagement.Service.EmptyRequest());

            if (unsentItemsAdjudication.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                foreach (var unsentItemAdjudication in unsentItemsAdjudication.Items)
                {
                    Log.Logger.Information($"Checking Adjudication Item {unsentItemAdjudication.Driver.DriverLicenseNumber}");
                    var item = GetMedicalUpdateDataforAdjudication(unsentItemAdjudication);
                    if (item != null)
                    {
                        string responseContent = _icbcClient.SendMedicalUpdate(item);
                        MarkMedicalUpdateSent(unsentItemAdjudication.CaseId);

                        if (!responseContent.Contains("SUCCESS"))
                        {
                            var bringForwardRequest = new BringForwardRequest
                            {
                                CaseId = unsentItemAdjudication.CaseId,
                                Subject = "ICBC Error",
                                Description = responseContent,
                                Assignee = string.Empty,
                                Priority = CallbackPriority.Normal
                            };

                            //_caseManagerClient.CreateBringForward(bringForwardRequest);

                            // Mark ICBC error 

                            var icbcError = new IcbcErrorRequest
                            {
                                ErrorMessage = "ICBC Error"
                            };

                            _caseManagerClient.MarkMedicalUpdateError(icbcError);

                            Log.Logger.Error($"ICBC Error");
                        }
                    }

                }
            }

            Log.Logger.Information("End of SendMedicalUpdates.");


        }

        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>

        public async Task SendMedicalUpdatesDryRun()
        {
            Log.Logger.Error("Starting SendMedicalUpdates Dry Run");

            // Get Unsent Medical for manual and clean pass

            Log.Logger.Information("*** P ***");

            var unsentItems = _caseManagerClient.GetUnsentMedicalPass(new CaseManagement.Service.EmptyRequest());

            if (unsentItems.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                foreach (var unsentItem in unsentItems.Items)
                {

                    var item = GetMedicalUpdateDataforPass(unsentItem);

                    if (item != null)
                    {
                        Log.Logger.Information($"SEND {unsentItem.Driver.DriverLicenseNumber} - PASS");
                    }
                    else
                    {
                        Log.Logger.Information($"SKIP {unsentItem.Driver.DriverLicenseNumber} - no need to send");
                    }
                }
            }
            else
            {
                Log.Logger.Error($"ERROR occurred when getting unsent items {unsentItems.ErrorDetail}");
            }
            
            // create for one more call for GetmedicalAdjudication

            Log.Logger.Information("*** J ***");

            var unsentItemsAdjudication = _caseManagerClient.GetUnsentMedicalAdjudication(new CaseManagement.Service.EmptyRequest());

            if (unsentItemsAdjudication.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                //Log.Logger.Information($"{unsentItemsAdjudication.Items.Count} Items Received");
                foreach (var unsentItemAdjudication in unsentItemsAdjudication.Items)
                {

                    var item = GetMedicalUpdateDataforAdjudication(unsentItemAdjudication);
                    if (item != null)
                    {
                        Log.Logger.Information($"SEND {unsentItemAdjudication.Driver.DriverLicenseNumber} - J ");
                    }
                    else
                    {
                        Log.Logger.Information($"SKIP {unsentItemAdjudication.Driver.DriverLicenseNumber} - no need to send");
                    }

                }
            }
            else
            {
                Log.Logger.Error($"ERROR occurred when getting unsent items {unsentItemsAdjudication.ErrorDetail}");
            }


            

            Log.Logger.Information("End of SendMedicalUpdates Dry Run.");


        }

        private void MarkMedicalUpdateSent(string caseId)
        {
            var idListRequest = new IdListRequest();
            idListRequest.IdList.Add(caseId);
            var result = _caseManagerClient.MarkMedicalUpdatesSent(idListRequest);

            Log.Logger.Error($"Mark Medical Update Sent {caseId} status is  {result.ResultStatus} {result.ErrorDetail}");
        }

        public IcbcMedicalUpdate GetMedicalUpdateDataforPass(DmerCase item)
        {

            // Start by getting the current status for the given driver.  If the medical disposition matches, do not proceed.

            if (item.Driver != null)
            {
                string licenseNumber = item.Driver.DriverLicenseNumber;
                try
                {
                    var driver = _icbcClient.GetDriverHistory(licenseNumber);
                    var medicalDispositionValue = GetMedicalDisposition(driver);
                    var driverStatus = GetDriverMasterStatus(driver);

                    if (driver == null || driver.INAM?.SURN == null)
                    {
                        Log.Logger.Error($"Null received for driver history for {licenseNumber}");
                    }

                    else
                    {

                        if (medicalDispositionValue != "P" && driverStatus != "DECEASED"
                           // Add check driver already has a P in the driver history
                           )
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
                            }

                            // get most recent Medical Issue Date from the driver.

                            DateTimeOffset adjustedDate = GetMedicalIssueDate(driver); // DateUtility.FormatDateOffsetPacific(GetMedicalIssueDate(driver)).Value;

                            newUpdate.MedicalIssueDate = adjustedDate;
                            if (newUpdate.MedicalDisposition == "P")
                            {
                                return newUpdate;
                            }
                            else
                            {
                                Log.Logger.Information("GetMedicalUpdateDataforPass No Fit to Drive - not sending P");
                            }
                            
                        }
                        else
                        {
                            Log.Logger.Information("GetMedicalUpdateDataforPass medicalDispositionValue already P");

                            // tag as sent.
                            MarkMedicalUpdateSent(item.CaseId);
                        }
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
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IcbcMedicalUpdate GetMedicalUpdateDataforAdjudication(DmerCase item)
        {

            // Start by getting the current status for the given driver.  If the medical disposition matches, do not proceed.

            if (item.Driver != null)
            {
                string licenseNumber = item.Driver.DriverLicenseNumber;
                try
                {
                    var driver = _icbcClient.GetDriverHistory(licenseNumber);
                    var medicalDispositionValue = GetMedicalDisposition(driver);
                    var driverStatus = GetDriverMasterStatus(driver);

                    if (driver == null && driver.INAM?.SURN == null)
                    {
                        Log.Logger.Error("Error getting driver from ICBC.");
                    }
                    else
                    {
                        // check driver already has a J in the driver history
                        if (medicalDispositionValue != "J" && driverStatus != "DECEASED")
                        {
                            var newUpdate = new IcbcMedicalUpdate()
                            {
                                DlNumber = licenseNumber,
                                LastName = driver.INAM.SURN,
                                MedicalDisposition = "J"
                            };


                            // get most recent Medical Issue Date from the driver.

                            DateTimeOffset adjustedDate = GetMedicalIssueDate(driver); // DateUtility.FormatDateOffsetPacific(GetMedicalIssueDate(driver)).Value;

                            newUpdate.MedicalIssueDate = adjustedDate;

                            return newUpdate;
                        }
                        else
                        {
                            Log.Logger.Information("GetMedicalUpdateDataforAdjudication medicalDispositionValue already J");

                            // tag as sent.
                            MarkMedicalUpdateSent(item.CaseId);
                        }

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
        /// UpdateBirthdateFromIcbc
        /// </summary>
        /// <returns></returns>

        public async Task UpdateBirthdateFromIcbc()
        {

            // Get List of  drivers from dynamics

            var driversReply = _caseManagerClient.GetDrivers(new CaseManagement.Service.EmptyRequest());

            foreach (var driver in driversReply.Items)
            {
                var dlNumber = driver.DriverLicenseNumber;

                // Call the tombstone endpoint
                var response = _icbcClient.GetDriverHistory(dlNumber);
                if (response != null && response.BIDT != null)
                {
                    // Compare Dynamics DOB and ICBC DOB
                    if (driver.BirthDate.ToDateTime() != (DateTime)response.BIDT)
                    {
                        _caseManagerClient.UpdateDriver(new CaseManagement.Service.Driver
                        {
                            DriverLicenseNumber = dlNumber,
                            BirthDate = Timestamp.FromDateTimeOffset(response.BIDT ?? DateTime.Now),
                            GivenName = response.INAM?.GIV1 ?? string.Empty,
                            Surname = response.INAM?.SURN ?? string.Empty
                        });
                    }

                }
            }
        }

        /// <summary>
        /// Get Medical IssueDate
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public DateTime GetMedicalIssueDate(CLNT driver)
        {
            DateTime result = DateTime.MinValue;
            if (driver.DR1MST != null && driver.DR1MST.DR1MEDN != null)
            {
                foreach (var item in driver.DR1MST.DR1MEDN)
                {
                    if (item.MDSP != null && item.MDSP != "I" && item.MIDT != null && item.MIDT > result)
                    {
                        result = item.MIDT.Value;
                    }
                }
            }


            return result;
        }

        /// <summary>
        /// Get Medical Disposition
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public string GetMedicalDisposition(CLNT driver)
        {
            string result = string.Empty;

            if (driver.DR1MST != null && driver.DR1MST.DR1MEDN != null)
            {
                foreach (var item in driver.DR1MST.DR1MEDN)
                {
                    if (item.MDSP != null && item.MDSP != "I")
                    {
                        result = item.MDSP;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get GetDriverMasterStatus
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public string GetDriverMasterStatus(CLNT driver)
        {
            string result = string.Empty;

            if (driver.DR1MST != null)
            {
                

                result = driver.DR1MST.MSCD;
              
            }

            return result;
        }

        public async Task GetIcbcNotificationsAndUpdateCase()
        {
            var notificationFile = await GetIcbcNotifications();

            var cases = await ParseIcbcNotication(notificationFile);

            CreateOrUpdateCases(cases);
        }

        internal void CreateOrUpdateCases(List<DRVILS> cases)
        {
            Log.Logger.Information("Creating Or Updating case with ICBC Notification data...");
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

                    if (validationErrors != "")
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
            if (record.LNUM.Contains(" "))
            {
                errors += "\nLNUM: " + record.LNUM;
            }
            if (record.CLNO.Contains(" "))
            {
                errors += "\nCLNO: " + record.CLNO;
            }
            char[] genderCharactors = { 'M', 'F', 'X', 'U' };
            if (genderCharactors.Any(c=> record.GENDER.Contains(c)))
            {
                errors += "\nGENDER: " + record.GENDER;
            }
            if (DateTime.TryParseExact(record.BIRTH_DT, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
            {
                errors += "\nBIRTH_DT: " + record.BIRTH_DT;
            }
            if (DateTime.TryParseExact(record.LIC_EXPIRY_DT, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
            {
                errors += "\nLIC_EXPIRY_DT: " + record.LIC_EXPIRY_DT;
            }
            if (DateTime.TryParseExact(record.LIC_EXPIRY_DT, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
            {
                errors += "\nLAST_EXAM_DT: " + record.LAST_EXAM_DT;
            }
            if (DateTime.TryParseExact(record.ADDR_DOCMNT_DT, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
            {
                errors += "\nADDR_DOCMNT_DT : " + record.ADDR_DOCMNT_DT;
            }
            if (DateTime.TryParseExact(record.LIC_EXPIRY_DT, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _))
            {
                errors += "\nCAND_SENT_DT: " + record.CAND_SENT_DT;
            }
            return string.Empty;
        }

        private async Task<IFormFile> GetIcbcNotifications()
        {
            Log.Logger.Information("Fetching ICBC Notifications dat file...");

            var filePath = @"C:\Users\FintanR\Downloads\drv-ilsnew-202506110013 (1).dat";
            var fileName = Path.GetFileName(filePath);
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream",
                ContentDisposition = $"form-data; name=\"file\"; filename=\"{fileName}\""
            };

            return formFile;
        }

        public class ClientResult
        {
            public CLNT CLNT { get; set; }
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
