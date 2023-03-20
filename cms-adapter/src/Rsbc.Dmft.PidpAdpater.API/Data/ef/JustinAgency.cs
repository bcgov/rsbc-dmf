using pdipadapter.Models;
using pdipadapter.Models.Lookups;

namespace pdipadapter.Data.ef;

[Table(nameof(JustinAgency))]
public class JustinAgency : BaseAuditable
{
    [Key]
    public long AgencyId { get; set; }
    [Required]
    public string Name { get;set; } = string.Empty;
    [Required]
    public string AgencyCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual ICollection<JustinUser> Users { get; set; } = new List<JustinUser>();    
    public ICollection<JustinAgencyAssignment> AgencyAssignments { get; set; } = new List<JustinAgencyAssignment>();    
  
}
public class AgencyDataGenerator : ILookupDataGenerator<JustinAgency>
{
    public IEnumerable<JustinAgency> Generate() => new[]
    {
                new JustinAgency {AgencyId = 1, Name = "Sannich Police Department", AgencyCode = "SPD"},
                 new JustinAgency {AgencyId = 2, Name = "Victoria Police Department", AgencyCode = "VICPD"},
                  new JustinAgency {AgencyId = 3, Name = "Delta Police Department", AgencyCode = "DPD"},
                   new JustinAgency {AgencyId = 4, Name = "Vancouver Police Department", AgencyCode = "VPD"},
                    new JustinAgency {AgencyId = 5, Name = "Royal Canada Mount Police", AgencyCode = "RCMP"},
    };
}