using Microsoft.AspNetCore.Mvc;
using RSBC.DMF.MedicalPortal.API.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpGet("current")]
        public async Task<ActionResult<UserProfile>> GetCurrentProfile()
        {
            var profile = await userService.GetCurrentUserContext();

            if (profile == null) return NotFound();

            return new UserProfile
            {
                Id = profile.Id,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Clinics = profile.ClinicAssignments.Select(c => new ClinicUserProfile
                {
                    ClinicId = c.ClinicId,
                    ClinicName = c.ClinicName,
                    Role = c.Role
                })
            };
        }
    }

    public record UserProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<ClinicUserProfile> Clinics { get; set; }
    }

    public record ClinicUserProfile
    {
        public string ClinicId { get; set; }
        public string ClinicName { get; set; }
        public string Role { get; set; }
    }
}