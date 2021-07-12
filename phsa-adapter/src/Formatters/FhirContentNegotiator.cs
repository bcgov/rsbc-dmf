using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using Rsbc.Dmf.PhsaAdapter.Extensions;

namespace Rsbc.Dmf.PhsaAdapter.Formatters
{
    public class FhirContentNegotiator : DefaultContentNegotiator
    {
        public override ContentNegotiationResult Negotiate(Type type, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
        {
            MediaTypeFormatter formatter;
            if(request.IsRawBinaryRequest(type))
            {
                formatter = formatters.Where(f => f is BinaryFhirFormatter).SingleOrDefault();
                if (formatter != null) return new ContentNegotiationResult(formatter.GetPerRequestFormatterInstance(type, request, null), null);
            }

            return base.Negotiate(type, request, formatters);
        }
    }
}