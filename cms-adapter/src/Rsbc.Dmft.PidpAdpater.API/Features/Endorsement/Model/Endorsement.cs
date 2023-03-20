namespace MedicalPortal.API.Features.Endorsement.Model;
public class Endorsement
{
  
    public string HpDid { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<LicenceInformation> Licences { get; set; } = new();

    public class LicenceInformation
    {
        public string? IdentifierType { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusReasonCode { get; set; }
    }
}

