using System;
using System.Collections.Generic;

namespace Rsbc.Dmf.CaseManagement.Dto
{
    // Dynamics schema name incident
    public class Case
    {
        // ticketnumber 
        // TODO Rename to IDCode
        public string CaseNumber { get; set; }

        // TODO rename DueDate
        // dfp_latestcompliancedate
        public DateTimeOffset? LatestComplianceDate { get; set; }

        // customerid_contact
        public Person Person { get; set; }

        // dfp_DriverId
        public Driver Driver { get; set; }

        // bcgov_incident_bcgov_documenturl
        public IEnumerable<Document> Documents { get; set; }

        public string DmerType { get; set; }
    }
}
