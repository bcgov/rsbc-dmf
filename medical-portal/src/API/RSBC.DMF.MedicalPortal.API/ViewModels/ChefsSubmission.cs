using Newtonsoft.Json;

namespace RSBC.DMF.MedicalPortal.API.ViewModels;

public static class SubmissionStatus
{
    public const string Draft = "Draft";
    public const string Final = "Final";
}

public class ChefsSubmission
{
    [JsonProperty("status")] 
    public string Status { get; set; } = SubmissionStatus.Draft;

    [JsonProperty("submission")] 
    public Dictionary<string, object> Submission { get; set; } = new();

    [JsonProperty("flags")] 
    public Dictionary<string, object> Flags { get; set; } = new();

    [JsonProperty("priority")] 
    public string Priority { get; set; } = new("");

    [JsonProperty("assign")] 
    public string Assign { get; set; } = new("");
}