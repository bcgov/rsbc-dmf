using System.Collections.Generic;

public class ChefsSubmission
{
    public string Status { get; set; }
    // Use a dictionary to store all key-value pairs for submission data
    public Dictionary<string, object> Submission { get; set; }

    public ChefsSubmission()
    {
        Status = "DRAFT";
        Submission = new Dictionary<string, object>();
    }
}