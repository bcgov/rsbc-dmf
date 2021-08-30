using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    /// <summary>
    /// Extensions for OData context
    /// </summary>
    internal partial class DynamicsContext
    {
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
}