using System;
using System.Collections.Generic;

namespace Rsbc.Dmf.IcbcAdapter.IcbcModels
{
    public class DriverDetails
    {
        public int LicenceNumber { get; set; }
        public DateTime LicenceExpiryDate { get; set; }
        public int LicenceClass { get; set; }
        public int MasterStatusCode { get; set; }
        public List<Restrictions> Restrictions { get; set; }
        public List<ExpandedStatuses> ExpandedStatuses { get; set; }
    }
}

