using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System.Net;

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
        public async Task<ActionResult<UserProfile>> GetCurrentProfile()
        {
            var userContext = await userService.GetCurrentUserContext();
            if (userContext == null) return NotFound("");

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
                DriverId = userContext.DriverId
            };
        }

        /// <summary>
        /// user registration which sets the user's profile email
        /// </summary>
        /// <param name="newEmail"></param>
        /// <returns></returns>
        [HttpPut("register")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
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
                _logger.LogError($"{nameof(UserRegistration)} failed for driverLicenseNumber: {userRegistration.DriverLicenseNumber}.\n {0}", getDriverReply.ErrorDetail);
                return StatusCode(500, getDriverReply.ErrorDetail);
            }
            if (getDriverReply.Items?.Count == 0)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "No driver found.");
            }
            CaseManagement.Service.Driver foundDriver = null;

            // parse birthdate
            if (!DateTime.TryParse(profile.BirthDate, out DateTime claimBirthDate))
            {
                _logger.LogError($"{nameof(Register)} could not parse DL {userRegistration.DriverLicenseNumber} birthDate {profile.BirthDate}.");
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
                    if (profile.FirstName == driver.GivenName && profile.LastName == driver.Surname && claimBirthDate.Date == driver.BirthDate.ToDateTime().Date)
                    {
                        foundDriver = driver;
                    }
                }
                    
            }
            if (foundDriver == null)
            {
                return StatusCode(401, "Driver details do not match.");
            }

            // update driver email, create new login if no login exists
            var userSetEmailRequest = new UserSetEmailRequest();
            userSetEmailRequest.UserId = profile.Id;
            userSetEmailRequest.DriverId = profile.DriverId;
            userSetEmailRequest.Email = userRegistration.Email;
            userSetEmailRequest.NotifyByMail = userRegistration.NotifyByMail;
            userSetEmailRequest.NotifyByEmail = userRegistration.NotifyByEmail;
            var setDriverReply = _userManagerClient.SetDriverLoginAndEmail(userSetEmailRequest);
            if (setDriverReply.ResultStatus != ResultStatus.Success) 
            {
                _logger.LogError($"{nameof(UserRegistration)}.{nameof(UserManager.UserManagerClient.SetDriverLoginAndEmail)} failed for driverLicenseNumber: {userRegistration.DriverLicenseNumber}.\n {0}", setDriverReply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, setDriverReply.ErrorDetail);
            }

            return Ok();
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
