namespace MedicalPortal.API.Features.Endorsement.Model;
public class EndorsementData
{
    public class Model
    {
        public string? Hpdid { get; set; } = string.Empty;
        public List<LicenceInformation> Licences { get; set; } = new();

        public class LicenceInformation
        {
            public string? IdentifierType { get; set; }
            public string? StatusCode { get; set; }
            public string? StatusReasonCode { get; set; }
        }
    }
}
