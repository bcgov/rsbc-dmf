using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly DocumentManager.DocumentManagerClient _documentManagerClient;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(DocumentManager.DocumentManagerClient documentManagerClient, IUserService userService, IMapper mapper, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _documentManagerClient = documentManagerClient;
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<DocumentController>();
        }

        [HttpGet("MyDmers")]
        // TODO
        //[Authorize(Policy = Policy.MedicalPractitioner)]
        [ProducesResponseType(typeof(IEnumerable<CaseDocument>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMyDocumentsByType()
        {
            var profile = await _userService.GetCurrentUserContext();

            // TODO we need to set this before this controller method will work
            var loginIds = profile.LoginIds;

            var dmerDocumentTypeCode = _configuration["Constants:DmerDocumentTypeCode"];
            var request = new GetDocumentsByTypeForUsersRequest { DocumentTypeCode = dmerDocumentTypeCode, LoginIds = { loginIds } };
            var reply = _documentManagerClient.GetDocumentsByTypeForUsers(request);
            if (reply.ResultStatus == ResultStatus.Success)
            {
                var caseDocuments = _mapper.Map<IEnumerable<CaseDocument>>(reply.Items);
                return Ok(caseDocuments);
            }
            else
            {
                // TODO
                _logger.LogError($"{nameof(GetMyDocumentsByType)} error: unable to get documents by type - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }
        }

        [HttpGet("GetDriverAndCaseDocuments")]
       // TODO
        //[Authorize(Policy = Policy.MedicalPractitioner)]
        [ProducesResponseType(typeof(IEnumerable<Document>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetDriverAndCaseDocuments")]
        public async Task<IActionResult> GetDriverAndCaseDocuments([FromRoute] string caseId)
        {
            var profile = await _userService.GetCurrentUserContext();
            var loginIds = profile.LoginIds;

            // TODO # Change the loginId to claim.loginid 
            var request = new GetDriverAndCaseDocumentsRequest { CaseId = caseId, LoginId = loginIds.FirstOrDefault() };

            var reply = _documentManagerClient.GetDriverAndCaseDocuments(request);
            if (reply.ResultStatus == ResultStatus.Success)
            {
                // This includes all the documents except Open Required, Issued, Sent documents on Submission History Tab
                var replyItemsWithDocuments = reply.Items
                    .Where(i => !string.IsNullOrEmpty(i.DocumentUrl))
                    .Where(i => i.SubmittalStatus != "Open-Required" && i.SubmittalStatus != "Issued" && i.SubmittalStatus != "Sent");
                var result = _mapper.Map<List<CaseDocument>>(replyItemsWithDocuments);

                // Sort The documents

                if(result.Count > 0)
                {
                    result = result.OrderByDescending(doc =>doc.CreatedOn).ToList();
                }
                
                return Json(result);

               

            }
             else
            {
               
                _logger.LogError($"{nameof(GetDriverAndCaseDocuments)} error: unable to get documents for this case - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }

        }
    }
}
