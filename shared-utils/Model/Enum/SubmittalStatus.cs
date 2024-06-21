using System;
using System.ComponentModel;

namespace Pssg.SharedUtils
{
    // NOTE this should be in a shared library with cms-adapter, so they can both use this enum
    // after moved, remove Enums.NET package from driver-portal project
    public enum SubmittalStatus
    {
        [Description("Empty")]
        Empty = 100000013,

        [Description("Issued")]
        Issued = 100000011,

        [Description("Actioned Non-comply")]
        ActionedNoncomply = 100000007,

        [Description("Under Review")]
        UnderReview = 100000002,

        [Description("Reviewed")]
        Reviewed = 100000003,

        [Description("Non-Comply")]
        Noncomply = 100000005,
        
        [Description("Carry Forward")]
        CarryForward = 100000006,

        [Description("Received")]   // Accept
        Received = 100000001,

        [Description("Reject")]
        Rejected = 100000004,

        [Description("Clean Pass")]
        CleanPass = 100000009,

        [Description("Manual Pass")]
        ManualPass = 100000012,

        [Description("Open-Required")]
        OpenRequired = 100000000,

        [Description("Uploaded")]
        Uploaded = 100000010,

        [Description("Sent")]
        Sent = 100000008
    }
}
