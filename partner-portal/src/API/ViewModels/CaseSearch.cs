using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels
{
    public class CaseSearch
    {
        /// <summary>
        /// The GUID that is the primary key for this case, sent as a string
        /// </summary>
        public string CaseId { get; set; }

        /// <summary>
        /// The GUID that is the primary key for the driver, sent as a string
        /// </summary>
        public string DriverId { get; set; }
        /// <summary>
        /// Indicates system title of the case
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Indicates the Code which will be used for eDMERs, eg H2M0L1
        /// </summary>
        public string IdCode { get; set; }

        // Driver Details
        
        public string DriverLicenseNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
