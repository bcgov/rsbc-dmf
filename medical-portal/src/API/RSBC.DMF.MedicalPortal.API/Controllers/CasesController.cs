using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Net;
using static Rsbc.Dmf.CaseManagement.Service.DocumentManager;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentManagerClient _documentManagerClient;
        private readonly ICachedIcbcAdapterClient _icbcAdapterClient;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CasesController(IUserService userService, CaseManager.CaseManagerClient cmsAdapterClient, DocumentManagerClient documentManagerClient, ICachedIcbcAdapterClient icbcAdapterClient, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _userService = userService;
            _cmsAdapterClient = cmsAdapterClient;
            _documentManagerClient = documentManagerClient;
            _icbcAdapterClient = icbcAdapterClient;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<CasesController>();
        }

        [HttpGet("search/{idCode}")]
        [ProducesResponseType(typeof(PatientCase), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("SearchCaseByIdCode")]
        public async Task<ActionResult> SearchCaseByIdCode([Required][FromRoute] string idCode)
        {
            var profile = await _userService.GetCurrentUserContext();

            PatientCase result = null;

            if (string.IsNullOrEmpty(idCode) || idCode == Guid.Empty.ToString())
            {
                return BadRequest("The Case Number was invalid.");
            }

            // get case by id code
            var getCaseByIdCodeRequest = new GetCaseByIdCodeRequest { IdCode = idCode };
            var @case = await _cmsAdapterClient.GetCaseByIdCodeAsync(getCaseByIdCodeRequest);
            if (@case?.Item?.CaseId == null)
            {
                return NotFound();
            }
            if (@case.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Fail)
            {
                _logger.LogError($"Error getting case by id code: {0}", @case.ErrorDetail);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to get case.");
            }

            // get DMER
            var getDmerRequest = new CaseIdRequest { CaseId = @case.Item.CaseId };
            var document = await _documentManagerClient.GetDmerAsync(getDmerRequest);
            if (document.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Fail)
            {
                _logger.LogError($"Error getting dmer: {0}", @case.ErrorDetail);
            }

            if (@case != null && @case.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                result = new PatientCase();

                result.CaseId = @case.Item.CaseId;
                result.DmerType = string.IsNullOrEmpty(@case.Item?.DmerType) ? "Suspected Medical Condition" : @case.Item.DmerType;
                result.Status = string.IsNullOrEmpty(document.Item?.Status) ? "Not Requested" : document.Item?.Status;
                result.Status = TranslateDmerStatus(result.Status, document.Item?.Provider?.Id);
                result.IsOwner = document.Item?.Provider?.Id == profile.Id;
                result.Name = document.Item?.Provider?.Name ?? string.Empty;
                result.DriverLicenseNumber = @case.Item.DriverLicenseNumber;
                result.IdCode = @case.Item.IdCode;
                result.LatestComplianceDate = @case.Item.LatestComplianceDate?.ToDateTimeOffset();
                result.DriverId = @case.Item.DriverId;
                result.DocumentId = document.Item?.DocumentId ?? string.Empty;
                
                // get driver info from ICBC
                if (@case.Item.DriverLicenseNumber != null)
                {
                    result.DriverLicenseNumber = @case.Item.DriverLicenseNumber;
                    var request = new DriverInfoRequest();
                    request.DriverLicence = @case.Item.DriverLicenseNumber;
                    var driverInfoReply = await _icbcAdapterClient.GetDriverInfoAsync(request);
                    if (driverInfoReply.ResultStatus == Rsbc.Dmf.IcbcAdapter.ResultStatus.Fail)
                    {
                        _logger.LogError($"Failed to get icbc driver info details: {0}", driverInfoReply.ErrorDetail);
                        return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to get icbc driver info details.");
                    }
                    result.FirstName = driverInfoReply.GivenName;
                    result.LastName = driverInfoReply.Surname;
                    DateTime.TryParse(driverInfoReply.BirthDate, out DateTime parsedBirthdate);
                    result.BirthDate = parsedBirthdate;
                }
            }

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        private string TranslateDmerStatus(string dmerStatus, string loginId)
        {
           

            if (dmerStatus == "Open-Required")
            {
                if (string.IsNullOrEmpty(loginId))
                {
                    dmerStatus = "Required - Unclaimed";
                }
                else
                {
                    dmerStatus = "Required - Claimed";
                }
            }

            if(dmerStatus == "Non-Comply")
            {
                if (string.IsNullOrEmpty(loginId))
                {
                    dmerStatus = "Non-Comply - Unclaimed";
                }
                else
                {
                    dmerStatus = "Non-Comply - Claimed";
                }
            }
            return dmerStatus;
        } 
    }
}
