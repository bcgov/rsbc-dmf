using System;

namespace Rsbc.Dmf.CaseManagement
{
    public class UpdateDocumentCommand
    {
        public Guid Id { get; set; }
        public int SubmittalStatus { get; set; }

        public string DpsPriority { get; set; }

        public string Queue { get; set; }
       
    }
}
