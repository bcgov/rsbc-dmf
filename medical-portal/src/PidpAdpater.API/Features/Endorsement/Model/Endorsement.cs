using static MedicalPortal.API.Features.Endorsement.Model.EndorsementData.Model;

namespace MedicalPortal.API.Features.Endorsement.Model;

public class Endorsement
{
    public string? Hpdid { get; set; } = string.Empty;
    public List<LicenceInformation> Licences { get; set; } = new();
}

