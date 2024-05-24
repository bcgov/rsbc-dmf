using System;

namespace Rsbc.Dmf.CaseManagement.DomainModels
{
    // Dynamics schema name bcgov_documenturl
    public class Document
    {
        // dfp_dmertype
        public int DmerType { get; set; }

        // dfp_dmerstatus
        public int DmerStatus { get; set; }

        // dfp_compliancedate
        public DateTime? ComplianceDate { get; set; }

        // bcgov_caseid
        public Case Case { get; set; }

        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public string Description { get; set; }

        public string SubmittalStatus { get; set; }
        public string DocumentType { get; set; }
        public string DocumentSubType { get; set; }
        public string DocumentUrl { get; set; }
    }
}
