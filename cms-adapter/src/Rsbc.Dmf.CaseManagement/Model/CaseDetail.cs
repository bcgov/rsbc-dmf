using System;
using System.Collections.Generic;
using System.Text;

namespace Rsbc.Dmf.CaseManagement
{
    public class CaseDetail
    {
        public string CaseId { get; set; }

        /// <summary>
        /// Indicates system title of the case
        /// </summary>
        /// 
        public int CaseSequence { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Indicates the Code which will be used for eDMERs, eg H2M0L1
        /// </summary>
        public string IdCode { get; set; }

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

        public string EligibleLicenseClass { get; set; }

        public int OutstandingDocuments { get; set; }

        /// <summary>
        /// Indicates the LatestCompliance Date
        /// </summary>
        public DateTimeOffset LatestComplianceDate { get; set; }

        // Driver
        public string DriverId { get; set; }
        public string Name { get; set; }
        public string DriverLicenseNumber { get; set; }
        public DateTimeOffset? BirthDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public IEnumerable<MedicalCondition> MedicalConditions { get; set; }
    }
}