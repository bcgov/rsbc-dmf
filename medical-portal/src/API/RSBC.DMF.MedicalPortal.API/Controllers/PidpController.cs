using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PidpAdapter;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System.Net;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PidpController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly MedicalPortalConfiguration _configuration;
        private readonly PidpManager.PidpManagerClient _pidpAdapterClient;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PidpController(IHttpContextAccessor httpContextAccessor, PidpManager.PidpManagerClient pidpAdapterClient, IUserService userService, IMapper mapper, MedicalPortalConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _pidpAdapterClient = pidpAdapterClient;
            _mapper = mapper;
            _configuration = configuration;
            _userService = userService;
            _logger = loggerFactory.CreateLogger<PidpController>();
        }

        [HttpGet("endorsements")]
        [ProducesResponseType(typeof(IEnumerable<Endorsement>), 200)] // change this after converted to GRPC
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetMyEndorsements))]
        public async Task<ActionResult> GetMyEndorsements()
        {
            try
            {
                var profile = await _userService.GetCurrentUserContext();
                var userId = profile.Id;

                var reply = await _pidpAdapterClient.GetEndorsementsAsync(new GetEndorsementsRequest { UserId = userId });
                if (reply.ResultStatus == ResultStatus.Fail)
                {
                    _logger.LogError($"{nameof(GetMyEndorsements)} error: unable to get endorsements - {reply.ErrorDetail}");
                    return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail);
                }
                var endorsements = _mapper.Map<IEnumerable<Endorsement>>(reply.Endorsements);

                // TODO remove this temporary hack
                var i = 1;
                foreach (var endorsement in endorsements)
                {
                    endorsement.FullName = $"PRACTITIONER FAKE{i}";
                    endorsement.Email = $"fake.prac{i}@mailinator.com";
                    endorsement.Role = "Practitioner";
                    i++;
                }

                return Ok(endorsements);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
