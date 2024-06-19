using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static Rsbc.Dmf.CaseManagement.Service.DocumentManager;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : ControllerBase
    {
        private readonly ICaseQueryService caseQueryService;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentManagerClient _documentManagerClient;
        private readonly IMapper _mapper;

        public CasesController(ICaseQueryService caseQueryService, CaseManager.CaseManagerClient cmsAdapterClient, DocumentManager.DocumentManagerClient documentManagerClient, IMapper mapper)
        {
            this.caseQueryService = caseQueryService;
            _cmsAdapterClient = cmsAdapterClient;
            _documentManagerClient = documentManagerClient;
            _mapper = mapper;
        }

        [HttpGet("search/{idCode}")]
        [ProducesResponseType(typeof(PatientCase), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("SearchCaseByIdCode")]
        public ActionResult SearchCaseByIdCode([Required][FromRoute] string idCode)
        {
            PatientCase result = null;

            if (string.IsNullOrEmpty(idCode) || idCode == Guid.Empty.ToString())
            {
                return BadRequest("Case Number  was invalid.");
            }

            // get case by id code
            var getCaseByIdCodeRequest = new GetCaseByIdCodeRequest { IdCode = idCode };
            var @case = _cmsAdapterClient.GetCaseByIdCode(getCaseByIdCodeRequest);

            // get DMER
            var getDmerRequest = new CaseIdRequest { CaseId = @case.Item.CaseId };
            var document = _documentManagerClient.GetDmer(getDmerRequest);

            // TODO get ICBC driver name and birthdate

            if (@case != null && @case.ResultStatus == ResultStatus.Success)
            {
                result = new PatientCase();

                result.CaseId = @case.Item.CaseId;
                result.DmerType = document.Item.DmerType;
                result.Status = document.Item.Status;
                result.Name = document.Item.Provider.Name;
                result.DriverLicenseNumber = @case.Item.DriverLicenseNumber;
                
                result.IdCode = @case.Item.IdCode;
                // TODO get driver info from ICBC instead
                result.FirstName = @case.Item.FirstName;
                result.LastName = @case.Item.LastName;
                result.MiddleName = @case.Item.Middlename;
                
                //if (@case.Item.DriverBirthDate != null)
                //{
                //    result.BirthDate = @case.Item.DriverBirthDate.ToDateTime();
                //}

                if (@case.Item.LatestComplianceDate != null)
                {
                    result.LatestComplianceDate = @case.Item.LatestComplianceDate.ToDateTimeOffset();
                }

                // set to null if no decision has been made.
                if (result.BirthDate == DateTime.MinValue)
                {
                    result.BirthDate = null;
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
            if (c != null && c.ResultStatus == ResultStatus.Success)
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