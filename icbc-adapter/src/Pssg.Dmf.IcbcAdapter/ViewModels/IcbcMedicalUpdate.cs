using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.IcbcAdapter.ViewModels
{
    public class IcbcMedicalUpdate
    {
        /// <summary>
        /// Driver's Licence Number 
        /// </summary>
        public string DlNumber { get; set; }

        /// <summary>
        /// schema name of the entity on which this document is uploaded
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// P or J
        /// </summary>
        public string MedicalDisposition { get; set; }

        /// <summary>
        /// Date issued
        /// </summary>
        public DateTimeOffset MedicalIssueDate { get; set; }

    }
}
