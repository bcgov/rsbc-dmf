using Rsbc.Dmf.CaseManagement.Service;
using System.Collections.Generic;
using System.Linq;

namespace Rsbc.Dmf.IcbcAdapter
{
    public static class CaseUtils
    {
        static public HashSet<string> closedStatus = new HashSet<string>
            {
                "Decision Rendered",
                "Canceled"
            };

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

    }
}
