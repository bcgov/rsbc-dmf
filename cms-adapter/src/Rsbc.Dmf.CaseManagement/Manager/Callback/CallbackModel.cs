using System;
using System.ComponentModel;

namespace Rsbc.Dmf.CaseManagement
{
    public enum CallbackCallStatus
    {
        Open = 0,   // Mapped to EntityState.Active
        Closed = 1  // Mapped to EntityState.Inactive
    }

    public class Callback
    {
        public Guid? Id { get; set; }
        public DateTimeOffset RequestCallback { get; set; }
        public CallbackCallStatus CallStatus { get; set; }
        public DateTimeOffset? Closed { get; set; }
        public string Phone { get; set; }
        public PreferredTime PreferredTime { get; set; }
        public bool NotifyByMail { get; set; }
        public bool NotifyByEmail { get; set; }
        // TODO change type to Guid
        public string CaseId { get; set; }
        public string Assignee { get; set; }

        [Description("Topic")]
        public string Subject { get; set; }

        public string Description { get; }
        public CallbackPriority? Priority { get; set; }
        public int? Origin { get; set; }
    }

    public enum PreferredTime
    {
        Anytime,
        Morning,
        Evening
    }
}
