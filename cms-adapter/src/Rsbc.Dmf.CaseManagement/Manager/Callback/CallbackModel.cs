using System;

namespace Rsbc.Dmf.CaseManagement
{
    public enum CallbackTopic
    {
        View = 0,
        Upload = 1
    }

    public enum CallbackCallStatus
    {
        Open = 0,
        Closed = 1
    }

    public class Callback
    {
        public Guid Id { get; set; }
        public DateTimeOffset RequestCallback { get; set; }
        public CallbackTopic Topic { get; set; }
        public CallbackCallStatus CallStatus { get; set; }
        public DateTimeOffset Closed { get; set; }
    }
}
