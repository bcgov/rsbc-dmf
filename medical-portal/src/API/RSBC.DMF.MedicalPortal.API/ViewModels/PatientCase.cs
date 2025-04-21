namespace RSBC.DMF.MedicalPortal.API.ViewModels;

public class PatientCase
{
    // Case
    public string CaseId { get; set; }
    public string IdCode { get; set; }
    public DateTime DueDate { get; set; } 
    public string DriverId { get; set; }
    // TODO remove and use DueDate instead
    public DateTimeOffset? LatestComplianceDate { get; set; }

    //Indicates the current stage of case
    public string Status { get; set; }

    // Indicated the date the case was opened.
    public DateTimeOffset? OpenedDate { get; set; }

    // Patient
    public string Name { get; set; }
    public string DriverLicenseNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }

    // Document
    public string DmerType { get; set; }
   
    public string DmerStatus { get; set; }
    public bool IsOwner { get; set; }

    public string DocumentId { get; set; }

    // Claimed user Id

    public string ClaimedUserId { get; set; }
}
