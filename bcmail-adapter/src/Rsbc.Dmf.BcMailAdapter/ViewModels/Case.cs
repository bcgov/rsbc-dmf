using System;
using System.Collections.Generic;

namespace Rsbc.Dmf.BcMailAdapter.ViewModels
{
    /// <summary>
    /// Case View Model
    /// </summary>
    public class Case
    {
        /// <summary>
        /// Case Id
        /// </summary>
        public string CaseId { get; set; }

        /// <summary>
        /// Documents
        /// </summary>
        public List<Document> Documents { get; set; }
    }
}
