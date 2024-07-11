using System.ComponentModel;

namespace RSBC.DMF.MedicalPortal.API
{
    public class Constants
    {
        public const string CorsPolicy = "CorsPolicy";
    }

    // NOTE matches cms-adapter LoginType
    public enum ExternalSystem
    {
        [Description("bcsc")]
        Bcsc,

        [Description("bceid")]
        BceId,

        [Description("idir")]
        Idir
    }
}
