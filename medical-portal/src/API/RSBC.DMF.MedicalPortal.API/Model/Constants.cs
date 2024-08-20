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

    public static class LicenceStatusCode
    {
        public const string Active = "ACTIVE";
        public const string Inactive = "INACTIVE";
        public const string Terminated = "TERMINATED";
    }
}
