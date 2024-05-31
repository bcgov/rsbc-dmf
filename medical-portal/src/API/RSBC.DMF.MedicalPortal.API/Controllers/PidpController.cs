using Microsoft.AspNetCore.Mvc;
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

        public PidpController(IHttpContextAccessor httpContextAccessor, IUserService userService, MedicalPortalConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet("endorsements")]
        public async Task<ActionResult> GetEndorsements()
        {
            var profile = _userService.GetCurrentUserContext();
            var userId = profile.Id;

            // TODO temp code until this is replaced with GRPC
            var bearerToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Split(" ")[1];

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var responseStream = await httpClient.GetAsync($"{_configuration.Settings.PidpApiUrl}/api/contacts/{userId}/endorsements");
            var response = await responseStream.Content.ReadAsStringAsync();
            return new JsonResult(response);
        }
    }
}
