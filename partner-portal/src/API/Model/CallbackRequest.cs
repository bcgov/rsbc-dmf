namespace Rsbc.Dmf.PartnerPortal.Api;

public class CallbackRequest
{
    public string Phone { get; set; }
    public PreferredTime PreferredTime { get; set; }
    public string Subject { get; set; }
}

public enum PreferredTime
{
    Anytime,
    Morning,
    Evening
}