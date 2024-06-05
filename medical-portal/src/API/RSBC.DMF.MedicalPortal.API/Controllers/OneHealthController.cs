using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneHealthAdapter;
using RSBC.DMF.MedicalPortal.API.Services;
using System.Net.Http.Headers;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OneHealthController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly MedicalPortalConfiguration _configuration;
        private readonly OneHealthManager.OneHealthManagerClient _oneHealthAdapterClient;

        public OneHealthController(IHttpContextAccessor httpContextAccessor, OneHealthManager.OneHealthManagerClient oneHealthAdapterClient, IUserService userService, MedicalPortalConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _oneHealthAdapterClient = oneHealthAdapterClient;
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

                // TODO temp code until this is replaced with GRPC
                /*var bearerToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]
                    .ToString()
                    .Split(" ")[1];

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var responseStream = await httpClient.GetAsync($"{_configuration.Settings.PidpApiUrl}/api/contacts/{userId}/endorsements");
                var response = await responseStream.Content.ReadAsStringAsync();
                */
                
                var response = await _oneHealthAdapterClient.GetEndorsementsAsync(new GetEndorsementsRequest { UserId = userId });
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
