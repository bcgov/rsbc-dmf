using System;

namespace Rsbc.Dmf.CaseManagement.Dto
{
    public class DmerDriverRequest
    {
        public string DriverLicenceNumber { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Comments { get; set; }
        public string DmerId { get; set; }
    }
}
