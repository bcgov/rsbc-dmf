using System.ComponentModel;

namespace SharedUtils.Model.Enum
{
    public enum Origin
    {
        [Description("Practitioner Portal")]
        PractitionerPortal,

        [Description("Partner Portal")]
        PartnerPortal,

        [Description("Mercury Uploaded RSBC")]
        MercuryUploadedRSBC,

        [Description("Migration")]
        Migration,

        [Description("Medical Portal")]
        DriverPortal,

        [Description("DPS/KOFAX")]
        DPSKofax,
    }
}
