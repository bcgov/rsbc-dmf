using Microsoft.AspNetCore.Authorization;

namespace Rsbc.Dmf.DriverPortal.Api
{
    public class Policy
    {
        public const string Driver = "Driver";
    }

    public class DriverPolicyFactory
    {
        public AuthorizationPolicy Create()
        {
            return new AuthorizationPolicyBuilder()
                .RequireClaim(UserClaimTypes.DriverId)
                .Build();
        }
    }
}
