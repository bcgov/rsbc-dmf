namespace MedicalPortal.API.Features.Endorsement.Model;
public class Endorsement : EndorsementData.Model
{
    public string ContactId { get; set; }  = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? BirthDate { get; set; }
    public string Role { get; set; } = string.Empty;
}

