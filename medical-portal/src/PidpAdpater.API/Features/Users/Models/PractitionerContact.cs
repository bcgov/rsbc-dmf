using Microsoft.OData.Edm;

namespace PidpAdapter.API.Features.Users.Models;
public class PractitionerContact
{
    public string ContactId { get; set; } = string.Empty;
    public string MedicalPractictionerId { get; set; } = string.Empty;
}
public class PractitionerContactResponse
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime? Birthdate { get; set; }
}
