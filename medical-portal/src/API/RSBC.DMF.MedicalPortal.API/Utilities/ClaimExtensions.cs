using System.Security.Claims;
using System.Text.Json;

namespace RSBC.DMF.MedicalPortal.API.Utilities
{
    public static class ClaimExtensions
    {
        public static List<Claim> AddClaim<T>(this List<Claim> claims, string claimType, T value)
        {
            claims.Add(new Claim(claimType, JsonSerializer.Serialize(value)));
            return claims;
        }

        public static T GetClaim<T>(this ClaimsPrincipal claims, string claimType)
        {
            var claim = claims.FindFirstValue(claimType);
            if (claim == null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(claim);
        }
    }
}
