using System;
using System.ComponentModel.DataAnnotations;

namespace Rsbc.Dmf.LegacyAdapter.ViewModels
{
    /// <summary>
    /// Container for a Document object.  Used by DFCMS and DPS
    /// </summary>
    public class Document
    {
        /// <summary>
        /// The file contents
        /// </summary>
        [Required]
        public byte[] FileContents { get; set; }
        /// <summary>
        /// The Driver object
        /// </summary>
        [Required]

        public ViewModels.Driver Driver { get; set; }

        /// <summary>
        /// Sequence Number - DFCMS primary key for the case
        /// </summary>
        [Required]
        public long SequenceNumber { get; set;}        

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

        /// <summary>
        /// GUID Document Id from the Case Management System
        /// </summary>
        public string DocumentId { get; set;}

        /// <summary>
        /// Date the document was made
        /// </summary>
        public DateTimeOffset FaxReceivedDate { get; set; }
        public DateTimeOffset ImportDate { get; set; }
    }
}
