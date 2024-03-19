using static Rsbc.Dmf.CaseManagement.Service.Callback.Types;

namespace Rsbc.Dmf.DriverPortal.ViewModels
{
    public class Callback
    {
        public Guid Id { get; set; }
        public DateTimeOffset RequestCallback { get; set; }
        public CallbackTopic Topic { get; set; }
        public string Topic { get; set; }
        public int CallStatus { get; set; }
        public DateTimeOffset Closed { get; set; }
    }
}