using System;
using System.Collections.Generic;
using System.Text;

namespace Rsbc.Dmf.CaseManagement.Model.Dto
{
    public class SetDmerAIProccessedStatusRequest
    {
        public string dmerAiStatus { get; set; }
        public string documenturlId { get; set; }
    }
}
