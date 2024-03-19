using System.ComponentModel;

namespace Rsbc.Dmf.CaseManagement
{
    public class CallbackRequest3
    {
        public string CaseId { get; set; }
        public string Assignee { get; set; }

        [Description("Topic")]
        public string Subject { get; set; }

        public string Description { get; set; }
        public CallbackPriority? Priority { get; set; }
        public CallbackCallStatus CallStatus { get; set; }
        public string Phone { get; set; }
        public PreferredTime PreferredTime { get; set; }
        public bool NotifyByMail { get; set; }
        public bool NotifyByEmail { get; set; }
        public int? Origin { get; set; }
    }
}
