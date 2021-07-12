
using FhirModel = Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.IO;
using System.Threading.Tasks;
using Rsbc.Dmf.PhsaAdapter.ViewModels;
using Rsbc.Dmf.PhsaAdapter;
using Rsbc.Dmf.PhsaAdapter.Extensions;

namespace Rsbc.Dmf.PhsaAdapter.Formatters
{
    public class BinaryOutputFormatter : OutputFormatter
    {
        public BinaryOutputFormatter()
        {
            SupportedMediaTypes.Add(FhirMediaType.OctetStreamMimeType);
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(FhirModel.Binary).IsAssignableFrom(type) || typeof(FhirResponse).IsAssignableFrom(type);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            if (typeof(FhirModel.Binary).IsAssignableFrom(context.ObjectType) || typeof(FhirResponse).IsAssignableFrom(context.ObjectType))
            {
                FhirModel.Binary binary = null;
                if (typeof(FhirResponse).IsAssignableFrom(context.ObjectType))
                {
                    FhirResponse response = (FhirResponse)context.Object;

                    context.HttpContext.Response.AcquireHeaders(response);
                    context.HttpContext.Response.StatusCode = (int)response.StatusCode;

                    binary = response.Resource as FhirModel.Binary;
                }
                if (binary == null) return;

                context.HttpContext.Response.Headers.Add(HttpHeaderName.CONTENT_DISPOSITION, "attachment");
                context.HttpContext.Response.ContentType = binary.ContentType;

                //Stream stream = new MemoryStream(binary.Content);
                Stream stream = new MemoryStream(binary.Data);
                await stream.CopyToAsync(context.HttpContext.Response.Body);
            }
        }
    }
}
