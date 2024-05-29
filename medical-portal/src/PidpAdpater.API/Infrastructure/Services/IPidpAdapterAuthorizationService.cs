namespace pdipadapter.Infrastructure.Services;

using DomainResults.Common;
using System.Linq.Expressions;
using System.Security.Claims;
public interface IPidpAdapterAuthorizationService
    {
    /// <summary>
    /// Checks that the given Contact both exists and can be accessed by the given User.
    /// </summary>
    /// <param name="contactId">The Id of the Contact</param>
    /// <param name="user">The User to authorize against</param>
    /// <returns></returns>
    //Task<IDomainResult> CheckContactAccessibility(int contactId, ClaimsPrincipal user);

    /// <summary>
    /// Checks that the given Contact both exists and can be accessed by the given User.
    /// </summary>
    /// <param name="contactId">The Id of the Contact</param>
    /// <param name="user">The User to authorize against</param>
    /// <returns></returns>
    Task<IDomainResult> CheckContactAccessibility(string contactId, ClaimsPrincipal user);

}

