using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PhsaAdapter
{
    public class FhirOauthRequirementHandler : AuthorizationHandler<FhirOauthRequirement>
    {
        private readonly IConfiguration Configuration;
        private readonly bool isPhsaOauthDisabled;

        public FhirOauthRequirementHandler(IConfiguration configuration)
        {
            Configuration = configuration;
            isPhsaOauthDisabled = string.IsNullOrEmpty(Configuration["ENABLE_PHSA_OAUTH"]);
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FhirOauthRequirement requirement)
        {
            if (isPhsaOauthDisabled ||
                (
                 context.User.Identities.Any(x => x.IsAuthenticated)
                 && context.User.Claims.Contains(new System.Security.Claims.Claim("scope", "phsa-adapter"))
                )
               )
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    
}
