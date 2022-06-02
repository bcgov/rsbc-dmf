using System;
using System.Collections.Generic;

namespace Rsbc.Dmf.LegacyAdapter.ViewModels
{
    public class Case
    {
        public string CaseId { get; set; }

        public List<Document> Documents { get; set; }
    }
}
