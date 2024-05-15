namespace Rsbc.Dmf.CaseManagement.DomainModels
{
    // Dynamics schema name incident
    public class Case
    {
        // ticketnumber
        public string CaseNumber { get; set; }

        // customerid_contact
        public Person Person { get; set; }
    }
}
