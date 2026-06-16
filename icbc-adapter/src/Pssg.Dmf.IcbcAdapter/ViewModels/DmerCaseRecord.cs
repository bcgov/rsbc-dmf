using System;

namespace Rsbc.Dmf.IcbcAdapter.ViewModels
{
    public class DmerCaseRecord
    {
        public string DriverLicenseNumber { get; set; }
        public string CaseTypeCode { get; set; }
        public string TriggerType { get; set; }
        public string Owner { get; set; }
        public DateTime DriverDateOfBirthUtc { get; set; }
        public string DriverSurname { get; set; }
    }
}
