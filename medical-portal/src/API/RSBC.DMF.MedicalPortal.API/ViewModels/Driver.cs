using Rsbc.Dmf.CaseManagement.Service;

namespace RSBC.DMF.MedicalPortal.API.ViewModels
{
    public class Driver
    {
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public double Weight { get; set; }
        public string Sex { get; set; }
        public string BirthDate { get; set; }
        public double Height { get; set; }
        public Address Address { get; set; }
        public string DriverLicenceNumber { get; set; }
        public int LicenceClass { get; set; }
    }
}