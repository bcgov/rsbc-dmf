
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // public API
    public class CasesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;

        public CasesController(IConfiguration configuration, CaseManager.CaseManagerClient cmsAdapterClient, DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient)
        {
            _configuration = configuration;
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
        }

        /// <summary>
        /// Get Case
        /// </summary>        
        [HttpGet("{caseId}")]
        [ProducesResponseType(typeof(ViewModels.CaseDetail), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetCase")]
        public ActionResult GetCase([Required][FromRoute] string caseId)
        {
            var result = new ViewModels.CaseDetail();

            var c = _cmsAdapterClient.GetCaseDetail(new CaseIdRequest { CaseId = caseId });
            if (c != null && c.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                string caseType = "Unsolicited";

                if (c.Item.CaseType == "DMER")
                {
                    caseType = "Solicited";
                }

                result.CaseId = c.Item.CaseId;
                result.Title = c.Item.Title;
                result.IdCode = c.Item.IdCode;
                result.OpenedDate = c.Item.OpenedDate.ToDateTimeOffset();
                result.CaseType = caseType;
                result.DmerType = c.Item.DmerType;
                result.Status = c.Item.Status;
                result.AssigneeTitle = c.Item.AssigneeTitle;
                result.LastActivityDate = c.Item.LastActivityDate.ToDateTimeOffset();
                result.LatestDecision = c.Item.LatestDecision;
                result.DecisionForClass = c.Item.DecisionForClass;
                result.DecisionDate = c.Item.DecisionDate.ToDateTimeOffset();
                if (c.Item.CaseSequence > -1)
                {
                    result.CaseSequence = (int)c.Item.CaseSequence;
                }
                result.DpsProcessingDate = c.Item.DpsProcessingDate.ToDateTimeOffset();
                
            }

            // set to null if no decision has been made.
            if (result.DecisionDate == DateTimeOffset.MinValue)
            {
                result.DecisionDate = null;
            }
            return Json(result);
        }

    }
    
}
