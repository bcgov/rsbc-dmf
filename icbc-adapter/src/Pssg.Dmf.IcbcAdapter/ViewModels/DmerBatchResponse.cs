using System.Collections.Generic;
using Newtonsoft.Json;
using Rsbc.Dmf.CaseManagement.Service;

namespace Rsbc.Dmf.IcbcAdapter.ViewModels
{
    /// <summary>DMER records and navigation metadata for the requested page.</summary>
    public sealed record DmerBatchResponse(
        [property: JsonProperty("items")] IReadOnlyList<DmerDocumentFile> Items,
        [property: JsonProperty("page_size")] int PageSize,
        [property: JsonProperty("nextLink", NullValueHandling = NullValueHandling.Include)] string NextLink);
}
