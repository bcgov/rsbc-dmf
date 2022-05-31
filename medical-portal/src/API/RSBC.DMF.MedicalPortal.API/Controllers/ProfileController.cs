using Microsoft.AspNetCore.Mvc;
using RSBC.DMF.MedicalPortal.API.Services;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
                EmailAddress = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Clinics = profile.ClinicAssignments.Select(c => new ClinicUserProfile
                {
                    PractitionerId = c.PractitionerId,
                    ClinicId = c.ClinicId,
                    ClinicName = c.ClinicName,
                    Role = c.Role
                })
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


        /// <summary>
        /// Add the given practitioner role
        /// </summary>
        /// <param name="newRole"></param>
        /// <returns></returns>
        [HttpPut("practitionerRole")]
        public ActionResult SetRole([FromBody] PractitionerBridge newRole)
        {
            return Ok();
        }

        /// <summary>
        /// Remove the given practioner role
        /// </summary>
        /// <param name="newRole"></param>
        /// <returns></returns>
        [HttpDelete("practitionerRole")]
        public ActionResult ClearRole([FromBody] PractitionerBridge newRole)
        {
            return Ok();
        }


        public record EmailUpdate
        {
            public string Email { get; set; }
        }

        public enum ProviderRole
        {
            None = 0,
            Practitioner,
            [EnumMember(Value = "Medical Office Manager")]
            MedicalOfficeManager,
            [EnumMember(Value = "Medical Office Assistant")]
            MedicalOfficeAssistant,
        }

        public record UserProfile
        {
            public string EmailAddress { get; set; }

            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public IEnumerable<PractitionerBridge> Practitioners { get; set; }

            public IEnumerable<ClinicUserProfile> Clinics { get; set; }
        }

        public record PractitionerBridge
        {
            public string PractitionerId { get; set; }
            public ProviderRole Role { get; set; }
        }

        public record ClinicUserProfile
        {
            public string PractitionerId { get; set; }
            public string ClinicId { get; set; }
            public string ClinicName { get; set; }
            public string Role { get; set; }
        }
    }
}