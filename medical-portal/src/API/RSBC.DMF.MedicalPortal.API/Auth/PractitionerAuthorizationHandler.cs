using Microsoft.AspNetCore.Authorization;
using RSBC.DMF.MedicalPortal.API.Auth.Extension;
using static RSBC.DMF.MedicalPortal.API.Auth.AuthConstant;

namespace RSBC.DMF.MedicalPortal.API.Auth
{
    // NOTE only use this when you need to know practitioner role of user at runtime, otherwise use [Authorize(Policy = Policies.MedicalPractitioner)] instead
    public class PractitionerAuthorizationHandler : AuthorizationHandler<PractitionerRequirement>
    {
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, PractitionerRequirement requirement)
        {
            var roleClaimKey = context.User.Identities
                .FirstOrDefault(c => !string.IsNullOrEmpty(c.RoleClaimType))
                .RoleClaimType;
            if (!context.User.HasClaim(c => c.Type == roleClaimKey))
            {
                return;
            }

            if (context.User.GetRoles()?.Contains(Roles.Practitoner) ?? false)
            {
                context.Succeed(requirement);
            }

            return;
        }
    }

    public class PractitionerRequirement : IAuthorizationRequirement { }
}