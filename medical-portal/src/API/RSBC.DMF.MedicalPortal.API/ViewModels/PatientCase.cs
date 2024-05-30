using System;

public class PatientCase
{
    // Case
    public string CaseId { get; set; }
    public string DmerType { get; set; }
    public string Status { get; set; }

    public string IdCode { get; set; }

    public DateTime DueDate { get; set; } 

    public string DriverId { get; set; }


    // Patient
    public string Name { get; set; }
    public string DriverLicenseNumber { get; set; }
    public DateTime? BirthDate { get; set; }

    public string FirstName { get; set; }
    public string SurName { get; set; }
}
