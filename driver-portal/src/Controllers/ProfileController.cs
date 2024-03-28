using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Net;
using System.Security.Claims;

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
        private readonly IConfiguration _configuration;

        public ProfileController(CaseManager.CaseManagerClient cmsAdapterClient, UserManager.UserManagerClient userManagerClient, IUserService userService, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            this.userService = userService;
            _cmsAdapterClient = cmsAdapterClient;
            _configuration = configuration;
            _userManagerClient = userManagerClient;
            _logger = loggerFactory.CreateLogger<ProfileController>();
        }

        [HttpGet("current")]
        [ProducesResponseType(typeof(UserProfile), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName(nameof(GetCurrentProfile))]
        public async Task<ActionResult<UserProfile>> GetCurrentProfile()
        {
            var userContext = await userService.GetCurrentUserContext();
            if (userContext == null) 
            { 
                return NotFound(""); 
            }

            string emailAddress = userContext.Email;
            string firstName = userContext.FirstName;
            string lastName = userContext.LastName;

            if (userContext.DriverId != null)
            {
                var driverIdRequest = new DriverIdRequest() { Id = userContext.DriverId };
                var reply = _cmsAdapterClient.GetDriverById(driverIdRequest);

                if (reply.ResultStatus == ResultStatus.Success && reply.Items != null && reply.Items.Count > 0)
                {
                    var driverRecord = reply.Items.FirstOrDefault();
                    if (driverRecord != null)
                    {
                        firstName = driverRecord.GivenName;
                        lastName = driverRecord.Surname;
                    }
                }
            }

            return new UserProfile
            {
                Id = userContext.Id,
                EmailAddress = emailAddress,
                FirstName = firstName,
                LastName = lastName,                
                DriverId = userContext.DriverId,
                DriverLicenseNumber = userContext.DriverLicenseNumber
            };
        }

        /// <summary>
        /// user registration which sets the user's profile email
        /// </summary>
        /// <param name="newEmail"></param>
        /// <returns></returns>
        [HttpPut("register")]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName(nameof(Register))]
        public async Task<ActionResult> Register([FromBody] UserRegistration userRegistration)
        {
            var profile = await userService.GetCurrentUserContext();
            if (profile == null) 
            { 
                return NotFound(); 
            }

            // verify user registration name and birth date
            var driverLicenseRequest = new DriverLicenseRequest();
            driverLicenseRequest.DriverLicenseNumber = userRegistration.DriverLicenseNumber;
            var getDriverReply = _cmsAdapterClient.GetDriverPerson(driverLicenseRequest);
            if (getDriverReply.ResultStatus != ResultStatus.Success)
            {
                _logger.LogError($"{nameof(Register)} failed.\n {0}", getDriverReply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, getDriverReply.ErrorDetail);
            }
            if (getDriverReply.Items?.Count == 0)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "No driver found.");
            }
            CaseManagement.Service.Driver foundDriver = null;

            // parse birthdate
            if (!DateTime.TryParse(profile.BirthDate, out DateTime claimBirthDate))
            {
                _logger.LogError($"{nameof(Register)} could not parse DL.");
                return StatusCode((int)HttpStatusCode.Unauthorized, "No driver found.");
            }

            foreach (var driver in getDriverReply.Items)
            {
                if (string.IsNullOrEmpty(_configuration["ENABLE_ICBC"]))
                {
                    foundDriver = driver;
                }
                else
                {
                    DateTime driverBirthDate = new DateTime(driver.BirthDate.ToDateTimeOffset().Year,
                        driver.BirthDate.ToDateTimeOffset().Month, driver.BirthDate.ToDateTimeOffset().Day);

                    if (profile.FirstName == driver.GivenName && profile.LastName == driver.Surname && claimBirthDate.Date == driver.BirthDate.ToDateTime().Date)
                    {
                        foundDriver = driver;
                    }
                }
                    
            }
            if (foundDriver == null)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "Driver details do not match.");
            }

            // update driver email
            var request = new SetDriverLoginRequest();
            request.LoginId = profile.Id;
            request.DriverId = foundDriver.Id;

            var setDriverReply = _userManagerClient.SetDriverLogin(request);
            if (setDriverReply.ResultStatus != ResultStatus.Success) 
            {
                _logger.LogError($"{nameof(Register)}.{nameof(UserManager.UserManagerClient.SetDriverLogin)} failed.\n {0}", setDriverReply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, setDriverReply.ErrorDetail);
            }

            // update driver email, create new login if no login exists
            var updateLoginRequest = new UpdateLoginRequest();
            updateLoginRequest.LoginId = profile.Id;
            updateLoginRequest.Email = userRegistration.Email;
            updateLoginRequest.NotifyByMail = userRegistration.NotifyByMail;
            updateLoginRequest.NotifyByEmail = userRegistration.NotifyByEmail;
            updateLoginRequest.ExternalUserName = profile.DisplayName;
            updateLoginRequest.Address = userRegistration.Address;

            setDriverReply = _userManagerClient.UpdateLogin(updateLoginRequest);
            if (setDriverReply.ResultStatus != ResultStatus.Success)
            {
                _logger.LogError($"{nameof(UserRegistration)}.{nameof(UserManager.UserManagerClient.UpdateLogin)} failed.\n {0}", setDriverReply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, setDriverReply.ErrorDetail);
            }

            userService.UpdateClaim(ClaimTypes.Email, userRegistration.Email);

            return Ok();
        }

        // set the user's profile email
        [HttpPut("driver")]
        [Authorize(Policy = Policy.Driver)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName(nameof(UpdateDriver))]
        public async Task<ActionResult> UpdateDriver([FromBody] DriverUpdate request)
        {
            var profile = await userService.GetCurrentUserContext();
            if (profile == null) 
            { 
                return NotFound(); 
            }

            var updateEmailRequest = new UserSetEmailRequest();
            updateEmailRequest.Email = request.Email;
            updateEmailRequest.LoginId = profile.Id;
            var reply = _userManagerClient.UpdateEmail(updateEmailRequest);
            if (reply.ResultStatus != ResultStatus.Success)
            {
                _logger.LogError($"{nameof(UpdateDriver)}.{nameof(UserManager.UserManagerClient.UpdateEmail)} failed.\n {0}", reply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail);
            }

            userService.UpdateClaim(ClaimTypes.Email, request.Email);

            return Ok();
        }

        public record EmailUpdate
        {
            public string Email { get; set; }
        }

        public record DriverUpdate
        {
            public string? DriverLicenseNumber { get; set; }
            public string Email { get; set; }
            public bool NotifyByMail { get; set; }
            public bool NotifyByEmail { get; set; }
            public FullAddress? Address { get; set; }
        }

        public record UserProfile
        {
            public string EmailAddress { get; set; }
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string DriverId { get; set; }
            public string DriverLicenseNumber { get; set; }
        }
    }
}
