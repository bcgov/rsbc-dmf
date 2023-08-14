using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SystemStatus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   
    public class LoginController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            var authResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            if (authResult?.Succeeded != true)
            {
                // Handle failed authentication
                return RedirectToAction("Login");
            }

            // Get the access token and refresh token
            var accessToken = authResult.Properties.GetTokenValue("access_token");
            var refreshToken = authResult.Properties.GetTokenValue("refresh_token");

            // Save the tokens to the user's session or database
            HttpContext.Session.SetString("access_token", accessToken);
            HttpContext.Session.SetString("refresh_token", refreshToken);

            // Redirect the user to the desired page
            return RedirectToAction("Index");
        }
    }
}
