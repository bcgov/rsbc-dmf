using System.Collections.Generic;

namespace Rsbc.Dmf.BcMailAdapter.ViewModels
{
    /// <summary>
    /// Bc Mail 
    /// </summary>
    public class BcMail
    {
        /// <summary>
        /// Is Preview
        /// </summary>
        public bool? isPreview { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Attachment> Attachments { get; set; }

    }
}
