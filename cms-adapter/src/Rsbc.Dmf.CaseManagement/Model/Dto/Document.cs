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
        public DateTimeOffset? ComplianceDate { get; set; }

        // bcgov_caseid
        public Case Case { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
      
        public string Description { get; set; }

        public string SubmittalStatus { get; set; }
        public DocumentType DocumentType { get; set; }

        public DocumentSubType DocumentSubType { get; set; }

        public string DocumentUrl { get; set; }
    }
}
