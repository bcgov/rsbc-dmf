namespace RSBC.DMF.MedicalPortal.API.ViewModels
{
    public class Endorsement
    {
        public string UserId { get; set; }
        public IEnumerable<Licence> Licence { get; set; }
    }

    public class Licence
    {
        public string IdentifierType { get; set; }
        public string StatusCode { get; set; }
        public string StatusReasonCode { get; set; }
    }
}
