using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Rsbc.Dmf.LegacyAdapter
{
    public static class CaseUtils
    {

        static public HashSet<string> closedStatus = new HashSet<string>
            {
                "Decision Rendered",
                "Canceled"
            };

        /// <summary>
        /// Get Case Id
        /// </summary>
        /// <param name="licenseNumber"></param>
        /// <param name="surcode"></param>
        /// <returns></returns>
        public static string GetCaseId(this CaseManager.CaseManagerClient _cmsAdapterClient, string licenseNumber, string surcode)
        {
            

            string trimmedSurcode = surcode;
            if (trimmedSurcode.Length > 3)
            {
                trimmedSurcode = trimmedSurcode.Substring(0, 3);
            }

            string caseId = null;
            var reply = _cmsAdapterClient.Search(new SearchRequest { DriverLicenseNumber = licenseNumber ?? string.Empty });
            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // ensure newest first.
                var sorted = reply.Items.OrderByDescending(x => x.CreatedOn);

                foreach (var item in sorted)
                {

                    if (!closedStatus.Contains(item.Status))
                    {
                        if ((bool)(item.Driver?.Surname.ToUpper().StartsWith(trimmedSurcode.ToUpper())))
                        {
                            caseId = item.CaseId;
                            break;
                        }
                    }
                }
            }
            return caseId;
        }

        /// <summary>
        /// Get a Case ID by DL, Surcode and Sequence Number
        /// </summary>
        /// <param name="_cmsAdapterClient"></param>
        /// <param name="licenseNumber"></param>
        /// <param name="surcode"></param>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
        public static string GetCaseId(this CaseManager.CaseManagerClient _cmsAdapterClient, string licenseNumber, string surcode, int sequenceNumber)
        {
            
            string trimmedSurcode = surcode;
            if (trimmedSurcode.Length > 3)
            {
                trimmedSurcode = trimmedSurcode.Substring(0, 3);
            }

            string caseId = null;
            var reply = _cmsAdapterClient.Search(new SearchRequest { DriverLicenseNumber = licenseNumber ?? string.Empty });
            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // ensure newest first.
                var sorted = reply.Items.OrderByDescending(x => x.CreatedOn);

                foreach (var item in sorted)
                {

                    if (!closedStatus.Contains(item.Status))
                    {
                        if ((bool)(item.Driver?.Surname.ToUpper().StartsWith(trimmedSurcode.ToUpper())) && item.CaseSequence == sequenceNumber)
                        {
                            caseId = item.CaseId;
                            break;
                        }
                    }
                }
            }
            return caseId;
        }


        public static string GetCaseId(this CaseManager.CaseManagerClient _cmsAdapterClient, string licenseNumber)
        {           
            string caseId = null;
            var reply = _cmsAdapterClient.Search(new SearchRequest { DriverLicenseNumber = licenseNumber ?? string.Empty });
            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                // ensure newest first.
                var sorted = reply.Items.OrderByDescending(x => x.CreatedOn);

                foreach (var item in sorted)
                {
                    if (!closedStatus.Contains(item.Status))
                    {
                        caseId = item.CaseId;
                        break;
                    }
                }
            }
            return caseId;
        }
        
    }
}
