using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PidpController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PidpController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("endorsements")]
        public async Task<ActionResult> GetEndorsements()
        {
            // TODO temp code until this is replaced with GRPC
            var bearerToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var responseStream = await httpClient.GetAsync("https://localhost:7215/api/contacts/kkpqtjseoyaygbqxmjq7kltol7wffrn6@bcsc/endorsements");
            var response = await responseStream.Content.ReadAsStringAsync();
            return new JsonResult(response);
        }
    }
}
