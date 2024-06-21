using System;

namespace Rsbc.Dmf.CaseManagement.Dto
{
    // Dynamics schema name contact
    public class Person
    {
        // fullname
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        // firstname
        public string FirstName { get; set; }

        // lastname
        public string LastName { get; set; }

        // birthdate
        public DateTime? Birthday { get; set; }
    }
}
