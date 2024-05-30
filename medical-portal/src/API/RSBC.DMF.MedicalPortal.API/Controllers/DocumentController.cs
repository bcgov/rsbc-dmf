using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using Rsbc.Dmf.CaseManagement.Service;
using System.Reflection.Emit;

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
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;

        public DocumentController(DocumentManager.DocumentManagerClient documentManagerClient, IUserService userService, IMapper mapper, IConfiguration configuration, ILoggerFactory loggerFactory, CaseManager.CaseManagerClient cmsAdapterClient)
        {
            _documentManagerClient = documentManagerClient;
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<DocumentController>();
            _cmsAdapterClient = cmsAdapterClient;
        }

        [HttpGet("MyDmers")]
        [ProducesResponseType(typeof(IEnumerable<CaseDocument>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMyDocumentsByType()
        {
            var profile = await _userService.GetCurrentUserContext();
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
                _logger.LogError($"{nameof(GetMyDocumentsByType)} error: unable to get documents by type - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }
        }

        [HttpGet("GetDriverAndCaseDocuments")]
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

                if (result.Count > 0)
                {
                    result = result.OrderByDescending(doc => doc.CreatedOn).ToList();
                }

                return Json(result);
            }
            else
            {

                _logger.LogError($"{nameof(GetDriverAndCaseDocuments)} error: unable to get documents for this case - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }

        }

        /// <summary>
        /// Get all documents for a given driver but filter out documents without a url
        /// </summary>
        /// <returns>IEnumerable&lt;Document&gt;</returns>
        [HttpGet("{driverId}/AllDocuments")]
        [ProducesResponseType(typeof(IEnumerable<CaseDocument>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetAllDocuments")]
        public async Task<ActionResult> GetAllDocuments([FromRoute] string driverId)
        {
            var profile = await _userService.GetCurrentUserContext();

            var driverIdRequest = new DriverIdRequest() { Id = driverId };
            var reply = _cmsAdapterClient.GetDriverDocumentsById(driverIdRequest);
            if (reply != null && reply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                // This includes all the documents except Open Required, Issued, Sent documents on Submission History Tab
                var replyItemsWithDocuments = reply.Items
                    .Where(i => i.SubmittalStatus != "Open-Required" && i.SubmittalStatus != "Issued" && i.SubmittalStatus != "Sent");
                var result = _mapper.Map<List<CaseDocument>>(replyItemsWithDocuments);

                // sort the documents
                if (result.Count > 0)
                {
                    result = result.OrderByDescending(cs => cs.CreatedOn).ToList();
                }

                return Json(result);
            }
            else
            {
                _logger.LogError($"{nameof(GetAllDocuments)} failed for driverId: {driverId}", reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }
        }


    }
}
