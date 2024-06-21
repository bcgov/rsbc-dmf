using Rsbc.Dmf.IcbcAdapter;

namespace RSBC.DMF.MedicalPortal.API.ViewModels;

public class ChefsBundle
{
    public string caseId { get; set; }
    public PatientCase patientCase { get; set; }
    public DriverInfoReply driverInfoReply { get; set; }
}