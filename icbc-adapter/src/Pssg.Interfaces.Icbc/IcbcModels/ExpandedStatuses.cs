using System;

namespace Pssg.Interfaces.IcbcModels
{
    public class ExpandedStatuses
    {
        public string StatusSection { get; set; }
        public string MasterStatus { get; set; }
        public string ExpandedStatus { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ReviewDate { get; set; }
        public string StatusDescription { get; set; }
    }
}