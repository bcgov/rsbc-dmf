using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

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
            bool success = isPhsaOauthDisabled;
            if (isPhsaOauthDisabled ||
                (
                 context.User.Identities.Any(x => x.IsAuthenticated)
                 && context.User.Claims.Contains(new System.Security.Claims.Claim("scope", "phsa-adapter"))
                )
               )
            {
                success = true;
                context.Succeed(requirement);
            }

            if (!success)
            {
                bool isAuthenticated = context.User.Identities.Any(x => x.IsAuthenticated);
                Log.Information($"ERROR IN OAUTH - Authenticated is {isAuthenticated.ToString()}");
                foreach (var claim in context.User.Claims)
                {
                    Log.Information($"CLAIM {claim.Type} is {claim.Value}");
                }
            }

            return Task.CompletedTask;
        }
    }

    
}
