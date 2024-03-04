using System;

namespace Rsbc.Dmf.LegacyAdapter.ViewModels
{
    public class Decision
    {
        public string CaseId { get; set; }
        public string DriverId { get; set; }
        public string OutcomeText { get; set; }
        public string SubOutcomeText { get; set; }
        public DateTimeOffset StatusDate { get; set; }
    }
}
