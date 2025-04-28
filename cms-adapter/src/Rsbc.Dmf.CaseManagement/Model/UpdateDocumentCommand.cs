using System;

namespace Rsbc.Dmf.CaseManagement
{
    public class UpdateDocumentCommand
    {
        public Guid Id { get; set; }
        public int SubmittalStatus { get; set; }
        //public string DocumentType { get; set; }
    }
}
