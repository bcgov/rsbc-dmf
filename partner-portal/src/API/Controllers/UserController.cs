using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter.Client;
using Rsbc.Dmf.PartnerPortal.Api.Services;
using Rsbc.Dmf.PartnerPortal.Api.ViewModels;
using Serilog.Core;
using ResultStatus = Rsbc.Dmf.CaseManagement.Service.ResultStatus;
namespace Rsbc.Dmf.PartnerPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: Controller
    {
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly PortalPartnerUserManager.PortalPartnerUserManagerClient _portalPartnerUserManagerClient;
        private readonly UserManager.UserManagerClient _userManagerClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;


        public UserController(
            IConfiguration configuration,
            CaseManager.CaseManagerClient cmsAdapterClient,
            UserManager.UserManagerClient userManagerClient,
            DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient,
            IUserService userService,
            IMapper mapper,
            ILoggerFactory loggerFactory,
            ICachedIcbcAdapterClient icbcAdapterClient,
            ILogger<UserController> logger,
            PortalPartnerUserManager.PortalPartnerUserManagerClient portalPartnerUserManagerClient
        )
        {
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _userService = userService;
            _mapper = mapper;
            _userManagerClient = userManagerClient;
            _portalPartnerUserManagerClient = portalPartnerUserManagerClient;
            _logger = logger;

        }


        [HttpPost("getUsers")]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.User>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetUsers))]
        public async Task<ActionResult> GetUsers([FromBody] UsersSearchRequest searchRequest)
        {
            var result = new List<ViewModels.User>();

            var profile = _userService.GetDriverInfo();
            try
            {
                var users = await _portalPartnerUserManagerClient.SearchContactsAsync(searchRequest);
                result = _mapper.Map<List<ViewModels.User>>(users.User.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users.");
                throw ex;
            }

            return Json(result);
        }

        [HttpPost("updateUser")]
        [ProducesResponseType(typeof(ViewModels.User), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetUsers))]
        public async Task<ActionResult> UpdateUser([FromBody] ViewModels.User user)
        {

            try
            {
                var result = _mapper.Map<Contact>(user);
                var userContext = await _userService.GetCurrentUserContext();
                result.ModifiedBy = userContext.DisplayName;
                var users = await _portalPartnerUserManagerClient.UpdateContactAsync(result);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user.");
                throw ex;
            }


        }

        [HttpGet("getContactRoles")]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.UserRole>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetContactRoles))]
        public async Task<ActionResult> GetContactRoles()
        {


            try
            {
                var request = new GetContactRolesRequest();
                var roles = await _portalPartnerUserManagerClient.GetContactRolesAsync(request);
                var result = roles.Role.Select(r =>
                {
                   return new UserRole
                    {
                        Name = r.Description,
                        RoleID = r.Name,
                        Id = r.Id
                    };
                });
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact roles.");
                throw ex;
            }

        }

        [HttpPost("updateContactRoles")]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetContactRoles))]
        public async Task<ActionResult> UpdateContactRoles([FromBody] ViewModels.UpdateContactRole user)
        {


            try
            {
                var request = new UpdateContactRoleRequest
                {
                    AddRole = user.AddRole,
                    ContactId = user.ContactId,
                    RoleId = user.RoleId,
                    ModifiedBy = (await _userService.GetCurrentUserContext()).DisplayName
                };
                await _portalPartnerUserManagerClient.UpdateContactRoleAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errorupdating contact role.");
                throw ex;
            }

        }


        [HttpGet("getCurrentLoginDetails")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetCurrentLoginDetails))]
        public async Task<ActionResult> GetCurrentLoginDetails()
        {
            try
            {

                var userId = (await _userService.GetCurrentUserContext()).DisplayName;
                var request = new GetCurrentLoginUserRequest {
                    UserId = userId
                };
                var loginDetails = await _portalPartnerUserManagerClient.GetCurrentLoginUserAsync(request);
                var result = loginDetails.UserRoles;
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user.");
                throw ex;
            }

        }
    }
}
