namespace RSBC.DMF.MedicalPortal.API.ViewModels;

public enum SubmissionStatus
{
    Draft,
    Final
}

public class ChefsSubmission
{
    public SubmissionStatus Status { get; set; }

    // Use a dictionary to store all key-value pairs for submission data
    public Dictionary<string, object> Submission { get; set; }

    public ChefsSubmission()
    {
        Status = SubmissionStatus.Draft;
        Submission = new Dictionary<string, object>();
    }
}