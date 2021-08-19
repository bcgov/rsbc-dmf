using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    internal class DynamicsContext : Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM.System
    {
        public DynamicsContext(Uri serviceRoot, Uri url, Func<Task<string>> tokenFactory, ILogger<DynamicsContext> logger) : base(serviceRoot)
        {
            this.SaveChangesDefaultOptions = SaveChangesOptions.None; // SaveChangesOptions.BatchWithSingleChangeset;  
            this.EntityParameterSendOption = EntityParameterSendOption.SendOnlySetProperties;
            

            Func<Uri, Uri> formatUri = requestUri => requestUri.IsAbsoluteUri
                    ? new Uri(url, (url.AbsolutePath == "/" ? string.Empty : url.AbsolutePath) + requestUri.AbsolutePath + requestUri.Query)
                    : new Uri(serviceRoot, (url.AbsolutePath == "/" ? string.Empty : url.AbsolutePath) + serviceRoot.AbsolutePath + requestUri.ToString());

            BuildingRequest += (sender, args) =>
            {
                args.Headers.Add("Authorization", $"Bearer {tokenFactory().GetAwaiter().GetResult()}");
                args.RequestUri = formatUri(args.RequestUri);
            };

            Configurations.RequestPipeline.OnEntryStarting((arg) =>
            {
                // do not send reference properties and null values to Dynamics
                arg.Entry.Properties = arg.Entry.Properties.Where((prop) => !prop.Name.StartsWith('_') && prop.Value != null);
            });

            Configurations.RequestPipeline.OnEntityReferenceLink((arg) =>
            {
                logger.LogDebug("OnEntityReferenceLink url {0}", arg.EntityReferenceLink.Url);
            });
        }
    }
}