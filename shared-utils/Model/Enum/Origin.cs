using System.ComponentModel;

namespace SharedUtils.Model.Enum
{
    public enum Origin
    {
        [Description("Migration")]
        Migration = 100000015,

       [Description("Mercury Uploaded RSBC")]
        MercuryUploadedRSBC = 100000014,

       [Description("Practitioner Portal")]
        PractitionerPortal = 100000000,

        [Description("Medical Portal")]
        DriverPortal = 100000016,

        [Description("DPS/KOFAX")]
        DPSKofax = 100000017,

        [Description("Partner Portal")]
        PartnerPortal = 100000001,
    }
}
