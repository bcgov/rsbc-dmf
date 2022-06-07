using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RSBC.DMF.MedicalPortal.API.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
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
                    Role = ConvertProviderRole(c.Role)
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
        /// Get the current practitioner roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("practitionerRoles")]
        public ActionResult GetRoles()
        {
            List<PractitionerBridge> result = new List<PractitionerBridge>();

            return new JsonResult(result);
        }


        /// <summary>
        /// Add the given practitioner role
        /// </summary>
        /// <param name="newRoles"></param>
        /// <returns></returns>
        [HttpPut("practitionerRoles")]
        public ActionResult SetRole([FromBody] List<PractitionerBridge> newRoles)
        {
            return Ok();
        }

        /// <summary>
        /// Remove the given practioner role
        /// </summary>
        /// <param name="rolesToClear"></param>
        /// <returns></returns>
        [HttpDelete("practitionerRoles")]
        public ActionResult ClearRole([FromBody] List<PractitionerBridge> rolesToClear)
        {
            return Ok();
        }

        public record EmailUpdate
        {
            public string Email { get; set; }
        }

        public enum ProviderRole
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// Medical Practitioner
            /// </summary>
            [EnumMember(Value = "Medical Practitioner")]           
            MedicalPractitioner,
            /// <summary>
            /// Medical Office Manager
            /// </summary>
            [EnumMember(Value = "Medical Office Manager")]
            MedicalOfficeManager,
            /// <summary>
            /// Medical Office Assistant
            /// </summary>
            [EnumMember(Value = "Medical Office Assistant")]
            MedicalOfficeAssistant,
        }

        public enum ProviderRelationshipType
        {            
            [EnumMember(Value = "Medical Staff Association")]
            MedicalStaffAssociation,
            [EnumMember(Value = "Medical Practitioner Association")]
            MedicalPractitionerAssociation
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
            public ProviderRole Role { get; set; }
        }

        private ProviderRole ConvertProviderRole (string data)
        {
            ProviderRole result = ProviderRole.None;
            switch (data)
            {
                case "Medical Practitioner":
                    result = ProviderRole.MedicalPractitioner;
                    break;
                case "Medical Office Manager":
                    result = ProviderRole.MedicalOfficeManager;
                    break;
                case "Medical Office Assistant":
                    result = ProviderRole.MedicalOfficeAssistant;
                    break;
            }
            return result;
        }
    }
}