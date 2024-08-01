namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels;

public class Callback
{
    public Guid Id { get; set; }
    public DateTimeOffset RequestCallback { get; set; }
    public string Topic { get; set; }
    public CallStatus CallStatus { get; set; }
    public DateTimeOffset Closed { get; set; }
    public string Phone { get; set; }
    public Api.PreferredTime PreferredTime { get; set; }
}

public enum CallStatus
{
    Open = 0,
    Closed = 1
}
