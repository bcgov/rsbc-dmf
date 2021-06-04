namespace Rsbc.Dmf.Interfaces
{
    using Microsoft.Rest;

    public class S3Exception : RestException
    {

        /// <summary>
        /// Gets information about the associated HTTP request.
        /// </summary>
        public HttpRequestMessageWrapper Request { get; set; }

        /// <summary>
        /// Gets information about the associated HTTP response.
        /// </summary>
        public HttpResponseMessageWrapper Response { get; set; }


        /// <summary>
        /// Initializes a new instance of the HttpOperationException class.
        /// </summary>
        public S3Exception()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpOperationException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public S3Exception(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpOperationException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public S3Exception(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

    }
}