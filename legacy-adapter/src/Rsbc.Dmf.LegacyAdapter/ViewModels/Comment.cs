using System.ComponentModel.DataAnnotations;

namespace Rsbc.Dmf.LegacyAdapter.ViewModels
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
        public string CommentText { get; set; }
        /// <summary>
        /// The Drivers License Number
        /// </summary>
        [Required]

        public ViewModels.Driver Driver { get; set; }

        /// <summary>
        /// Sequence Number
        /// </summary>
        [Required]
        public short SequenceNumber { get; set;}

        /// <summary>
        /// Comment Type Code - typically "W" for DFWEB
        /// </summary>
        [Required]
        public string CommentTypeCode { get; set;}

        /// <summary>
        /// The User ID for the user making the comment
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// GUID Case Id from the Case Management System
        /// </summary>
        [Required]
        public string CaseId { get; set; }
    }
}
