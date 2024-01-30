using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Rsbc.Dmf.LegacyAdapter.Controllers
{
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration Configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Returns a security token.
        /// </summary>
        /// <param name="secret"></param>
        /// <returns>A new JWT</returns>
        [HttpGet("token")]
        [AllowAnonymous]
        public string GetToken([FromQuery] string secret)
        {
            string result = "Invalid secret.";
            string configuredSecret = Configuration["JWT_TOKEN_KEY"];
            if (configuredSecret.Equals(secret))
            {

                var key = new byte[16];
                var sourceKey = Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]);
                sourceKey.CopyTo(key,0);

                var symmetricSecurityKey = new SymmetricSecurityKey(key);


                var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    Configuration["JWT_VALID_ISSUER"],
                    Configuration["JWT_VALID_AUDIENCE"],
                    expires: DateTime.UtcNow.AddYears(3),
                    signingCredentials: creds
                    );
                result = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken); 
            }

            return result;
        }
    }
}
