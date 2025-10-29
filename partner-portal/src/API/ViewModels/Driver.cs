using Rsbc.Dmf.IcbcAdapter;

namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels
{
    public class Driver
    {
        public string Id { get; set; }
        public bool? Flag51 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DriverLicenseNumber{ get; set; }
        public bool? LoadedFromICBC { get; set; }
        public DateTimeOffset? MedicalIssueDate { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public string Sex { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Postal { get; set; }
        public string Country { get; set; }
        public string LicenceClass { get; set; }
        public string MasterStatusCode { get; set; }
        public DateTimeOffset? LicenceExpiryDate { get; set; }
        public List<string> RestrictionCodes { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string SecurityKeyword { get; set; }
        public List<DriverStatus> Status { get; set; }
        public List<DriverMedicals> Medicals { get; set; }
    }
}