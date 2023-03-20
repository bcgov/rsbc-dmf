namespace pdipadapter.Data.ef;

[Table(nameof(JustinAgencyAssignment))]
public class JustinAgencyAssignment
{
    [Key]
    public long AgencyAssignmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long? AgencyId { get; set; }

    public virtual JustinAgency Agency { get; set; } = new JustinAgency();
}