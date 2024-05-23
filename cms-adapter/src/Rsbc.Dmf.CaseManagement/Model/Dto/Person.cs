using System;

namespace Rsbc.Dmf.CaseManagement.DomainModels
{
    // Dynamics schema name contact
    public class Person
    {
        // fullname
        public string FullName { get; set; }

        // birthdate
        public DateTime? Birthday { get; set; }
    }
}
