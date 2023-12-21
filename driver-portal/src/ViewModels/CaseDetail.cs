using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;

namespace Rsbc.Dmf.DriverPortal.ViewModels
{
    public class CaseDetail
    {
        /// <summary>
        /// The GUID that is the primary key for this case, sent as a string
        /// </summary>
        public string CaseId { get; set; }

        /// <summary>
        /// Indicates system title of the case
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Indicates the Code which will be used for eDMERs, eg H2M0L1
        /// </summary>
        public string IdCode { get; set; }

        /// <summary>
        /// The Oracle Case Sequence, if available
        /// </summary>
        public int? CaseSequence { get; set; }

        /// <summary>
        /// Indicated the date the case was opened.
        /// </summary>
        public DateTimeOffset OpenedDate { get; set; }

        /// <summary>
        /// Indicates the purpose for which the case was opened
        /// </summary>
        public string CaseType { get; set; }

        /// <summary>
        /// Indicates the DMER type, if relevant
        /// </summary>
        public string DmerType { get; set; }

        /// <summary>
        //  Indicates the current stage of workflow
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Indicates the job title of the case assignee
        /// </summary>
        public string AssigneeTitle { get; set; }

        /// <summary>
        /// Indicates the most recent case update date
        /// </summary>
        public DateTimeOffset LastActivityDate { get; set; }

        /// <summary>
        /// Indicates the type and category of the most recent decision
        /// </summary>
        public string LatestDecision { get; set; }

        /// <summary>
        /// Indicates approved license class of the most recent decision
        /// </summary>
        public string DecisionForClass { get; set; }

        /// <summary>
        /// Indicates the date of the most recent decision
        /// </summary>
        public DateTimeOffset? DecisionDate { get; set; }

        /// <summary>
        /// Indicates the current date of fax intake processing for DPS
        /// </summary>
        public DateTimeOffset DpsProcessingDate { get; set; }

        public List<Document> Documents { get; set; }

        public List<Comment> Comments { get; set; }

        public string EligibleLicenseClass { get; set; }

        public int OutstandingDocuments { get; set; }

    }
}
