using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Rsbc.Dmf.IcbcAdapter.ViewModels
{
    /// <summary>Request to create a case for an existing DMER document.</summary>
    public sealed class AddDmerCaseRequest
    {
        [Required]
        [JsonProperty("case_type")]
        public string CaseType { get; init; }

        /// <summary>Currently ignored; the case is assigned to the default DMER owner.</summary>
        [JsonProperty("owner")]
        public string Owner { get; init; }

        [Required]
        [JsonProperty("driver")]
        public AddDmerCaseDriver Driver { get; init; }

        [Required]
        [JsonProperty("document_type")]
        public string DocumentType { get; init; }

        /// <summary>Currently ignored.</summary>
        [JsonProperty("comments")]
        public string Comments { get; init; }
    }

    public sealed class AddDmerCaseDriver
    {
        /// <summary>Driver licence number.</summary>
        [Required]
        [JsonProperty("driver_id")]
        public string DriverId { get; init; }
    }

    public sealed record AddDmerCaseResponse(
        [property: JsonProperty("caseId")] string CaseId,
        [property: JsonProperty("documentRecordId")] string DocumentRecordId,
        [property: JsonProperty("status")] string Status);
}
