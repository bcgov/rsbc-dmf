namespace Rsbc.Dmf.CaseManagement.DomainModels
{
    // Dynamics schema name incident
    public class Case
    {
        // ticketnumber 
        // TODO Rename to IDCode
        public string CaseNumber { get; set; }

        // customerid_contact
        public Person Person { get; set; }
    }
}
