using System.Threading.Tasks;

namespace Pssg.Interfaces.Icbc.Services
{
    /// <summary>
    /// Interface for OAuth2 token management
    /// </summary>
    public interface IOAuth2TokenService
    {
        /// <summary>
        /// Gets a valid access token, acquiring a new one if needed
        /// </summary>
        /// <returns>Valid access token</returns>
        Task<string> GetAccessTokenAsync();

        /// <summary>
        /// Forces acquisition of a new token (bypassing cache)
        /// </summary>
        /// <returns>New access token</returns>
        Task<string> RefreshTokenAsync();
    }
}