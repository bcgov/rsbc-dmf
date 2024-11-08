using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Services;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using Google.Protobuf;
using Pssg.DocumentStorageAdapter;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;
using Winista.Mime;
using RSBC.DMF.MedicalPortal.API.Model;
using CaseDocument = RSBC.DMF.MedicalPortal.API.ViewModels.CaseDocument;
using Driver = Rsbc.Dmf.CaseManagement.Service.Driver;
using Microsoft.AspNetCore.Authorization;
using static RSBC.DMF.MedicalPortal.API.Auth.AuthConstant;
using System.Net;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly DocumentManager.DocumentManagerClient _documentManagerClient;
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentController> _logger;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly DocumentFactory _documentFactory;

        public DocumentController(DocumentManager.DocumentManagerClient documentManagerClient, IUserService userService, IAuthorizationService authorizationService, IMapper mapper, IConfiguration configuration, ILoggerFactory loggerFactory, CaseManager.CaseManagerClient cmsAdapterClient, DocumentStorageAdapterClient documentStorageAdapterClient, DocumentFactory documentFactory)
        {
            _documentManagerClient = documentManagerClient;
            _userService = userService;
            _authorizationService = authorizationService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<DocumentController>();
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _documentFactory = documentFactory;
        }

        [HttpGet("MyDmers")]
        [ProducesResponseType(typeof(IEnumerable<DmerDocument>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMyDocumentsByType()
        {
            var result = new List<DmerDocument>();

            try
            {
                var profile = await _userService.GetCurrentUserContext();
                var loginIds = profile.LoginIds;
                var loggedInUserName = $"{profile.FirstName} {profile.LastName}";
                
                // add login ids of users in your network
                var networkLoginIds = profile.Endorsements.Select(x => x.LoginId.ToString()).ToList();
                loginIds.AddRange(networkLoginIds);

                var dmerDocumentTypeCode = _configuration["CONSTANTS_DOCUMENT_TYPE_DMER"] ?? "001";                
                var request = new GetDocumentsByTypeForUsersRequest { DocumentTypeCode = dmerDocumentTypeCode, LoginIds = { loginIds } };
                var reply = _documentManagerClient.GetDocumentsByTypeForUsers(request);
                if (reply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
                {
                    var caseDocuments = _mapper.Map<IEnumerable<DmerDocument>>(reply.Items);
                    // Go through the list and map the DMER status

                    foreach( var caseDocument in caseDocuments )
                    {
                      
                        caseDocument.DmerStatus = DmerUtilities.TranslateDmerStatus(caseDocument.DmerStatus, caseDocument.LoginId);
                        caseDocument.LoggedInUserName = loggedInUserName;
                      
                    }
                    return Ok(caseDocuments);
                }
                else
                {
                    _logger.LogError($"{nameof(GetMyDocumentsByType)} error: unable to get documents by type - {reply.ErrorDetail}");
                    return StatusCode(500, reply.ErrorDetail);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error getting DMER's");
                return StatusCode(500, "Bad Request");
            }

           

        }

        [HttpGet("{driverId}/AllDocuments")]
        [ProducesResponseType(typeof(IEnumerable<CaseDocument>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetAllDocuments")]
        public async Task<ActionResult> GetAllDocuments([FromRoute] string driverId)
        {
            var profile = await _userService.GetCurrentUserContext();

            var driverIdRequest = new DriverIdRequest() { Id = driverId };
            var reply = _documentManagerClient.GetDriverDocumentsById(driverIdRequest);
            if (reply != null && reply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                // This includes all the documents except Open Required, Issued, Sent documents on Submission History Tab
                var replyItemsWithDocuments = reply.Items;
             
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

        /// <summary>
        /// Upload Document Content
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpPost("upload")]     
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(UploadDriverDocument))]
        // NOTE an operation filter was started but the generated code was not completed
        // to save time, we opted to just write this api call manually on the frontend
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> UploadDriverDocument([FromForm] IFormFile file, [FromForm] int documentSubTypeId, [FromForm] string driverId)
        {
            // sanity check paramters
            if (string.IsNullOrEmpty(file?.FileName) || file.Length == 0)
            {
                return BadRequest();
            }

            // validate document sub type
            var documentIdRequest = new DocumentIdRequest();
            documentIdRequest.Id = documentSubTypeId;
            var documentSubTypeGuidReply = _documentManagerClient.GetDocumentSubTypeGuid(documentIdRequest);
            if (documentSubTypeGuidReply.ResultStatus != Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                _logger.LogError("ERROR getting document sub type id.\n {0}", documentSubTypeGuidReply.ErrorDetail);
                return StatusCode(500, documentSubTypeGuidReply.ErrorDetail);
            }

            var profile = await _userService.GetCurrentUserContext();

            // read file stream into byte array
            var fileStream = new MemoryStream((int)file.Length);
            file.CopyTo(fileStream);
            var bytes = fileStream.ToArray();

            // verify mimetype from byte array
            var mimeTypes = new MimeTypes();
            var mimeType = mimeTypes.GetMimeType(bytes);
            if (!DocumentUtils.IsAllowedMimeType(mimeType.Name))
            {
                _logger.LogError($"ERROR in uploading file due to invalid mime type {mimeType?.Name}");
                return BadRequest("Invalid file type.");
            }

            // add the document
            var request = new UploadFileRequest()
            {
                ContentType = file.ContentType,
                Data = ByteString.CopyFrom(bytes),
                EntityName = "dfp_driver",
                FileName = file.FileName,
                FolderName = driverId,
            };
            var fileReply = _documentStorageAdapterClient.UploadFile(request);
            if (fileReply.ResultStatus != Pssg.DocumentStorageAdapter.ResultStatus.Success)
            {
                return StatusCode(500, fileReply.ErrorDetail);
            }

            // create document and then link to driver
            var driver = new Driver();
            driver.Id = driverId;

            var document = _documentFactory.Create(driver, profile.Id, fileReply.FileName, "Submitted Document", _configuration["DRIVER_DOCUMENT_TYPE_CODE"]);
            document.DocumentSubTypeId = documentSubTypeGuidReply.Id.ToString();
            var result = _cmsAdapterClient.CreateUnsolicitedDocumentOnDriver(document);
            if (result.ResultStatus != Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                return StatusCode(500, result.ErrorDetail);
            }

            return Ok();
        }

        [HttpPost("claimDmer")]
        [ProducesResponseType(typeof(DmerDocument), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = Policies.MedicalPractitioner)]
        public async Task<IActionResult> UpdateClaimDmerOnDocument([FromQuery] string documentId)
        {
            var profile = await _userService.GetCurrentUserContext();
            var loginId = profile.LoginId;
            return await AssignDmerClaim(loginId, documentId);
        }

        [HttpPost("assignDmer")]
        [ProducesResponseType(typeof(DmerDocument), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignClaimDmerOnDocument([FromQuery] string documentId, [FromQuery] Guid loginId)
        {
            if (loginId == Guid.Empty)
            {
                return BadRequest("Invalid loginId");
            }

            var user = _userService.GetUser();
            var hasAccess = user.IsInRole(Roles.Practitoner);
            // user must be a practitioner (above) or be endorsed by a practitioner (below)
            if (!hasAccess)
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(user, loginId, Policies.NetworkPractitioner);
                hasAccess = authorizationResult.Succeeded;
            }
            if (hasAccess)
            {
                return await AssignDmerClaim(loginId.ToString(), documentId);
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
        }

        [HttpPost("unclaimDmer")]
        [ProducesResponseType(typeof(DmerDocument), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Policy = Policies.MedicalPractitioner)]
        public async Task<IActionResult> UpdateUnclaimDmerOnDocument([FromQuery] string documentId)
        {
            var profile = await _userService.GetCurrentUserContext();
            var loginId = profile.LoginId;

            var request = new UpdateClaimRequest
            {
                LoginId = loginId,
                DocumentId = documentId
            };

            var reply = _documentManagerClient.UpdateUnClaimDmer(request);

            if (reply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                var caseDocument = _mapper.Map<DmerDocument>(reply.Item);
                return Ok(caseDocument);
            }
            else
            {
                _logger.LogError($"{nameof(UpdateUnclaimDmerOnDocument)} error: unable to Unclaim DMER document - {reply.ErrorDetail}");
                return StatusCode(500, reply.ErrorDetail);
            }
        }

        private async Task<ActionResult> AssignDmerClaim(string loginId, string documentId)
        {
            var request = new UpdateClaimRequest
            {
                LoginId = loginId,
                DocumentId = documentId
            };

            var reply = await _documentManagerClient.UpdateClaimDmerAsync(request);
            if (reply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
            {
                var caseDocument = _mapper.Map<DmerDocument>(reply.Item);
                return Ok(caseDocument);
                }
                else
                {
                _logger.LogError($"{nameof(AssignDmerClaim)} error: unable to Claim/Assign DMER document - {reply.ErrorDetail}");
                return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail);
            }
        }
    }
}
