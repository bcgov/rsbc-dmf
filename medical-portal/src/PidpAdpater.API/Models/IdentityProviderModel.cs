

namespace pdipadapter.Models;

public class IdentityProviderModel : BaseAuditable
{
    public long Id { get; set; }
    public string IdentityProvider { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
