using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

    [JsonProperty("submission")] public Dictionary<string, object> Submission { get; set; } = new();
}