using System.Security.Claims;
using IdentityModel;

namespace PidpAdapter.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Returns the UserId of the logged in user (from the 'sub' claim). If there is no logged in user, this will return Guid.Empty
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal? user)
    {
        var userId = user?.FindFirstValue(JwtClaimTypes.Subject);
        return Guid.TryParse(userId, out var parsed)
            ? parsed
            : Guid.Empty;
    }
}
