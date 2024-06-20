using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.IcbcAdapter;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using static Rsbc.Dmf.CaseManagement.Service.DocumentManager;
using static Rsbc.Dmf.IcbcAdapter.IcbcAdapter;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : ControllerBase
    {
        private readonly ICaseQueryService caseQueryService;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentManagerClient _documentManagerClient;
        private readonly ICachedIcbcAdapterClient _icbcAdapterClient;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CasesController(ICaseQueryService caseQueryService, CaseManager.CaseManagerClient cmsAdapterClient, DocumentManagerClient documentManagerClient, ICachedIcbcAdapterClient icbcAdapterClient, IMapper mapper, ILoggerFactory loggerFactory)
        {
            this.caseQueryService = caseQueryService;
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
                result.DmerType = document.Item?.DmerType ?? string.Empty;
                result.Status = document.Item?.Status ?? string.Empty;
                result.Name = document.Item?.Provider?.Name ?? string.Empty;
                result.DriverLicenseNumber = @case.Item.DriverLicenseNumber;
                result.IdCode = @case.Item.IdCode;

                result.LatestComplianceDate = @case.Item.LatestComplianceDate?.ToDateTimeOffset();

                // get driver info from ICBC
                if (@case.Item.DriverLicenseNumber != null)
                {
                    //result.DriverLicenseNumber = @case.Item.DriverLicenseNumber;
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

        [HttpGet("{caseId}")]
        [ProducesResponseType(typeof(PatientCase), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetCaseById")]
        public async Task<ActionResult> GetCaseById([Required][FromRoute] string caseId)
        {
            var result = new PatientCase();

            if (string.IsNullOrEmpty(caseId) || caseId == Guid.Empty.ToString())
            {
                return BadRequest("Case id was invalid.");
            }

            var c = _cmsAdapterClient.GetCaseDetail(new CaseIdRequest { CaseId = caseId });
            if (c != null && c.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                result.CaseId = c.Item.CaseId;
                result.DmerType = c.Item.DmerType;
                result.Status = c.Item.Status;
                result.Name = c.Item.AssigneeTitle;
                result.DriverLicenseNumber = c.Item.DriverLicenseNumber;
                result.BirthDate = c.Item.BirthDate.ToDateTime();
                result.IdCode = c.Item.IdCode;
                result.FirstName = c.Item.FirstName;
                result.LastName = c.Item.LastName;
                result.MiddleName = c.Item.Middlename;
                result.DriverId = c.Item.DriverId;
                result.LatestComplianceDate = c.Item.LatestComplianceDate.ToDateTimeOffset();
            }
            // TODO handle failure

            // set to null if no decision has been made.
            if (result.BirthDate == DateTime.MinValue)
            {
                result.BirthDate = null;
            }

            return Ok(result);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<DmerCaseListItem>>> GetCases([FromQuery] CaseSearchQuery query)
        {
            var cases = await caseQueryService.SearchCases(query);

            // second pass to populate birthdate.

            return Ok(cases);
        }

      
    }
}