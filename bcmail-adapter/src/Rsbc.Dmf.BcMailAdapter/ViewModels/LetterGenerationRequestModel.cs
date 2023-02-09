using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Rsbc.Dmf.BcMailAdapter.ViewModels
{
    /// <summary>
    /// Letter Generation Request Model
    /// </summary>
    public class LetterGenerationRequestModel
    {
        /// <summary>
        /// Data
        /// </summary>
        public Data? Data { get; set; }

        //public string? Formatters { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        public Options? Options { get; set; }

        /// <summary>
        /// Template
        /// </summary>
        public Template Template { get; set; }

    }

    /// <summary>
    /// Data
    /// </summary>
    public class Data
    {
        
        //public string? FirstName { get; set; }
        //public string? LastName { get; set; }
        //public string? Title { get; set; }

    }

    /// <summary>
    /// Options
    /// </summary>
    public class Options
    {
        //public bool? CacheReport { get; set; }
        /// <summary>
        /// Conver To
        /// </summary>
        public string? ConvertTo { get; set; }

        /// <summary>
        /// Overwrite
        /// </summary>
        public bool? Overwrite { get; set; }

        /// <summary>
        /// ReportName
        /// </summary>
        public string? ReportName { get; set; }

    }

    /// <summary>
    /// Template
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Content
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// File Type
        /// </summary>
        public string? FileType { get; set; }

        /// <summary>
        /// Encoding Type
        /// </summary>
        public string? EncodingType { get; set; }
    }


}
