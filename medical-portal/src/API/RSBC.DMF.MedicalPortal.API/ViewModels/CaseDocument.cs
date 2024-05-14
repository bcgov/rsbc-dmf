namespace RSBC.DMF.MedicalPortal.API.ViewModels
{
    public class CaseDocument
    {
        public int DmerType { get; set; }
        public int DmerStatus { get; set; }
        public string CaseNumber { get; set; }
        public string FullName { get; set; }
        public string Birthday { get; set; }
        // TODO forgot submission date
    }
}
