﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : ControllerBase
    {
        private readonly ICaseQueryService caseQueryService;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentManager.DocumentManagerClient _documentManagerClient;
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
        public async Task<ActionResult> SearchCaseByIdCode([Required][FromRoute] string idCode)
        {
            var result = new PatientCase();

            if (string.IsNullOrEmpty(idCode) || idCode == Guid.Empty.ToString())
            {
                return BadRequest("Case Number  was invalid.");
            }

            
             var c = await _cmsAdapterClient.GetCaseByIdCodeAsync(new GetCaseByIdCodeRequest { IdCode = idCode});
            if (c != null && c.ResultStatus == ResultStatus.Success)
            {
                result.CaseId = c.Item.CaseId;
                //result.DmerType = c.Item.DmerType;
                //result.Status = c.Item.Status;
                //result.Name = c.Item.Name;
                result.DriverLicenseNumber = c.Item.DriverLicenseNumber;
                
                result.IdCode = c.Item.IdCode;
                result.FirstName = c.Item.FirstName;
                result.LastName = c.Item.LastName;
                result.MiddleName = c.Item.Middlename;

                if (c.Item.BirthDate != null)
                {
                    result.BirthDate = c.Item.BirthDate.ToDateTime();
                }

                if (c.Item.LatestComplianceDate != null)
                {
                    result.LatestComplianceDate = c.Item.LatestComplianceDate.ToDateTimeOffset();
                }


            }

            // set to null if no decision has been made.
            if (result.BirthDate == DateTime.MinValue)
            {
                result.BirthDate = null;
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
                //result.DmerType = c.Item.DmerType;
                //result.Status = c.Item.Status;
                //result.Name = c.Item.AssigneeTitle;
                result.DriverLicenseNumber = c.Item.DriverLicenseNumber;
                result.BirthDate = c.Item.BirthDate.ToDateTime();
                result.IdCode = c.Item.IdCode;
                result.FirstName = c.Item.FirstName;
                result.LastName = c.Item.LastName;
                result.MiddleName = c.Item.Middlename;
                result.DriverId = c.Item.DriverId;
                result.LatestComplianceDate = c.Item.LatestComplianceDate.ToDateTimeOffset();

                // get dmer documents for the case
                var caseDmerDocumentsRequest = new GetDriverAndCaseDocumentsRequest
                {
                    CaseId = c.Item.CaseId
                };
                var caseDmerDocuments = await _documentManagerClient.GetDriverAndCaseDocumentsAsync(caseDmerDocumentsRequest);
                if (caseDmerDocuments != null && caseDmerDocuments.ResultStatus == ResultStatus.Success)
                {
                    result.Documents = _mapper.Map<IEnumerable<ViewModels.Document>>(caseDmerDocuments.Items);
                }
            }

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