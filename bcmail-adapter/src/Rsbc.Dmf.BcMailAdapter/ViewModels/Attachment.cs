namespace Rsbc.Dmf.BcMailAdapter.ViewModels
{
    /// <summary>
    /// Attachment
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// Body
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// Header
        /// </summary>
        public byte[] Header { get; set; }

        /// <summary>
        /// Footer
        /// </summary>
        public byte[] Footer { get; set; }

        /// <summary>
        /// Content Type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        public string FileName { get; set; }
    }
}