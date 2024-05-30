using static MedicalPortal.API.Features.Endorsement.Model.EndorsementData.Model;

namespace MedicalPortal.API.Features.Endorsement.Model;
public class Endorsement
{
    public string? Hpdid { get; set; } = string.Empty;
    public List<LicenceInformation> Licences { get; set; } = new();
    public Contact Contact { get; set; } = new Contact();
}
public class Contact
{
    public string ContactId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? BirthDate { get; set; }
    public string Role { get; set; } = string.Empty;
}
