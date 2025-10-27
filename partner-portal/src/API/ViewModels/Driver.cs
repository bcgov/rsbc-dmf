namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels
{
    public class Driver
    {
        public string Id { get; set; }

        public bool? Flag51 { get; set; }

        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// License Number
        /// </summary>
        public string DriverLicenseNumber{ get; set; }

        /// <summary>
        /// True if loaded from ICBC
        /// </summary>
        public bool? LoadedFromICBC { get; set; }

        /// <summary>
        /// The date this particular case had the medical issue date.
        /// </summary>
        public DateTimeOffset? MedicalIssueDate { get; set; }

        /// <summary>
        /// Birth Date
        /// </summary>
        public string BirthDate { get; set; }

        /// <summary>
        /// Gender
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// Address Line 1
        /// </summary>
        public string AddressLine1 { get; set; }
        
        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Province
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// Postal Code
        /// </summary>
        public string Postal { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Licence Class
        /// </summary>
        public string LicenceClass { get; set; }


    }
}