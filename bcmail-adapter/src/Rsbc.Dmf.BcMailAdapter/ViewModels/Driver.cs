using System;

namespace Rsbc.Dmf.BcMailAdapter.ViewModels
{
    public class Driver
    {
        public bool? Flag51 { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// License Number
        /// </summary>
        public string LicenseNumber { get; set; }

        /// <summary>
        /// True if loaded from ICBC
        /// </summary>
        public bool? LoadedFromICBC { get; set; }

        /// <summary>
        /// The date this particular case had the medical issue date.
        /// </summary>
        public DateTimeOffset? MedicalIssueDate { get; set; }
    }
}
