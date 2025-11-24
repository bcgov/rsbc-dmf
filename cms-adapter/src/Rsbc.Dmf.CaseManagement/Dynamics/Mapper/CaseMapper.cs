using AutoMapper;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public class CaseAutoMapperProfile : Profile
    {

        public CaseAutoMapperProfile()
        {
            CreateMap<incident, Dto.Case>()
                // TODO rename
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.incidentid))
                .ForMember(dest => dest.CaseNumber, opt => opt.MapFrom(src => src.ticketnumber))
                .ForMember(dest => dest.DmerType, opt => opt.MapFrom(src => TranslateDmerType(src.dfp_dmertype)))
                .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.customerid_contact))
                .ForMember(dest => dest.LatestComplianceDate, opt => opt.MapFrom(src => src.dfp_latestcompliancedate))
                .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.dfp_DriverId))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.bcgov_incident_bcgov_documenturl));
        }

        protected string TranslateDmerType(int? optionSetValue)
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
                case null:
                    result = "Suspected Medical Condition";
                    break;
                case 100000005:
                    result = "No DMER";
                    break;
            }
            return result;
        }
    }

    internal class CaseMapper :  IMapperAsync<incident, CaseDetail>
    {
        private readonly DynamicsContext _dynamicsContext;

        public CaseMapper(DynamicsContext dynamicsContext)
        {
            _dynamicsContext = dynamicsContext;
        }

        public async Task<CaseDetail> Map(incident @case)
        {
            var result = new CaseDetail();

            _dynamicsContext.LoadProperty(@case, nameof(incident.dfp_DriverId));
            result = new CaseDetail();
            result.CaseId = @case.incidentid.ToString();
            result.DriverId = @case.dfp_DriverId?.dfp_driverid.ToString() ?? string.Empty;
            result.Title = @case.title;
            result.IdCode = @case.ticketnumber;
            result.OpenedDate = @case.createdon.Value;
            result.LastActivityDate = @case.modifiedon.Value;
            result.LatestDecision = null;
            result.DecisionDate = null;
            result.Name = @case.dfp_DriverId?.dfp_fullname;
            result.DriverLicenseNumber = @case.dfp_DriverId?.dfp_licensenumber;
            result.LatestComplianceDate = @case.dfp_latestcompliancedate.GetValueOrDefault();
            // NOTE this is old code, now we get driver info from ICBC adapter, not Dynamics
            result.BirthDate = @case.dfp_DriverId?.dfp_dob;
            result.FirstName = @case.dfp_DriverId?.dfp_PersonId?.firstname;
            result.LastName = @case.dfp_DriverId?.dfp_PersonId?.lastname;
            result.MiddleName = @case.dfp_DriverId?.dfp_PersonId?.middlename;
            result.showOnPortals = @case.dfp_showonportals.GetValueOrDefault();
            result.IsRehab = @case.dfp_rehab.GetValueOrDefault();
            result.IsInterlock = @case.dfp_interlock.GetValueOrDefault();

            if(@case.prioritycode != null)
            {
                result.Priority = TranslatePriorityCode(@case.prioritycode);
            }

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

            _dynamicsContext.LoadProperty(@case, nameof(incident.stageid_processstage));

            var bpf = _dynamicsContext.dfp_dmfcasebusinessprocessflows.Where(x => x._bpf_incidentid_value == @case.incidentid).FirstOrDefault();

            if (bpf != null)
            {
                _dynamicsContext.LoadProperty(bpf, nameof(dfp_dmfcasebusinessprocessflow.activestageid));
                result.Status = bpf.activestageid?.stagename;
            }

            // case assignment
            if (@case._owningteam_value.HasValue)
            {
                await _dynamicsContext.LoadPropertyAsync(@case, nameof(incident.owningteam));
                result.AssigneeTitle = @case.owningteam.name;
                
            }

            // get the medical conditions.
            var medicalConditions = new List<MedicalCondition>();
            _dynamicsContext.LoadProperty(@case, nameof(incident.dfp_incident_dfp_knownmedicalcondition));
            if (@case.dfp_incident_dfp_knownmedicalcondition != null &&
                @case.dfp_incident_dfp_knownmedicalcondition.Count > 0)
            {
                foreach (var m in @case.dfp_incident_dfp_knownmedicalcondition)
                {
                    if (m._dfp_medicalconditionid_value != null &&
                        m.statecode == 0 &&
                        (m.statuscode == 1 || m.statuscode == 100000003))
                    {
                        var medicalCondition = await _dynamicsContext.dfp_medicalconditions
                            .ByKey(m._dfp_medicalconditionid_value.Value).GetValueAsync();
                        if (medicalCondition != null)
                        {
                            medicalConditions.Add(new MedicalCondition
                            {
                                Id = medicalCondition.dfp_id,
                                Description = medicalCondition.dfp_description ?? "Unknown",
                                FormId = medicalCondition.dfp_formid ?? string.Empty
                            });
                        }
                    }
                }
            }
            result.MedicalConditions = medicalConditions;

            // get the related decisions.
            _dynamicsContext.LoadProperty(@case, nameof(incident.dfp_incident_dfp_decision));
            if (@case.dfp_incident_dfp_decision != null && @case.dfp_incident_dfp_decision.Count > 0)
            {
                foreach (var decision in @case.dfp_incident_dfp_decision)
                {
                    if ((result.DecisionDate == null || decision.createdon > result.DecisionDate) && decision.statecode == 0)
                    {
                        result.LatestDecision = "";

                        _dynamicsContext.LoadProperty(decision, nameof(dfp_decision.dfp_OutcomeStatus));
                        if (decision.dfp_OutcomeStatus != null)
                        {
                            result.LatestDecision = decision.dfp_OutcomeStatus.dfp_name;
                        }

                        // now try and get the sub type
                        _dynamicsContext.LoadProperty(decision, nameof(dfp_decision.dfp_OutcomeSubStatus));
                        if (decision.dfp_OutcomeSubStatus != null)
                        {
                            result.LatestDecision += " - " + decision.dfp_OutcomeSubStatus.dfp_name;
                        }

                        result.DecisionDate = decision.createdon;
                        result.DecisionForClass = decision.dfp_eligibledriverlicenceclass;
                    }
                }
            }

            // Load Documents
            _dynamicsContext.LoadProperty(@case, nameof(incident.bcgov_incident_bcgov_documenturl));

            // Find the documents that are outstanding
            if (@case.bcgov_incident_bcgov_documenturl != null)
            {
                foreach (var document in @case.bcgov_incident_bcgov_documenturl)
                {
                    if (document.dfp_submittalstatus == 100000000)
                    {
                        result.OutstandingDocuments++;
                    }
                }
            }

            // owner
            _dynamicsContext.LoadProperty(@case, nameof(incident.owninguser));
            if (@case.owninguser != null && @case.owninguser.dfp_portalidentity != null)
            {
                switch (@case.owninguser.dfp_portalidentity.Value)
                {
                    case 100000000:
                        result.AssigneeTitle = "Client Services Group";
                        break;
                    case 100000001:
                        result.AssigneeTitle = "Case Adjudication Group";
                        break;
                    case 100000002:
                        result.AssigneeTitle = "Case Manager Group";
                        break;
                    case 100000003:
                        result.AssigneeTitle = "Other";
                        break;
                }
            }

            return result;
        }

        private string TranslateCaseTypeToString(int? optionSetValue)
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

       
        private string TranslatePriorityCode(int? optionSetValue)
        {
            var statusMap = new Dictionary<int, string>()
            {
                { 1, "Critical Review" },
                { 2, "Regular" },
                { 3, "Urgent" },
                { 4, "Expedited" },
            };

            if (optionSetValue != null && statusMap.ContainsKey(optionSetValue.Value))
            {
                return statusMap[optionSetValue.Value];
            }
            else
            {
                return "Regular";
            }
        }


        [Obsolete("Use TranslateDmerType instead")]
        private string TranslateDmerTypeRaw(int? optionSetValue)
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


    }
}
