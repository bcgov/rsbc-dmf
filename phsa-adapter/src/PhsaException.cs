using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PhsaAdapter
{
    public class PhsaException : Exception
    {
        public HttpStatusCode StatusCode;
        public OperationOutcome Outcome { get; set; }

        public PhsaException(HttpStatusCode statuscode, string message = null) : base(message)
        {
            this.StatusCode = statuscode;
        }

        public PhsaException(HttpStatusCode statuscode, string message, params object[] values)
            : base(string.Format(message, values))
        {
            this.StatusCode = statuscode;
        }

        public PhsaException(string message) : base(message)
        {
            this.StatusCode = HttpStatusCode.BadRequest;
        }

        public PhsaException(HttpStatusCode statuscode, string message, Exception inner) : base(message, inner)
        {
            this.StatusCode = statuscode;
        }

        public PhsaException(HttpStatusCode statuscode, OperationOutcome outcome, string message = null)
            : this(statuscode, message)
        {
            this.Outcome = outcome;
        }
    }
}
