using System;

namespace Rsbc.Interfaces
{

    public class PdfResponse
    {


        /// Name of the file with extension 
        /// </summary>
        public string FileName { get; set; }

       
        /// <summary>
        /// Base 64 encoded string
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// ContentType of the file
        /// </summary>
        public string ContentType { get; set; }
    }
}