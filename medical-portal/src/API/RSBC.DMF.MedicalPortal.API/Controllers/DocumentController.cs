using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System.Collections;
using System.Collections.Generic;
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

        public DocumentController(DocumentManager.DocumentManagerClient documentManagerClient, IUserService userService, IMapper mapper)
        {
            _documentManagerClient = documentManagerClient;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("Type/{documentTypeCode}")]
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

            // TODO add setting for document type code "001"
            var request = new GetDocumentsByTypeForUsersRequest { DocumentTypeCode = "001", LoginIds = { loginIds } };
            var reply = _documentManagerClient.GetDocumentsByTypeForUsers(request);
            if (reply.ResultStatus == ResultStatus.Success)
            {
                var caseDocuments = _mapper.Map<IEnumerable<CaseDocument>>(reply.Items);
                return Ok(caseDocuments);
            }
            else
            {
                // TODO
                //_logger.LogError($"Unexpected error - unable to get documents meta-data for id {documentId} - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }
        }
    }
}
