using static OneHealthAdapter.Endorsement.Model.EndorsementData.Model;

namespace OneHealthAdapter.Endorsement.Model;

public class Endorsement
{
    public string? Hpdid { get; set; } = string.Empty;
    public List<LicenceInformation> Licences { get; set; } = new();
}
