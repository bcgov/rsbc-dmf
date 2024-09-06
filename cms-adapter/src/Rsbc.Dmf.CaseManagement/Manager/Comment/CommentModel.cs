using System;
using System.Collections.Generic;
using System.Text;

namespace Rsbc.Dmf.CaseManagement.Manager.Comment
{
    public class Comment
    {
        public int? SequenceNumber { get; set; }
        public string CommentTypeCode { get; set; }
        public string CommentText { get; set; }
        public string UserId { get; set; }
        public string CaseId { get; set; }
        public DateTimeOffset CommentDate { get; set; }
        public string CommentId { get; set; }
        public Driver Driver { get; set; }
        public string Assignee { get; set; }
        public string SignatureName { get; set; }
    }
}
