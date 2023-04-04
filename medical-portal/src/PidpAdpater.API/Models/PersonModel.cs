

namespace pdipadapter.Models;

public class PersonModel : BaseAuditable
{
    public int PersonId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public long UserId { get; set; }
    public UserModel User { get; set; } = new UserModel();

}
