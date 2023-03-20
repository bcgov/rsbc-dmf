namespace pdipadapter.Extensions;
using NodaTime;
using NodaTime.Text;
using System.Security.Claims;
using System.Text.Json;

using pdipadapter.Infrastructure.Auth;
using Microsoft.OData.Edm;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Returns the UserId of the logged in user (from the 'sub' claim). If there is no logged in user, this will return Guid.Empty
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal? user)
    {
        var userId = user?.FindFirstValue(Claims.Subject);
       // var idp = user.get

        return Guid.TryParse(userId, out var parsed)
            ? parsed
            : Guid.Empty;
    }
    /// <summary>
    /// Returns the Identity Provider ID of the User, or null if User is null
    /// </summary>
    public static string? GetIdpId(this ClaimsPrincipal? user) => user?.FindFirstValue(Claims.PreferredUsername);
    /// <summary>
    /// Returns the Birthdate Claim of the User, parsed in ISO format (yyyy-MM-dd)
    /// </summary>
    public static Date? GetBirthdate(this ClaimsPrincipal user)
    {
        var birthdate = user.FindFirstValue(Claims.Birthdate);
        Date c;
        var parsed = Microsoft.OData.Edm.Date.TryParse(birthdate,out c);
        if (parsed)
        {
            return c;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the Identity Provider of the User, or null if User is null
    /// </summary>
    public static string? GetIdentityProvider(this ClaimsPrincipal? user) => user?.FindFirstValue(Claims.IdentityProvider);

    /// <summary>
    /// Parses the Resource Access claim and returns the roles for the given resource
    /// </summary>
    /// <param name="resourceName">The name of the resource to retrive the roles from</param>
    public static IEnumerable<string> GetResourceAccessRoles(this ClaimsIdentity identity, string resourceName)
    {
        var resourceAccessClaim = identity.Claims
            .SingleOrDefault(claim => claim.Type == Claims.ResourceAccess)
            ?.Value;

        if (string.IsNullOrWhiteSpace(resourceAccessClaim))
        {
            return Enumerable.Empty<string>();
        }

        try
        {
            var resources = JsonSerializer.Deserialize<Dictionary<string, ResourceAccess>>(resourceAccessClaim, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return resources?.TryGetValue(resourceName, out var access) == true
                ? access.Roles
                : Enumerable.Empty<string>();
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }
    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
    {
        return principal.Identities.SelectMany(i =>
        {
            return i.Claims
                .Where(c => c.Type == i.RoleClaimType)
                .Select(c => c.Value)
                .ToList();
        });
    }

    private class ResourceAccess
    {
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    }
}

