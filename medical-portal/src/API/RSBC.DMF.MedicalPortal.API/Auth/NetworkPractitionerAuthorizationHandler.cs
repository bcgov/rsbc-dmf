using Microsoft.AspNetCore.Authorization;
using RSBC.DMF.MedicalPortal.API.Utilities;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using static RSBC.DMF.MedicalPortal.API.Auth.AuthConstant;

namespace RSBC.DMF.MedicalPortal.API.Auth
{
    // check the loginId is in network of endorsed practitioners with valid licence
    public class NetworkPractitionerAuthorizationHandler : AuthorizationHandler<NetworkPractitionerRequirement, Guid>
    {
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, NetworkPractitionerRequirement requirement, Guid loginId)
        {
            if (!context.User.HasClaim(c => c.Type == Claims.Endorsements))
            {
                return;
            }

            var endorsements = context.User.GetClaim<IEnumerable<Endorsement>>(Claims.Endorsements);
            if (endorsements.Any(e => e.LoginId == loginId && e.Role == Roles.Practitoner && e.Licences.Any(l => l.StatusCode == LicenceStatusCode.Active)))
            {
                context.Succeed(requirement);
            }

            return;
        }
    }

    public class  NetworkPractitionerRequirement : IAuthorizationRequirement { }
}
