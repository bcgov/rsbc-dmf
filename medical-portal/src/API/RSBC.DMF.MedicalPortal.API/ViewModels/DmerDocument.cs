namespace RSBC.DMF.MedicalPortal.API.ViewModels
{
    public class DmerDocument
    {
        public string DmerType { get; set; }
       
        public string DmerStatus { get; set; }

        public string DocumentId { get; set; }

        public string FullName { get; set; }

        public string LoginId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string CaseId { get; set; }
        public string IdCode { get; set; }
        public DateTimeOffset? DueDate { get; set; }

        public DateTimeOffset? BirthDate { get; set; }
    }
}
