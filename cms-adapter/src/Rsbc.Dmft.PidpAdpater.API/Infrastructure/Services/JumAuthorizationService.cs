using DomainResults.Common;
using pdipadapter.Data;
using pdipadapter.Models;
using pdipadapter.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace pdipadapter.Infrastructure.Services
{
    public class JumAuthorizationService : IJumAuthorizationService
    {
        private readonly IAuthorizationService authService;
        private readonly JumDbContext context;

        public JumAuthorizationService(IAuthorizationService authService, JumDbContext context)
        {
            this.authService = authService;
            this.context = context;
        }

        Task<IDomainResult> IJumAuthorizationService.CheckPartyAccessibility(int partyId, ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        Task<IDomainResult> IJumAuthorizationService.CheckResourceAccessibility<T>(Expression<Func<T, bool>> predicate, ClaimsPrincipal user, string policy)
        {
            throw new NotImplementedException();
        }



        //public async Task<IDomainResult> CheckPartyAccessibility(int partyId, ClaimsPrincipal user) => await this.CheckResourceAccessibility((Party party) => party.Id == partyId, user, Policies.UserOwnsResource);

        //public async Task<IDomainResult> CheckResourceAccessibility<T>(Expression<Func<T, bool>> predicate, ClaimsPrincipal user, string policy) where T : class, IOwnedResource
        //{
        //    var resourceStub = await this.context.Set<T>()
        //        .AsNoTracking()
        //        .Where(predicate)
        //        .Select(x => new OwnedResourceStub { UserId = x.UserId })
        //        .SingleOrDefaultAsync();

        //    if (resourceStub == null)
        //    {
        //        return DomainResult.NotFound();
        //    }

        //    var result = await this.authService.AuthorizeAsync(user, resourceStub, policy);

        //    return result.Succeeded
        //        ? DomainResult.Success()
        //        : DomainResult.Unauthorized();
        //}

        private class OwnedResourceStub : IOwnedResource
        {
            public Guid UserId { get; set; }
        }
    }
}
