using System.ComponentModel.DataAnnotations;

namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels
{
    public class Comment
    {
        /// <summary>
        /// The text body of the comment.
        /// </summary
        public string CommentText { get; set; }

        /// <summary>
        /// The Drivers License Number
        /// </summary>
        public Driver Driver { get; set; }

        /// <summary>
        /// Sequence Number
        /// </summary>
        public long? SequenceNumber { get; set; }

        /// <summary>
        /// Comment Type Code - typically "W" for DFWEB
        /// </summary>
        public string CommentTypeCode { get; set; }

        /// <summary>
        /// The User ID for the user making the comment
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// GUID Case Id from the Case Management System.  If this is empty the case will be created
        /// </summary>        
        public string CaseId { get; set; }

        /// <summary>
        /// GUID Comment Id from the Case Management System
        /// </summary>
        public string CommentId { get; set; }

        /// <summary>
        /// Date the comment was made
        /// </summary>
        public DateTimeOffset? CommentDate { get; set; }

        public string Origin { get; set; }
    }
}


