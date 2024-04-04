using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
using Rsbc.Dmf.DriverPortal.ViewModels;
using Rsbc.Dmf.IcbcAdapter;
using System.Net;
using System.Security.Claims;
using static Rsbc.Dmf.IcbcAdapter.IcbcAdapter;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    // DOMAIN user profile, user

    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly UserManager.UserManagerClient _userManagerClient;
        private readonly IcbcAdapterClient _icbcAdapterClient;
        private readonly IUserService _userService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ProfileController> _logger;
        private readonly IConfiguration _configuration;

        public ProfileController(CaseManager.CaseManagerClient cmsAdapterClient, UserManager.UserManagerClient userManagerClient, IcbcAdapterClient icbcAdapterClient, IUserService userService, ICacheService cacheService, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _userService = userService;
            _cmsAdapterClient = cmsAdapterClient;
            _icbcAdapterClient = icbcAdapterClient;
            _configuration = configuration;
            _userManagerClient = userManagerClient;
            _cacheService = cacheService;
            _logger = loggerFactory.CreateLogger<ProfileController>();
        }

        [HttpGet("current")]
        [ProducesResponseType(typeof(UserProfile), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName(nameof(GetCurrentProfile))]
        public async Task<ActionResult<UserProfile>> GetCurrentProfile()
        {
            var userContext = await _userService.GetCurrentUserContext();
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

                if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success && reply.Items != null && reply.Items.Count > 0)
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
            var profile = await _userService.GetCurrentUserContext();
            if (profile == null) 
            { 
                return NotFound(); 
            }

            // get driver info
            var driverInfoRequest = new DriverInfoRequest();
            driverInfoRequest.DriverLicence = userRegistration.DriverLicenseNumber;
            DriverInfoReply driverInfoReply = null;
            var serviceName = nameof(IcbcAdapterClient.GetDriverInfo);
            if (!_cacheService.TryGetValue(serviceName, driverInfoRequest.DriverLicence, out driverInfoReply))
            {
                driverInfoReply = await _icbcAdapterClient.GetDriverInfoAsync(driverInfoRequest);
                if (driverInfoReply.ResultStatus != IcbcAdapter.ResultStatus.Success)
                {
                    return StatusCode((int)HttpStatusCode.Unauthorized, "No driver found.");
                }
                _cacheService.Set(serviceName, driverInfoRequest.DriverLicence, driverInfoReply);
            }

            // TODO add extension method string.ToDateTime()
            // security validation
            if (string.IsNullOrEmpty(_configuration["DISABLE_ICBC"])) 
            {
                if (!DateTime.TryParse(profile.BirthDate, out DateTime claimBirthDate))
                {
                    _logger.LogError($"{nameof(Register)} could not parse profile birthdate.");
                    return StatusCode((int)HttpStatusCode.Unauthorized, "No driver found.");
                }
                if (!DateTime.TryParse(driverInfoReply.BirthDate, out DateTime replyBirthDate))
                {
                    _logger.LogError($"{nameof(Register)} could not parse ICBC birthdate.");
                    return StatusCode((int)HttpStatusCode.Unauthorized, "No driver found.");
                }
                if (profile.FirstName != driverInfoReply.GivenName || profile.LastName != driverInfoReply.Surname || claimBirthDate.Date != replyBirthDate.Date)
                {
                    return StatusCode((int)HttpStatusCode.Unauthorized);
                }
            }

            var driverLicenseRequest = new DriverLicenseRequest();
            driverLicenseRequest.DriverLicenseNumber = userRegistration.DriverLicenseNumber;
            var getDriverReply = await _cmsAdapterClient.GetDriverPersonAsync(driverLicenseRequest);
            if (getDriverReply.ResultStatus != CaseManagement.Service.ResultStatus.Success)
            {
                _logger.LogError($"{nameof(Register)} failed.\n {0}", getDriverReply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, getDriverReply.ErrorDetail);
            }

            // update driver email
            var request = new SetDriverLoginRequest();
            request.LoginId = profile.Id;
            request.DriverId = getDriverReply.Items.FirstOrDefault()?.Id ?? string.Empty;
            var setDriverReply = await _userManagerClient.SetDriverLoginAsync(request);
            if (setDriverReply.ResultStatus != CaseManagement.Service.ResultStatus.Success) 
            {
                _logger.LogError($"{nameof(Register)}.{nameof(UserManager.UserManagerClient.SetDriverLogin)} failed.\n {0}", setDriverReply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, setDriverReply.ErrorDetail);
            }

            // if no driver exists, we need to create driver with contact person
            if (!setDriverReply.HasDriver)
            {
                var createDriverRequest = new CreateDriverPersonRequest();
                createDriverRequest.LoginId = profile.Id;
                createDriverRequest.GivenName = profile.FirstName;
                createDriverRequest.Surname = profile.LastName;
                createDriverRequest.DriverLicenseNumber = userRegistration.DriverLicenseNumber;
                if (!DateTime.TryParse(profile.BirthDate, out DateTime claimBirthDate))
                {
                    // in theory, this should never happen and only possible if ICBC validation is disabled
                    _logger.LogError($"{nameof(Register)} could not parse profile birthdate.");
                    return StatusCode((int)HttpStatusCode.Unauthorized, "No driver found.");
                }
                createDriverRequest.BirthDate = claimBirthDate.ToUniversalTime().ToTimestamp();
                var createDriverReply = await _cmsAdapterClient.CreateDriverPersonAsync(createDriverRequest);
                if (createDriverReply.ResultStatus != CaseManagement.Service.ResultStatus.Success)
                {
                    _logger.LogError($"{nameof(Register)} could not create driver.");
                    return StatusCode((int)HttpStatusCode.Unauthorized, "No driver found.");
                }
                _userService.UpdateClaim(UserClaimTypes.DriverId, createDriverReply.DriverId);
                _userService.UpdateClaim(UserClaimTypes.DriverLicenseNumber, createDriverRequest.DriverLicenseNumber);
            }

            // update driver email, create new login if no login exists
            var updateLoginRequest = new UpdateLoginRequest();
            updateLoginRequest.LoginId = profile.Id;
            updateLoginRequest.Email = userRegistration.Email;
            updateLoginRequest.NotifyByMail = userRegistration.NotifyByMail;
            updateLoginRequest.NotifyByEmail = userRegistration.NotifyByEmail;
            updateLoginRequest.ExternalUserName = profile.DisplayName;
            updateLoginRequest.Address = userRegistration.Address;
            var updateLoginReply = _userManagerClient.UpdateLogin(updateLoginRequest);
            if (updateLoginReply.ResultStatus != CaseManagement.Service.ResultStatus.Success)
            {
                _logger.LogError($"{nameof(UserRegistration)}.{nameof(UserManager.UserManagerClient.UpdateLogin)} failed.\n {0}", updateLoginReply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, updateLoginReply.ErrorDetail);
            }

            // TODO we probably need to update driver id and driver license number if a new driver was added above
            _userService.UpdateClaim(ClaimTypes.Email, userRegistration.Email);

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
            var profile = await _userService.GetCurrentUserContext();
            if (profile == null) 
            { 
                return NotFound(); 
            }

            var updateEmailRequest = new UserSetEmailRequest();
            updateEmailRequest.Email = request.Email;
            updateEmailRequest.LoginId = profile.Id;
            var reply = _userManagerClient.UpdateEmail(updateEmailRequest);
            if (reply.ResultStatus != CaseManagement.Service.ResultStatus.Success)
            {
                _logger.LogError($"{nameof(UpdateDriver)}.{nameof(UserManager.UserManagerClient.UpdateEmail)} failed.\n {0}", reply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail);
            }

            _userService.UpdateClaim(ClaimTypes.Email, request.Email);

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
