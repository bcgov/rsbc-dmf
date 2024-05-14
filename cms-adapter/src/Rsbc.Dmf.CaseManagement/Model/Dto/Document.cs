using System;
using System.Collections.Generic;
using System.Text;

namespace Rsbc.Dmf.CaseManagement.DomainModels
{
    // Dynamics schema name bcgov_documenturl
    public class Document
    {
        // dfp_dmertype
        public int DmerType { get; set; }

        // dfp_dmerstatus
        public int DmerStatus { get; set; }

        // bcgov_caseid
        public Case Case { get; set; }
    }
}
