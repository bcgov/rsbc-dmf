using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService userService;

        public ProfileController(IUserService userService)
        {
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpGet("current")]
        public async Task<ActionResult<UserProfile>> GetCurrentProfile()
        {
            var profile = await userService.GetCurrentUserContext();

            if (profile == null) return NotFound();

            return new UserProfile
            {
                Id = profile.Id,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Roles = profile.Roles,
                Endorsements = profile.Endorsements
            };
        }

        public record UserProfile
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public IEnumerable<string> Roles { get; set; }
            public IEnumerable<Endorsement> Endorsements { get; set; }
        }
    }
}