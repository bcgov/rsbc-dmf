using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using System.Collections.Generic;
using System.Security.Claims;

namespace Rsbc.Dmf.PhsaAdapter
{
    public class FhirOauthRequirementHandler : AuthorizationHandler<FhirOauthRequirement>
    {
        private readonly IConfiguration Configuration;
        private readonly bool isPhsaOauthDisabled;
        private readonly bool enableDebug;

        public FhirOauthRequirementHandler(IConfiguration configuration)
        {
            Configuration = configuration;
            isPhsaOauthDisabled = string.IsNullOrEmpty(Configuration["ENABLE_PHSA_OAUTH"]);
            enableDebug = !string.IsNullOrEmpty(Configuration["ASPNETCORE_ENVIRONMENT"]) && Configuration["ASPNETCORE_ENVIRONMENT"] == "development";
        }

        private bool CheckClaims(IEnumerable<Claim> claims)
        {
            bool result = false;
            foreach (var claim in claims)
            {
                if (claim != null && 
                    claim.Type != null && claim.Type == "scope" && claim.Value != null && 
                    (claim.Value == "doctors-portal-api" || claim.Value == "phsa-adapter"))
                {
                    result = true;
                }
            }
            return result;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FhirOauthRequirement requirement)
        {
            bool success = isPhsaOauthDisabled;
            if (isPhsaOauthDisabled ||
                (
                 context.User.Identities.Any(x => x.IsAuthenticated)
                 && CheckClaims(context.User.Claims) 
                )
               )
            {
                success = true;
                context.Succeed(requirement);
            }

            if (!success && enableDebug)
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
