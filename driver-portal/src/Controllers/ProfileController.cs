using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
using Rsbc.Dmf.DriverPortal.ViewModels;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    // DOMAIN user profile, user

    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly UserManager.UserManagerClient _userManagerClient;
        private readonly IUserService userService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(CaseManager.CaseManagerClient cmsAdapterClient, UserManager.UserManagerClient userManagerClient, IUserService userService, ILoggerFactory loggerFactory)
        {
            this.userService = userService;
            _cmsAdapterClient = cmsAdapterClient;
            _userManagerClient = userManagerClient;
            _logger = loggerFactory.CreateLogger<ProfileController>();
        }

        [HttpGet("current")]
        [AllowAnonymous]
        public async Task<ActionResult<UserProfile>> GetCurrentProfile()
        {
            var userContext = await userService.GetCurrentUserContext();
            if (userContext == null || userContext.DriverId == null) return NotFound();

            var driverIdRequest = new DriverIdRequest() { Id = userContext.DriverId };
            var reply = _cmsAdapterClient.GetDriverById(driverIdRequest);

            string emailAddress = userContext.Email;
            string firstName = userContext.FirstName;
            string lastName = userContext.LastName;

            if (reply.ResultStatus == ResultStatus.Success && reply.Items != null && reply.Items.Count > 0)
            {
                var driverRecord = reply.Items.FirstOrDefault();
                if (driverRecord != null)
                {
                    firstName = driverRecord.GivenName;
                    lastName = driverRecord.Surname;
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
        /// user registration which sets the user's profile email
        /// </summary>
        /// <param name="newEmail"></param>
        /// <returns></returns>
        [HttpPut(nameof(Register))]
        [Authorize(Policy = Policy.Driver)]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(Register))]
        public async Task<ActionResult> Register([FromBody] UserRegistration userRegistration)
        {
            var profile = await userService.GetCurrentUserContext();

            if (profile == null) return NotFound();

            // verify user registration name and birth date
            // TODO check DOB after Keycloak is implemented
            var driverLicenseRequest = new DriverLicenseRequest();
            driverLicenseRequest.DriverLicenseNumber = userRegistration.DriverLicenseNumber;
            var getDriverReply = _cmsAdapterClient.GetDriverPerson(driverLicenseRequest);
            if (getDriverReply.ResultStatus != ResultStatus.Success)
            {
                _logger.LogError($"{nameof(UserRegistration)} failed for driverLicenseNumber: {userRegistration.DriverLicenseNumber}.\n {0}", getDriverReply.ErrorDetail);
                return StatusCode(500, getDriverReply.ErrorDetail);
            }
            if (getDriverReply.Items?.Count == 0)
            {
                return StatusCode(401, "No driver found.");
            }
            CaseManagement.Service.Driver foundDriver = null;
            foreach (var driver in getDriverReply.Items)
            {
                if (profile.FirstName == driver.GivenName && profile.LastName == driver.Surname /*&& userRegistration.BirthDate.Date == driver.BirthDate.ToDateTime().Date*/)
                {
                    foundDriver = driver;
                }
            }
            if (foundDriver == null)
            {
                return StatusCode(401, "Driver details do not match.");
            }

            // driver found and matched user registration
            // update email in dynamics 
            var userSetEmailRequest = new UserSetEmailRequest();
            userSetEmailRequest.UserId = profile.DriverId;
            userSetEmailRequest.Email = userRegistration.Email;
            var reply = _userManagerClient.SetDriverEmail(userSetEmailRequest);
            if (reply.ResultStatus == ResultStatus.Success)
            {
                return Ok();
            }
            else
            {
                _logger.LogError($"{nameof(UserRegistration)} updating email failed for driverLicenseNumber: {userRegistration.DriverLicenseNumber}.\n {0}", reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }
        }

        /// <summary>
        /// set the user's profile email
        /// </summary>
        /// <param name="newEmail"></param>
        /// <returns></returns>
        [HttpPut("driver")]
        public async Task<ActionResult> UpdateDriver([FromBody] DriverUpdate newEmail)
        {
            var profile = await userService.GetCurrentUserContext();

            if (profile == null) return NotFound();

            // update the driver information.
            
            return Ok();
        }

        public record EmailUpdate
        {
            public string Email { get; set; }
        }

        public record DriverUpdate
        {
            public string DriverLicense { get; set; }
            bool NotifyMail {  get; set; }
            bool NotifyEmail { get; set; }
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
