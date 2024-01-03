using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.DriverPortal.Api.Services;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly IUserService userService;

        public ProfileController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("current")]
        public async Task<ActionResult<UserProfile>> GetCurrentProfile()
        {
            var profile = await userService.GetCurrentUserContext();

            if (profile == null) return NotFound();

            return new UserProfile
            {
                Id = profile.Id,
                EmailAddress = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,                
            };
        }



        /// <summary>
        /// set the user's profile email
        /// </summary>
        /// <param name="newEmail"></param>
        /// <returns></returns>
        [HttpPut("email")]
        public async Task<ActionResult> UpdateEmail([FromBody] EmailUpdate newEmail)
        {
            var profile = await userService.GetCurrentUserContext();

            if (profile == null) return NotFound();

            // update the user email.

            await userService.SetEmail( profile.Id, newEmail.Email);

            return Ok();
        }

        public record EmailUpdate
        {
            public string Email { get; set; }
        }


        public record UserProfile
        {
            public string EmailAddress { get; set; }

            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

        }

    }
}