using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels
{
    public class RehabTrigger
    {
        public string RehabId { get; set; }
        public DateTimeOffset? AssignmentDate { get; set; }
        public DateTimeOffset? DecisionDate { get; set; }
        public string ClientType { get; set; }
        public string RehabActivity { get; set; }
        public string ClientPaid { get; set; }
        public string Stream { get; set; }
        public string Decision { get; set; }
    }
}
