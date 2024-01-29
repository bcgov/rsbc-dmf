using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly IUserService userService;

        public ProfileController(CaseManager.CaseManagerClient cmsAdapterClient, IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("current")]
        public async Task<ActionResult<UserProfile>> GetCurrentProfile()
        {
            var userContext = await userService.GetCurrentUserContext();
            if (userContext == null) return NotFound();

            var driverIdRequest = new DriverIdRequest() { Id = userContext.DriverId };
            var reply = _cmsAdapterClient.GetDriverDocumentsById(driverIdRequest);

            string emailAddress = userContext.Email;
            string firstName = userContext.FirstName;
            string lastName = userContext.LastName;

            if (reply.ResultStatus == ResultStatus.Success && reply.Items != null && reply.Items.Count > 0)
            {
                var driverRecord = reply.Items.FirstOrDefault();
                if (driverRecord != null)
                {
                    firstName = driverRecord.Driver.GivenName;
                    lastName = driverRecord.Driver.Surname;
                }
            }

            return new UserProfile
            {
                Id = userContext.Id,
                EmailAddress = emailAddress,
                FirstName = firstName,
                LastName = lastName,                
                DriverId = userContext.DriverId
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
            public string DriverId { get; set; }
        }
    }
}
