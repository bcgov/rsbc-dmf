namespace Pssg.Interfaces.Icbc.ViewModels
{
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class DriverMasterStatus
    {
        /// <summary>
        /// Initializes a new instance of the DR1MST class.
        /// </summary>
        public DriverMasterStatus()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DR1MST class.
        /// </summary>
        public DriverMasterStatus(int? mSCD = default(int?), object? rSCD = default(object?), System.DateTime? rRDT = default(System.DateTime?), int? lNUM = default(int?), int? lCLS = default(int?), IList<DriverMedical> dR1MEDN = default(IList<DriverMedical>))
        {
            MasterStatusCode = mSCD;            
            LicenceExpiryDate = rRDT;
            LicenceNumber = lNUM;            
            LicenceClass = lCLS;
            DriverMedicals = dR1MEDN;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "MSCD")]
        public int? MasterStatusCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "RSCD")]
        public List<int> RestrictionCodes { get; set; }

        /// <summary>
        /// </summary>
        [JsonConverter(typeof(DateJsonConverter))]
        [JsonProperty(PropertyName = "RRDT")]
        public System.DateTime? LicenceExpiryDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "LNUM")]
        public int? LicenceNumber { get; set; }

        /// <summary>
        /// </summary>        
        public DriverStatus DriverStatus { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "LCLS")]
        public int? LicenceClass { get; set; }

        /// <summary>
        /// </summary>        
        public IList<DriverMedical> DriverMedicals { get; set; }

    }
}
