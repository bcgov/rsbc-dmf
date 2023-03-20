using pdipadapter.Models;

#nullable disable

namespace pdipadapter.Data.ef;

[Table(nameof(JustinUser))]
public class JustinUser : BaseAuditable
{
    //public JustinUser()
    //{
    //    //PartyType = new JustinPartyType();
    //    UserRoles = new HashSet<JustinUserRole>();
    //    //Agency = new JustinAgency();
    //}
    [Key]
    public long UserId { get; set; }
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public long ParticipantId { get; set; }
    public Guid? DigitalIdentifier { get; set; } = default;
    public long AgencyId { get; set; }
    public long PersonId { get; set; }
    [Required]
    public bool IsDisabled { get; set; }
    public long? IdentityProviderId {get;set;}
    [ForeignKey(nameof(PersonId))]
    [InverseProperty(nameof(JustinPerson.User))]
    public virtual JustinPerson Person { get; set; } //= new JustinPerson();
    public virtual JustinPartyType PartyType { get; set; } //= new JustinPartyType();
    //public PartyTypeCode PartyTypeCode { get; set; }
    [InverseProperty(nameof(JustinUserRole.User))]
    public virtual ICollection<JustinUserRole> UserRoles { get; set; } //= new HashSet<JustinUserRole>();
    public virtual JustinIdentityProvider IdentityProvider { get; set; } 
    public virtual JustinAgency Agency { get; set; }// = new JustinAgency();


}
