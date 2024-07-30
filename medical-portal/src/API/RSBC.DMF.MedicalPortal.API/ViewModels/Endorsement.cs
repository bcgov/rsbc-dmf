namespace RSBC.DMF.MedicalPortal.API.ViewModels
{
    public class Endorsement
    {
        // Pidp User Id (hpdid)
        public string UserId { get; set; }

        // cms loginId
        public Guid LoginId { get; set; }

        // Pidp data
        public string FirstName { get; set; }
        public string LastName { get; set; }
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
