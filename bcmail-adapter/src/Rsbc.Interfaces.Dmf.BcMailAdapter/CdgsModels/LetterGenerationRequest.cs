namespace Rsbc.Interfaces.CdgsModels
{
    public class LetterGenerationRequest
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
}