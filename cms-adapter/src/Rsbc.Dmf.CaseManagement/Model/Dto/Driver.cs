using System;

namespace Rsbc.Dmf.CaseManagement.Dto
{
    public class Driver
    {
        //public string FullName { get; set; }
        public string DriverLicenceNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public Person Person { get; set; }
    }
}
