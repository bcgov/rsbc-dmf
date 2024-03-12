using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity;

namespace OAuthServer
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.AddRequestedClaims(context.Subject.Claims);

            foreach (Claim claim in context.Subject.Claims)
            {
                if (context.IssuedClaims.Contains(claim))
                    continue;

                context.IssuedClaims.Add(claim);
            }

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}
