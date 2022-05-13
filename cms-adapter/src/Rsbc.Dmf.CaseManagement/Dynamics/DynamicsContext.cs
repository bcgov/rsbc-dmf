using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    internal class DynamicsContext : Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM.System
    {
        public DynamicsContext(Uri serviceRoot, Uri url, Func<Task<string>> tokenFactory, ILogger<DynamicsContext> logger) : base(serviceRoot)
        {
            this.SaveChangesDefaultOptions = SaveChangesOptions.None; //BatchWithSingleChangeset;  // Set to SaveChangesOptions.None to troubleshoot query issues
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

        public void DetachAll()
        {
            foreach (var descriptor in this.EntityTracker.Entities)
            {
                this.Detach(descriptor.Entity);
            }
            foreach (var link in this.EntityTracker.Links)
            {
                this.DetachLink(link.Source, link.SourceProperty, link.Target);
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllPagesAsync<TEntity>(IQueryable<TEntity> query) => (await ((DataServiceQuery<TEntity>)query).GetAllPagesAsync());

        public void ActivateObject(object entity, int activeStatusValue) =>
            ModifyEntityStatus(this, entity, (int)EntityState.Active, activeStatusValue);

        public void DeactivateObject(object entity, int inactiveStatusValue) =>
            ModifyEntityStatus(this, entity, (int)EntityState.Inactive, inactiveStatusValue);

        private static void ModifyEntityStatus(DynamicsContext context, object entity, int state, int status)
        {
            var entityType = entity.GetType();
            if (!typeof(crmbaseentity).IsAssignableFrom(entityType)) throw new InvalidOperationException($"entity {entityType.FullName} is not a valid {typeof(crmbaseentity).FullName}");
            var statusProp = entity.GetType().GetProperty("statuscode");
            var stateProp = entity.GetType().GetProperty("statecode");

            statusProp.SetValue(entity, status);
            stateProp.SetValue(entity, state);

            context.UpdateObject(entity);
        }
    }

    internal enum EntityState
    {
        Active = 0,
        Inactive = 1
    }
}