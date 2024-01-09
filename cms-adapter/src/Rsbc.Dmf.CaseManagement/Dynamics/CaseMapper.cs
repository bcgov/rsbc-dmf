using Rsbc.Dmf.CaseManagement.Utilities;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public static class CaseMapper
    {
        public static async Task<CaseDetail> Map(incident @case)
        {
            var result = new CaseDetail();

            dynamicsContext.LoadProperty(@case, nameof(incident.dfp_DriverId));
            result = new CaseDetail
            {
                CaseId = @case.incidentid.ToString(),
                DriverId = @case.dfp_DriverId?.dfp_driverid.ToString() ?? string.Empty,
                Title = @case.title,
                IdCode = @case.ticketnumber,
                OpenedDate = @case.createdon.Value,
                LastActivityDate = @case.modifiedon.Value,
                LatestDecision = null
            };

            if (@case.dfp_dfcmscasesequencenumber == null)
            {
                result.CaseSequence = -1;
            }
            else
            {
                result.CaseSequence = @case.dfp_dfcmscasesequencenumber.Value;
            }

            // get the case type.
            if (@case.casetypecode != null)
            {
                result.CaseType = TranslateCaseTypeToString(@case.casetypecode);
            }

            if (@case.dfp_dmertype != null)
            {
                result.DmerType = TranslateDmerTypeRaw(@case.dfp_dmertype);
            }

            await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.stageid_processstage));

            var bpf = dynamicsContext.dfp_dmfcasebusinessprocessflows.Where(x => x._bpf_incidentid_value == @case.incidentid).FirstOrDefault();

            if (bpf != null)
            {
                await dynamicsContext.LoadPropertyAsync(bpf, nameof(dfp_dmfcasebusinessprocessflow.activestageid));
                result.Status = bpf.activestageid.stagename;
            }

            // case assignment
            if (@case._owningteam_value.HasValue)
            {
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.owningteam));
                result.AssigneeTitle = @case.owningteam.name;
            }

            // get the related decisions.
            await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_decision));
            if (@case.dfp_incident_dfp_decision != null && @case.dfp_incident_dfp_decision.Count > 0)
            {
                foreach (var decision in @case.dfp_incident_dfp_decision)
                {
                    if ((result.DecisionDate == null || decision.createdon > result.DecisionDate) && decision.statecode == 0)
                    {
                        result.LatestDecision = "";

                        await dynamicsContext.LoadPropertyAsync(decision, nameof(dfp_decision.dfp_OutcomeStatus));
                        if (decision.dfp_OutcomeStatus != null)
                        {
                            result.LatestDecision = decision.dfp_OutcomeStatus.dfp_name;
                        }

                        // now try and get the sub type
                        await dynamicsContext.LoadPropertyAsync(decision, nameof(dfp_decision.dfp_OutcomeSubStatus));
                        if (decision.dfp_OutcomeSubStatus != null)
                        {
                            result.LatestDecision += " - " + decision.dfp_OutcomeSubStatus.dfp_name;
                        }

                        result.DecisionDate = decision.createdon;
                        result.DecisionForClass = TranslateDecisionForClass(decision.dfp_eligibledlclass);
                    }
                }
            }

            result.DpsProcessingDate = GetDpsProcessingDate();
        }

        public static string TranslateCaseTypeToString(int? optionSetValue)
        {
            string result = null;
            switch (optionSetValue)
            {
                case 100000004:
                    result = "OTHR";
                    break;
                case 100000002:
                    result = "POL";
                    break;
                case 100000001:
                    result = "LEG";
                    break;
                case 2:
                    result = "DMER";
                    break;
                case 100000003:
                    result = "RSBC";
                    break;
                case 3:
                    result = "PDR";
                    break;
                case 100000005:
                    result = "UNSL";
                    break;
            }
            return result;
        }

        public static string TranslateDmerTypeRaw(int? optionSetValue)
        {
            string result = null;
            switch (optionSetValue)
            {
                case 100000000:
                    result = "Commercial/NSC";
                    break;
                case 100000001:
                    result = "Age";
                    break;
                case 100000002:
                    result = "Industrial Road";
                    break;
                case 100000003:
                    result = "Known Medical";
                    break;
                case 100000006:
                    result = "Suspected Medical";
                    break;
                case 100000005:
                    result = "No DMER";
                    break;
            }
            return result;
        }

        public static string TranslateDecisionForClass(string data)
        {
            string result = null;
            if (data != null)
            {
                var items = data.Split(",");
                result = "";
                foreach (var item in items)
                {
                    if (result.Length > 0)
                    {
                        result += ", ";
                    }
                    switch (item)
                    {
                        case "100000001":
                            result += "C1";
                            break;
                        case "100000002":
                            result += "C2";
                            break;
                        case "100000003":
                            result += "C3";
                            break;
                        case "100000004":
                            result += "C4";
                            break;
                        case "100000005":
                            result += "C5/C7";
                            break;
                        case "100000006":
                            result += "C6";
                            break;
                    }
                }
            }

            return result;
        }
    }
}
