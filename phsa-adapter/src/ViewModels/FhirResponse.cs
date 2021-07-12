using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PhsaAdapter.ViewModels
{
    /// <summary>
    /// Based on the response model in https://github.com/FirelyTeam/spark
    /// </summary>
    public class FhirResponse
    {
        public HttpStatusCode StatusCode;
        public IKey Key;
        public Resource Resource;

        public FhirResponse(HttpStatusCode code, IKey key, Resource resource)
        {
            StatusCode = code;
            Key = key;
            Resource = resource;
        }

        public FhirResponse(HttpStatusCode code, Resource resource)
        {
            StatusCode = code;
            Key = null;
            Resource = resource;
        }

        public FhirResponse(HttpStatusCode code)
        {
            StatusCode = code;
            Key = null;
            Resource = null;
        }

        public bool IsValid
        {
            get
            {
                int code = (int) StatusCode;
                return code <= 300;
            }
        }

        public bool HasBody
        {
            get { return Resource != null; }
        }

        public override string ToString()
        {
            string details = (Resource != null) ? string.Format("({0})", Resource.TypeName) : null;
            string location = Key?.ToString();
            return string.Format("{0}: {1} {2} ({3})", (int) StatusCode, StatusCode.ToString(), details, location);
        }
    }
}
