using static Rsbc.Dmf.CaseManagement.Service.Callback.Types;

namespace Rsbc.Dmf.DriverPortal.ViewModels;

public class Callback
{
    public Guid Id { get; set; }
    public DateTimeOffset RequestCallback { get; set; }

    public string Subject { get; set; }

    public string Description { get; set; }

    // TODO should not be using proto enum in view models
    public CallbackCallStatus CallStatus { get; set; }
    public DateTimeOffset Closed { get; set; }

    //public string Phone { get; set; }
    public Api.PreferredTime PreferredTime { get; set; }
}