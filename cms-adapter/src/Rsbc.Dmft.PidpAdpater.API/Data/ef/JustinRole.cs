global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
using pdipadapter.Models;
using pdipadapter.Models.Lookups;

namespace pdipadapter.Data.ef;

#nullable disable

[Table(nameof(JustinRole))]
public class JustinRole : BaseAuditable
{
    [Key]
    public long RoleId { get; set; }
    [Required]
    //[Column("NAME")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    //[Column("DESCRIPTION")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    [Required]
    //[Column("IS_PUBLIC")]
    public bool? IsPublic { get; set; }
    [Required]
    //[Column("IS_DISABLED")]
    public bool? IsDisabled { get; set; }
    [InverseProperty(nameof(JustinUserRole.Role))]
    public virtual ICollection<JustinUserRole> UserRoles { get; set; }
}
public enum RoleCode
{
    Administrator = 1,
    BCPS = 2,
    Defence = 3,
    Police = 4,
    Accused = 5,
    OutofCustody = 6
}
public class RoleCodeDataGenerator : ILookupDataGenerator<JustinRole>
{
    public IEnumerable<JustinRole> Generate() => new[]
    {
        new JustinRole {RoleId = 1, Name = "Administrator", Description = "Super Users", IsDisabled = false, IsPublic = false },
        new JustinRole {RoleId = 2, Name = "BCPS", Description = "BCPS Users", IsDisabled = false, IsPublic = false },
        new JustinRole {RoleId = 3, Name = "Defence Council", Description = "Defence Users", IsDisabled = false, IsPublic = false },
        new JustinRole {RoleId = 4, Name = "Police", Description = "Police Users", IsDisabled = false, IsPublic = false },
        new JustinRole {RoleId = 5, Name = "Accused", Description = "Accused Users", IsDisabled = false, IsPublic = false },
        new JustinRole {RoleId = 6, Name = "OutofCustody", Description = "OutofCustody Users", IsDisabled = false, IsPublic = false }
    };

}