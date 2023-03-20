
namespace pdipadapter.Models;

public class RoleModel
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public ICollection<UserModel> Users { get; } = new List<UserModel>();
}
