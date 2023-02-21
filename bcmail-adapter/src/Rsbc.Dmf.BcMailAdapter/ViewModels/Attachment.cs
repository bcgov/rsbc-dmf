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
        public string? Body { get; set; }

        /// <summary>
        /// Header
        /// </summary>
        public string? Header { get; set; }

        /// <summary>
        /// Footer
        /// </summary>
        public string? Footer { get; set; }

        /// <summary>
        /// Content Type
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// File Name
        /// </summary>
        public string? FileName { get; set; }
    }
}