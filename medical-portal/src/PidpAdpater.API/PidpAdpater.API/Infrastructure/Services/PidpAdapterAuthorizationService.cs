using DomainResults.Common;
using System.Security.Claims;
using pdipadapter.Extensions;
using Rsbc.Dmf.CaseManagement.Service;
using Microsoft.AspNetCore.Identity;

namespace pdipadapter.Infrastructure.Services
{
    public class PidpAdapterAuthorizationService : IPidpAdapterAuthorizationService
    {
        private readonly UserManager.UserManagerClient userManager;

        public PidpAdapterAuthorizationService(UserManager.UserManagerClient userManager)
         =>
            this.userManager = userManager;

        public async Task<IDomainResult> CheckContactAccessibility(string contactId, ClaimsPrincipal user)
        {
            var contact = await userManager.GetPractitionerContactAsync(new PractitionerRequest { Hpdid = $"{contactId}" });

            if (string.IsNullOrEmpty(contact.ContactId))
            {
                return DomainResult.NotFound();
            }

            var userId = user.GetUserId();
            var hpdid = user.GetIdpId();
            var pidpEmail = user.GetPidpEmail();
            if (userId != Guid.Empty && pidpEmail != string.Empty
                && contact.IdpId == hpdid)
            {
                return DomainResult.Success();
            }

            return DomainResult.Unauthorized();
        }
    }
}
