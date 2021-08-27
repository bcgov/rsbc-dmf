using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    internal static class DynamicsContextEx
    {
        public static void DetachAll(this DynamicsContext context)
        {
            foreach (var descriptor in context.EntityTracker.Entities)
            {
                context.Detach(descriptor.Entity);
            }
            foreach (var link in context.EntityTracker.Links)
            {
                context.DetachLink(link.Source, link.SourceProperty, link.Target);
            }
        }

        public static void Detach(this DynamicsContext context, params object[] entities)
        {
            foreach (var entity in entities)
            {
                context.Detach(entity);
            }
        }

        public static void ActivateObject(this DynamicsContext context, object entity, int activeStatusValue) =>
            ModifyEntityStatus(context, entity, (int)EntityState.Active, activeStatusValue);

        public static void DeactivateObject(this DynamicsContext context, object entity, int inactiveStatusValue) =>
            ModifyEntityStatus(context, entity, (int)EntityState.Inactive, inactiveStatusValue);

        private static void ModifyEntityStatus(this DynamicsContext context, object entity, int state, int status)
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