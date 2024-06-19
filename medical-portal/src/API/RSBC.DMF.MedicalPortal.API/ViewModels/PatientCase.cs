using System;

namespace RSBC.DMF.MedicalPortal.API.ViewModels;

public class PatientCase
{
    // Case
    public string CaseId { get; set; }
    public string DmerType { get; set; }
    public string Status { get; set; }

    public string IdCode { get; set; }

    //public DateTime DueDate { get; set; } 

    public string DriverId { get; set; }

    public DateTimeOffset? LatestComplianceDate { get; set; }


    // Patient
    public string Name { get; set; }
    public string DriverLicenseNumber { get; set; }
    public DateTimeOffset? BirthDate { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string MiddleName { get; set; }


    // Documents
    public IEnumerable<Document> Documents { get; set; }
}

public class Document
{
    public string DmerType { get; set; }
    public string DmerStatus { get; set; }
}
