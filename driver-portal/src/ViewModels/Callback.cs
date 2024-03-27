using static Rsbc.Dmf.CaseManagement.Service.Callback.Types;

namespace Rsbc.Dmf.DriverPortal.ViewModels
{
    public class Callback
    {
        public Guid Id { get; set; }
        public DateTimeOffset RequestCallback { get; set; }
        public string Topic { get; set; }
        public CallbackCallStatus CallStatus { get; set; }
        public DateTimeOffset Closed { get; set; }
        public string Phone { get; set; }
        public PreferredTime PreferredTime { get; set; }
    }
}