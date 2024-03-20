using Google.Protobuf.WellKnownTypes;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces.IcbcModels;
using Rsbc.Dmf.CaseManagement.Service;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task SendMedicalUpdates()
        {
            Log.Logger.Error("Starting SendMedicalUpdates");

            // Get Unsent Medical for manual and clean pass

            var unsentItems = _caseManagerClient.GetUnsentMedicalPass(new CaseManagement.Service.EmptyRequest());



            foreach (var unsentItem in unsentItems.Items)
            {
                Log.Logger.Information($"SENDING {unsentItem.Driver.DriverLicenseNumber}");
                var item = GetMedicalUpdateDataforPass(unsentItem);

                if (item != null)
                {

                    string responseContent = _icbcClient.SendMedicalUpdate(item);

                    if (responseContent.Contains("SUCCESS"))
                    {
                        // mark it as sent
                        MarkMedicalUpdateSent(unsentItem.CaseId);
                    }
                    else
                    {
                        var bringForwardRequest = new BringForwardRequest
                        {
                            CaseId = unsentItem.CaseId,
                            Subject = "ICBC Error",
                            Description = responseContent,
                            Assignee = string.Empty,
                            Priority = CallbackPriority.Normal

                        };

                        _caseManagerClient.CreateBringForward(bringForwardRequest);

                        // Mark ICBC error 

                        var icbcError = new IcbcErrorRequest
                        {
                            ErrorMessage = "ICBC Error"
                        };

                        _caseManagerClient.MarkMedicalUpdateError(icbcError);

                        Log.Logger.Error($"ICBC Error {responseContent}");
                    }
                }


                else
                {
                    Log.Logger.Error($"Null received from GetMedicalUpdateData for {unsentItem.CaseId} {unsentItem.Driver?.DriverLicenseNumber}");
                }


            }

            Log.Logger.Error("End of SendMedicalUpdates.");

            // create for one more call for GetmedicalAdjudication

            var unsentItemsAdjudication = _caseManagerClient.GetUnsentMedicalAdjudication(new CaseManagement.Service.EmptyRequest());
            foreach (var unsentItemAdjudication in unsentItemsAdjudication.Items)
            {

                var item = GetMedicalUpdateDataforAdjudication(unsentItemAdjudication);
                if (item != null)
                {

                    string responseContent = _icbcClient.SendMedicalUpdate(item);
                    Log.Logger.Information(responseContent);

                    if (responseContent.Contains("SUCCESS"))
                    {
                        // mark it as sent
                        MarkMedicalUpdateSent(unsentItemAdjudication.CaseId);

                    }

                    else
                    {
                        var bringForwardRequest = new BringForwardRequest
                        {
                            CaseId = unsentItemAdjudication.CaseId,
                            Subject = "ICBC Error",
                            Description = responseContent,
                            Assignee = string.Empty,
                            Priority = CallbackPriority.Normal

                        };

                        _caseManagerClient.CreateBringForward(bringForwardRequest);

                        // Mark ICBC error 

                        var icbcError = new IcbcErrorRequest
                        {
                            ErrorMessage = "ICBC Error"
                        };

                        _caseManagerClient.MarkMedicalUpdateError(icbcError);

                        Log.Logger.Error($"ICBC Error {responseContent}");
                    }
                }

            }


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

                    if (driver == null || driver.INAM?.SURN == null)
                    {
                        Log.Logger.Error($"Null received for driver history for {licenseNumber}");
                    }
                    else
                    {

                        if (medicalDispositionValue != "P"
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

                            return newUpdate;
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

                    if (driver == null && driver.INAM?.SURN == null)
                    {
                        Log.Logger.Error("Error getting driver from ICBC.");
                    }
                    else
                    {
                        // check driver already has a J in the driver history
                        if (medicalDispositionValue != "J")
                        {
                            var newUpdate = new IcbcMedicalUpdate()
                            {
                                DlNumber = licenseNumber,
                                LastName = driver.INAM.SURN,
                            };

                            if (newUpdate != null)
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
