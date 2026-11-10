namespace Rsbc.Dmf.CaseManagement.Dto
{
    public class AddDriverToAiDmerDocumentReply
    {
        public bool Success { get; set; }
        public string ReplyMessage { get; set; }
        public DriverOpenCase? DriverOpenCase { get; set; }
    }

    public class DriverOpenCase
    {
        public string CaseId { get; set; }
        public string CaseTitle { get; set; }
        public string CaseType { get; set; }
        public string CasePriority { get; set; }
        public string CaseOwner { get; set; }
        public string CaseStatus { get; set; }
    }
}
