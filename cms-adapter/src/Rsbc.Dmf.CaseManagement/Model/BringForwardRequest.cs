using System;


namespace Rsbc.Dmf.CaseManagement
{
    [Obsolete("Use CallbackRequest instead")]
    public class BringForwardRequest
    {
        public string CaseId { get; set; }
        public string Assignee { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public CallbackPriority? Priority { get; set; }
    }
}
