using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter.Client;
using Rsbc.Dmf.PartnerPortal.Api.Services;
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

        public record UserProfile
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string LoginId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            //public IEnumerable<string> Roles { get; set; }
        }

    }
}
