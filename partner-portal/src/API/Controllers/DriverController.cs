using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.IcbcAdapter.Client;
using Rsbc.Dmf.IcbcAdapter;
using System.Net;
using AutoMapper;
using Rsbc.Dmf.PartnerPortal.Api.Services;
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;
using Rsbc.Dmf.CaseManagement.Service;
using Google.Protobuf.WellKnownTypes;

[Route("api/[controller]")]
[ApiController]
public class DriverController : Controller
{
    private readonly ICachedIcbcAdapterClient _icbcAdapterClient;
    private readonly CaseManagerClient _caseManagerClient;
    private readonly DocumentManager.DocumentManagerClient _documentManagerClient;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public DriverController(ICachedIcbcAdapterClient icbcAdapterClient, CaseManagerClient caseManagerClient, DocumentManager.DocumentManagerClient documentManagerClient, IUserService userService, IMapper mapper, ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _icbcAdapterClient = icbcAdapterClient;
        _caseManagerClient = caseManagerClient;
        _userService = userService;
        _mapper = mapper;
        _logger = loggerFactory.CreateLogger<DriverController>();
        _configuration = configuration;
        _documentManagerClient = documentManagerClient;
    }

    [HttpGet("AllDocuments")]
    [ProducesResponseType(typeof(IEnumerable<Rsbc.Dmf.PartnerPortal.Api.ViewModels.Document>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    [ActionName("GetAllDocuments")]
    public async Task<ActionResult> GetAllDocuments()
    {
        var user = _userService.GetDriverInfo();

        var driverIdRequest = new DriverIdRequest() { Id = user.DriverId };
        var reply = _documentManagerClient.GetDriverDocumentsById(driverIdRequest);
        if (reply != null && reply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
        {
            var replyItemsWithDocuments = reply.Items;

            var result = _mapper.Map<List<Rsbc.Dmf.PartnerPortal.Api.ViewModels.Document>>(replyItemsWithDocuments);

            // sort the documents
            if (result.Count > 0)
            {
                result = result.OrderByDescending(cs => cs.CreateDate).ToList();
            }

            return Json(result);
        }
        else
        {
            _logger.LogError($"{nameof(GetAllDocuments)} failed for driverId: {user.DriverId}", reply.ErrorDetail);
            return StatusCode(500, reply.ErrorDetail);
        }
    }

    [HttpGet("info/{driverLicenceNumber}/{surCode}")]
    [ProducesResponseType(typeof(Rsbc.Dmf.PartnerPortal.Api.ViewModels.Driver), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ActionName(nameof(GetHistory))]
    public async Task<ActionResult<Rsbc.Dmf.PartnerPortal.Api.ViewModels.Driver>> GetHistory([FromRoute]string driverLicenceNumber, [FromRoute] string surCode)
    {
        try
        {
            var request = new DriverInfoRequest();
            request.DriverLicence = driverLicenceNumber;
            var reply = await _icbcAdapterClient.GetDriverInfoAsync(request, forceRefresh: true);
            if (reply.ResultStatus != Rsbc.Dmf.IcbcAdapter.ResultStatus.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail ?? $"{nameof(GetHistory)} failed to get driver name.");
            }

            var result = _mapper.Map<Rsbc.Dmf.PartnerPortal.Api.ViewModels.Driver>(reply);
            result.DriverLicenseNumber = driverLicenceNumber;

            // get the driver id
            var driverLicenceRequest = new DriverRequest { DriverLicenseNumber = driverLicenceNumber, SurCode = surCode };
            var getDriverReply = _caseManagerClient.GetDriverByIdAndSurCode(driverLicenceRequest);
            if (getDriverReply.ResultStatus != Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success || getDriverReply.Items?.Count == 0)
            {
                _logger.LogError($"Failed to get icbc driver info details: {0}", getDriverReply.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, getDriverReply.ErrorDetail ?? "Failed to get ICBC driver info details.");
                //return StatusCode((int)HttpStatusCode.InternalServerError, getDriverReply.ErrorDetail ?? $"{nameof(GetHistory)} failed to get driver id.");
            }
            result.Id = getDriverReply.Items.First().Id;

            // TODO this is temporary code until we architect the handling of the driver info
            _userService.SetDriverInfo(result);

            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetHistory)} failed.");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpGet("driverSession")]
    [ProducesResponseType(typeof(UserContext), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ActionName(nameof(GetDriverSession))]
    public ActionResult<UserContext> GetDriverSession()
    {
        return Json(_userService.GetDriverInfo());
    }

    [HttpPost("CreateDriver")]
    [ProducesResponseType(typeof(Rsbc.Dmf.PartnerPortal.Api.ViewModels.DriverCreateRecordResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ActionName(nameof(CreateDriver))]
    public async Task<ActionResult<Rsbc.Dmf.PartnerPortal.Api.ViewModels.DriverCreateRecordResponse>> CreateDriver([FromBody] Rsbc.Dmf.PartnerPortal.Api.ViewModels.DriverCreateRecordRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.DriverLicenceNumber))
        {
            return BadRequest(new Rsbc.Dmf.PartnerPortal.Api.ViewModels.DriverCreateRecordResponse
            {
                Success = false,
                Message = "Driver licence number is required."
            });
        }

        try
        {
            var normalizedDriverLicenceNumber = request.DriverLicenceNumber.Trim();
            var icbcReply = await _icbcAdapterClient.GetDriverInfoAsync(new DriverInfoRequest
            {
                DriverLicence = normalizedDriverLicenceNumber
            }, forceRefresh: true);

            if (icbcReply.ResultStatus != Rsbc.Dmf.IcbcAdapter.ResultStatus.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to get ICBC driver");
            }

            var birthDate = DateTime.TryParse(icbcReply.BirthDate, out var parsedBirthDate)
                ? Timestamp.FromDateTime(DateTime.SpecifyKind(parsedBirthDate, DateTimeKind.Utc))
                : null;

            var createReply = await _caseManagerClient.CreateDriverPersonAsync(new CreateDriverPersonRequest
            {
                DriverLicenseNumber = normalizedDriverLicenceNumber,
                Surname = icbcReply.Surname ?? string.Empty,
                GivenName = icbcReply.GivenName ?? string.Empty,
                BirthDate = birthDate,
                LoginId = string.Empty
            });

            if (createReply.ResultStatus != Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to Create Driver.");
            }

            return Ok(new Rsbc.Dmf.PartnerPortal.Api.ViewModels.DriverCreateRecordResponse
            {
                Success = true,
                Message = "Driver record created successfully. You may search for driver again",
                DriverId = createReply.DriverId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CreateDriver)} failed.");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to Create Driver.");
        }
    }
}
