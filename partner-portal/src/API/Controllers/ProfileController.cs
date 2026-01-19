using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter.Client;
using Rsbc.Dmf.PartnerPortal.Api.Services;
using Rsbc.Dmf.PartnerPortal.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PartnerPortal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly UserManager.UserManagerClient _userManagerClient;
        private readonly ICachedIcbcAdapterClient _icbcAdapterClient;
        private readonly IUserService _userService;
        private readonly ILogger<ProfileController> _logger;
        private readonly IConfiguration _configuration;

        public ProfileController(CaseManager.CaseManagerClient cmsAdapterClient, UserManager.UserManagerClient userManagerClient, ICachedIcbcAdapterClient icbcAdapterClient, IUserService userService, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _userService = userService;
            _cmsAdapterClient = cmsAdapterClient;
            _icbcAdapterClient = icbcAdapterClient;
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
            var userContext = await _userService.GetCurrentUserContext();
            if (userContext == null)
            {
                return NotFound("");
            }

            return new UserProfile
            {
                Id = userContext.UserId,
                DisplayName = userContext.DisplayName,
                FirstName = userContext.FirstName,
                LastName = userContext.LastName,
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
            // Step 1 : Search for the contacts (Find with ID or names)

            var userContactRequest = new GetUserContactRequest();
            userContactRequest.ExternalSystemUserId = profile.UserId;
            var getContactReply = await _userManagerClient.GetUserContactAsync(userContactRequest);
            if (getContactReply.ResultStatus == ResultStatus.Success && getContactReply.Contact != null)
            {
                // Contact already exists. Link the existing contact to the login (ensure login -> contact association).
                _logger.LogInformation($"{nameof(Register)}: contact already exists. Linking login to contact (ContactId={getContactReply.Contact.ContactId}).");

                var setUserContactLoginRequest = new SetUserContactLoginRequest
                {
                    LoginId = profile.UserId,
                    ContactId = getContactReply.Contact.ContactId ?? string.Empty,
                    LoginType = profile.IdentityProvider
                };

                var setContactLoginReply = await _userManagerClient.SetUserContactLoginAsync(setUserContactLoginRequest);

                if (setContactLoginReply.ResultStatus != ResultStatus.Success)
                {
                    _logger.LogError($"{nameof(Register)}: failed linking login to existing contact. {setContactLoginReply.ErrorDetail}");
                    return StatusCode((int)HttpStatusCode.BadRequest, setContactLoginReply.ErrorDetail);
                }

                // return existing contact back to caller
                return StatusCode((int)HttpStatusCode.OK, getContactReply.Contact);
            }



            // Step 2: If contact does not Exists create contact
            var createContactRequest = new UserContactRequest();
            createContactRequest.Contact = new UserContact();
            createContactRequest.Contact.ExternalSystemUserId = profile.UserId;
            createContactRequest.Contact.GivenName = userRegistration.GivenName;
            createContactRequest.Contact.SecondGivenName = userRegistration.SecondGivenName ?? string.Empty;
            createContactRequest.Contact.ThirdGivenName = userRegistration.ThirdGivenName ?? string.Empty;
            createContactRequest.Contact.Surname = userRegistration.SurName;
            createContactRequest.Contact.AddressFirstLine = userRegistration.AddressFirstLine;
            createContactRequest.Contact.AddressSecondLine = userRegistration.AddressSecondLine;
            createContactRequest.Contact.AddressThirdLine = userRegistration.AddressThirdLine;
            createContactRequest.Contact.City = userRegistration.City;
            createContactRequest.Contact.Province = userRegistration.Province;
            createContactRequest.Contact.Country = userRegistration.Country;
            createContactRequest.Contact.PostalCode = userRegistration.PostalCode;
            createContactRequest.Contact.PhoneNumber = userRegistration.PhoneNumber;
            createContactRequest.Contact.CellPhoneNumber = userRegistration.CellPhoneNumber;
            createContactRequest.Contact.EmailAddress = userRegistration.EmailAddress;


            var createContactReply = await _userManagerClient.CreateUserContactAsync(createContactRequest);
            if (createContactReply.ResultStatus != CaseManagement.Service.ResultStatus.Success)
            {
                _logger.LogError($"{nameof(Register)} could not create Contact.");
                return StatusCode((int)HttpStatusCode.BadRequest, "No Contact found.");
            }

            if (string.IsNullOrEmpty(createContactReply.ContactId))
            {
                _logger.LogError($"{nameof(Register)} Contact Id is null");
                return StatusCode((int)HttpStatusCode.BadRequest, "Contact Id is null.");
            }

            // Step 3: If contact is found link the contact with the login table
            var userLoginRequest = new SetUserContactLoginRequest();
            userLoginRequest.LoginId = profile.UserId;
            userLoginRequest.ContactId = createContactReply.ContactId;
            userLoginRequest.LoginType = profile.IdentityProvider;
            var setUserContactLoginReply = await _userManagerClient.SetUserContactLoginAsync(userLoginRequest);
            if (setUserContactLoginReply.ResultStatus != CaseManagement.Service.ResultStatus.Success)
            {
                _logger.LogError($"{nameof(Register)}.{nameof(UserManager.UserManagerClient.SetDriverLogin)} failed.\n {0}", setUserContactLoginReply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.BadRequest, setUserContactLoginReply.ErrorDetail);
            }

            return StatusCode((int)HttpStatusCode.OK, getContactReply.Contact);

        }

        public record UserProfile
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string LoginId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

    }
}
