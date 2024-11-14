namespace Rsbc.Dmf.DriverPortal.ViewModels
{
    public class CaseDocuments
    {
        public CaseDocuments()
        {
            CaseSubmissions = new List<Document>();
            LettersToDriver = new List<Document>();
            SubmissionRequirements = new List<Document>();
        }

        public List<Document> SubmissionRequirements { get; set; }
        public List<Document> CaseSubmissions { get; set; }
        public List<Document> LettersToDriver { get; set; }
    }
}