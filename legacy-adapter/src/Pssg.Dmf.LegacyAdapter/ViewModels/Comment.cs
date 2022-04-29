using System.ComponentModel.DataAnnotations;

namespace Pssg.Dmf.LegacyAdapter.ViewModels
{
    /// <summary>
    /// Container for a Comment object.  Used by DFWeb
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// The text body of the comment.
        /// </summary>
        [Required]
        public string commentText { get; set; }
        /// <summary>
        /// The Drivers License Number
        /// </summary>
        [Required]
        public string driversLicense { get; set; }

        /// <summary>
        /// Sequence Number
        /// </summary>
        [Required]
        public short sequenceNumber { get; set;}

        /// <summary>
        /// Comment Type Code - typically "W" for DFWEB
        /// </summary>
        [Required]
        public string commentTypeCode { get; set;}

        /// <summary>
        /// The User ID for the user making the comment
        /// </summary>
        [Required]
        public string userId { get; set; }

        /// <summary>
        /// GUID Case Id from the Case Management System
        /// </summary>
        [Required]
        public string caseId { get; set; }
    }
}
