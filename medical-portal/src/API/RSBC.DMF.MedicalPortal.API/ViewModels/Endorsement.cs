namespace RSBC.DMF.MedicalPortal.API.ViewModels
{
    public class Endorsement
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public IEnumerable<Licence> Licence { get; set; }
    }

    public class Licence
    {
        public string IdentifierType { get; set; }
        public string StatusCode { get; set; }
        public string StatusReasonCode { get; set; }
    }
}
