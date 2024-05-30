namespace pdipadapter.Infrastructure.Auth;

using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
public class RealmAccessRoleHandler : AuthorizationHandler<RealmAccessRoleRequirement>
    {
        /// <summary>
        /// Determine if the current user has the specified role.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RealmAccessRoleRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == Claims.RealmAccess))
            {
                return Task.CompletedTask;
            }

            var claim = context.User.Claims.First(c => c.Type == Claims.RealmAccess);
            if (claim.Value.Contains($"\"{requirement.Role}\""))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
