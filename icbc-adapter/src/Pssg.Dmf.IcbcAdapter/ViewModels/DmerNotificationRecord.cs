namespace Rsbc.Dmf.IcbcAdapter.ViewModels
{
    // DMETEXT fixed-width record fields.
    public class DmerNotificationRecord
    {
        public string Lnum { get; set; }
        public string Clno { get; set; }
        public string Surname { get; set; }
        public string MedicalIssueDate { get; set; }
        public string MedicalType { get; set; }
        public string Sex { get; set; }
        public string BirthDate { get; set; }
        public string LicenceExpiryDate { get; set; }
        public string LastMedicalDate { get; set; }
        public string LastExamDate { get; set; }
        public string AddressDocumentDate { get; set; }
        public string MasterStatusCode { get; set; }
        public string LicenceClass { get; set; }
    }
}
