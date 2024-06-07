using static PidpAdapter.Endorsement.Model.EndorsementData.Model;

namespace PidpAdapter.Endorsement.Model;

public class Endorsement
{
    public string? Hpdid { get; set; } = string.Empty;
    public List<LicenceInformation> Licences { get; set; } = new();
}
