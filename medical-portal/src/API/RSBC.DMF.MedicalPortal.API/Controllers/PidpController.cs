using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PidpAdapter;
using RSBC.DMF.MedicalPortal.API.Services;
using System.Net.Http.Headers;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PidpController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly MedicalPortalConfiguration _configuration;
        private readonly PidpManager.PidpManagerClient _pidpAdapterClient;

        public PidpController(IHttpContextAccessor httpContextAccessor, PidpManager.PidpManagerClient pidpAdapterClient, IUserService userService, MedicalPortalConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _pidpAdapterClient = pidpAdapterClient;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet("endorsements")]
        [ProducesResponseType(typeof(JsonResult), 200)] // change this after converted to GRPC
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetMyEndorsements))]
        [AllowAnonymous]
        public async Task<ActionResult> GetMyEndorsements()
        {
            try
            {
                var profile = await _userService.GetCurrentUserContext();
                var userId = profile.Id;

                // TODO remove fake data after we are unblocked from pidp endorsements
                userId = "test";

                var response = await _pidpAdapterClient.GetEndorsementsAsync(new GetEndorsementsRequest { UserId = userId });
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
