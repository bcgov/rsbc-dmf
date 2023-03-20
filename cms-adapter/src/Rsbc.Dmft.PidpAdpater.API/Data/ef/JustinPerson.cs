using pdipadapter.Models;

#nullable disable

namespace pdipadapter.Data.ef;

[Table(nameof(JustinPerson))]
public partial class JustinPerson : BaseAuditable
{
    [Key]
    public long PersonId { get;set; }
    [Required]
    //[Column("SURNAME")]
    [StringLength(50)]
    public string Surname { get; set; }
    [Required]
    //[Column("FIRST_NAME")]
    [StringLength(50)]
    public string FirstName { get; set; }
    //[Column("MIDDLE_NAMES")]
    [StringLength(200)]
    public string MiddleNames { get; set; }
    //[Column("NAME_SUFFIX")]
    [StringLength(50)]
    public string NameSuffix { get; set; }
    //[Column("PREFERRED_NAME")]
    [StringLength(200)]
    public string PreferredName { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    public string? Gender { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Phone { get; set; }
    //[Column("COMMENT")]
    [StringLength(2000)]
    public string Comment { get; set; }
    //[Column("ADDRESS_COMMENT")]
    [StringLength(2000)]
    public string AddressComment { get; set; }
    [Required]
    //[Column("IS_DISABLED")]
    public bool? IsDisabled { get; set; }
    public virtual JustinAddress Address { get; set; }
    [InverseProperty(nameof(JustinUser.Person))]
    public virtual JustinUser User { get; set; }
}