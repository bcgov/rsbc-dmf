namespace Rsbc.Dmf.DriverPortal.Api
{
    public class CallbackRequest
    {
        public string CaseId { get; set; }
        public int Origin { get; set; }
        public int Priority { get; set; }
        public DateTimeOffset RequestCallback { get; set; }
    }
}
