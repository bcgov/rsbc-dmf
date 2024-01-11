using System;
using System.Collections.Generic;
using System.Text;

namespace Rsbc.Dmf.CaseManagement
{
    public class BringForwardRequest
    {
        public string CaseId { get; set; }
        public string Assignee { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public BringForwardPriority? Priority { get; set; }

    }
}
