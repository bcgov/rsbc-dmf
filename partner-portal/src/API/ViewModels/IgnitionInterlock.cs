using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels
{
    public class IgnitionInterlock
    {
        public string IgnitionId { get; set; }
        public string IIActivity { get; set; }
        public string TermMonths { get; set; }
        public DateTimeOffset? InstallDate { get; set; }
        public DateTimeOffset? CompletionDate { get; set; }
        public string ClientPaid { get; set; }
    }
}
