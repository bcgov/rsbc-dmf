namespace PidpAdapter.Endorsement.Model;

public class EndorsementData
{
    public class Model
    {
        public string? Hpdid { get; set; } = string.Empty;
        public List<LicenceInformation> Licences { get; set; } = new();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public class LicenceInformation
        {
            public string? IdentifierType { get; set; }
            public string? StatusCode { get; set; }
            public string? StatusReasonCode { get; set; }
        }
    }
}
