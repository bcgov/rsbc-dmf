using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.IcbcAdapter.ViewModels
{
    public class Upload
    {
        /// <summary>
        /// Name of the file with extension 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// schema name of the entity on which this document is uploaded
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// GUID of the record in Dynamics
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Base 64 encoded string
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// ContentType of the file
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Metadata tag 1
        /// </summary>
        public string Tag1 { get; set; }

        /// <summary>
        /// Metadata tag 2
        /// </summary>
        public string Tag2 { get; set; }

        /// <summary>
        /// Metadata tag 3
        /// </summary>
        public string Tag3 { get; set; }
    }
}
