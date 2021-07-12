using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Rsbc.Dmf.PhsaAdapter.ViewModels;

namespace Rsbc.Dmf.PhsaAdapter
{
    public static class Error
    {
        public static PhsaException Create(HttpStatusCode code, string message, params object[] values)
        {
            return new PhsaException(code, message, values);
        }

        public static PhsaException BadRequest(string message, params object[] values)
        {
            return new PhsaException(HttpStatusCode.BadRequest, message, values);
        }

        public static PhsaException NotFound(string message, params object[] values)
        {
            return new PhsaException(HttpStatusCode.NotFound, message, values);
        }

        public static PhsaException NotFound(IKey key)
        {
            if (key.VersionId == null)
            {
                return NotFound("No {0} resource with id {1} was found.", key.TypeName, key.ResourceId);
            }
            else
            {
                return NotFound("There is no {0} resource with id {1}, or there is no version {2}", key.TypeName, key.ResourceId, key.VersionId);
            }
        }

        public static PhsaException NotAllowed(string message)
        {
            return new PhsaException(HttpStatusCode.Forbidden, message);
        }

        public static PhsaException Internal(string message, params object[] values)
        {
            return new PhsaException(HttpStatusCode.InternalServerError, message, values);
        }

        public static PhsaException NotSupported(string message, params object[] values)
        {
            return new PhsaException(HttpStatusCode.NotImplemented, message, values);
        }

        private static OperationOutcome.IssueComponent CreateValidationResult(string details, IEnumerable<string> location)
        {
            return new OperationOutcome.IssueComponent()
            {
                Severity = OperationOutcome.IssueSeverity.Error,
                Code = OperationOutcome.IssueType.Invalid,
                Details = new CodeableConcept("http://hl7.org/fhir/issue-type", "invalid"),
                Diagnostics = details,
                Location = location
            };
        }
    }
}
