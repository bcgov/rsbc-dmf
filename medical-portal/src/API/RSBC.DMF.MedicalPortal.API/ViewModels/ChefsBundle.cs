using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter;

namespace RSBC.DMF.MedicalPortal.API.ViewModels;

public class ChefsBundle
{
    public string caseId { get; set; }
    public PatientCase patientCase { get; set; }
    public Driver driverInfo { get; set; }
    public IEnumerable<MedicalCondition> medicalConditions { get; set; }
    public string dmerType { get; set; }
}