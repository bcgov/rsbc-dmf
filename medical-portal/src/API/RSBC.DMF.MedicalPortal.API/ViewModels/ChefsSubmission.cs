using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RSBC.DMF.MedicalPortal.API.ViewModels;

public enum SubmissionStatus
{
    Draft,
    Final
}

public class ChefsSubmission
{
    [JsonProperty("status")]
    [JsonConverter(typeof(StringEnumConverter))]
    public SubmissionStatus Status { get; set; }

    [JsonProperty("submission")]
    public Dictionary<string, object> Submission { get; set; }

    public ChefsSubmission()
    {
        Status = SubmissionStatus.Draft;
        Submission = new Dictionary<string, object>();
    }
}