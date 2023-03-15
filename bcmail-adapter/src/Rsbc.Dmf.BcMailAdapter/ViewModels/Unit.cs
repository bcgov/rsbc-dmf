using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rsbc.Dmf.BcMailAdapter.ViewModels
{
    /// <summary>
    /// Unit 
    /// </summary>
    public enum Unit
    {
        /// <summary>
        /// Milimeters
        /// </summary>
        [EnumMember(Value = "mm")]
        Milimeters,

        /// <summary>
        /// Centimeters
        /// </summary>
        [EnumMember(Value = "cm")]
        Centimeters,

        /// <summary>
        /// Inches
        /// </summary>
        [EnumMember(Value = "in")]
        Inches,

    }
}
