using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Rsbc.Dmf.DriverPortal.Api;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public class PolicyTests
    {
        // NOTE this must match the policy requirements for UserClaimTypes.DriverId, see Program.cs
        // [Authorize(Policy = Policy.Driver)]
        private readonly AuthorizationPolicy _authorizationPolicy = new AuthorizationPolicyBuilder()
            .RequireClaim(UserClaimTypes.DriverId)
            .Build();

        [Fact]
        public async Task Authorize_Driver_Success()
        {
            var claims = new List<Claim>
            {
                new Claim(UserClaimTypes.DriverId, "DriverId")
            };

            var result = await CanAuthorizeUserWithPolicyAsync(claims, _authorizationPolicy);
            Assert.True(result);
        }

        [Fact]
        public async Task Authorize_Driver_Forbidden()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "New User")
            };

            var result = await CanAuthorizeUserWithPolicyAsync(claims, _authorizationPolicy);

            Assert.False(result);
        }

        private async Task<bool> CanAuthorizeUserWithPolicyAsync(IEnumerable<Claim> claims, AuthorizationPolicy authorizationPolicy)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var handlers = authorizationPolicy.Requirements.Select(x => x as IAuthorizationHandler).ToArray();
            var authorizationOptions = Options.Create(new AuthorizationOptions());
            authorizationOptions.Value.AddPolicy(nameof(authorizationPolicy), authorizationPolicy);

            var policyProvider = new DefaultAuthorizationPolicyProvider(authorizationOptions);
            var handlerProvider = new DefaultAuthorizationHandlerProvider(handlers);
            var contextFactory = new DefaultAuthorizationHandlerContextFactory();

            var authorizationService = new DefaultAuthorizationService(
                policyProvider,
                handlerProvider,
                new NullLogger<DefaultAuthorizationService>(),
                contextFactory,
                new DefaultAuthorizationEvaluator(),
                authorizationOptions);

            var result = await authorizationService.AuthorizeAsync(user, authorizationPolicy);
            return result.Succeeded;
        }
    }
}
