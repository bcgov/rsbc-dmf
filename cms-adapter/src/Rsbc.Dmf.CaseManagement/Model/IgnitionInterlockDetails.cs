using System;
using System.Collections.Generic;
using System.Text;

namespace Rsbc.Dmf.CaseManagement.Model
{
    public class IgnitionInterlockDetails
    {
        public string IIActivity { get; set; }
        public string TermMonths { get; set; }
        public DateTimeOffset? InstallDate { get; set; }
        public DateTimeOffset? CompletionDate { get; set; }
        public string ClientPaid { get; set; }
    }
}
