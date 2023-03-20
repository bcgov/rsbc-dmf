using pdipadapter.Models;
using Microsoft.EntityFrameworkCore;

namespace pdipadapter.Data.ef;

#nullable disable

[Table(nameof(JustinUserRole))]
public class JustinUserRole : BaseAuditable
{
    [Key]
    public long UserRoleId { get; set; }
    public long UserId { get;set; }
    public long RoleId { get; set; }
    public bool? IsDisabled { get; set; }
    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(JustinUser.UserRoles))]
    public virtual JustinUser User { get; set; }
    [ForeignKey(nameof(RoleId))]
    [InverseProperty(nameof(JustinRole.UserRoles))]
    public virtual JustinRole Role { get; set; }
}