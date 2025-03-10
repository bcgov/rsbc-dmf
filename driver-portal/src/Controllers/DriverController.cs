using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.Api.Services;
using Rsbc.Dmf.DriverPortal.ViewModels;
using Rsbc.Dmf.IcbcAdapter;
using Rsbc.Dmf.IcbcAdapter.Client;
using System.Net;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;
using static Rsbc.Dmf.CaseManagement.Service.DocumentManager;

namespace Rsbc.Dmf.DriverPortal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policy.Driver)]
    public class DriverController : Controller
    {
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentManager.DocumentManagerClient _documentManagerClient;
        private readonly ICachedIcbcAdapterClient _icbcAdapterClient;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<DriverController> _logger;
        private readonly DocumentStorageAdapterClient _documentStorageAdapterClient;

        public DriverController(CaseManager.CaseManagerClient cmsAdapterClient, DocumentManager.DocumentManagerClient documentManagerClient, ICachedIcbcAdapterClient icbcAdapterClient, DocumentStorageAdapterClient documentStorageAdapterClient, IUserService userService, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _cmsAdapterClient = cmsAdapterClient;
            _icbcAdapterClient = icbcAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _userService = userService;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<DriverController>();
            _documentManagerClient = documentManagerClient;
        }


        /// <summary>
        /// Get all documents for a given driver but filter out documents without a url
        /// </summary>
        /// <returns>IEnumerable&lt;Document&gt;</returns>
        [HttpGet("AllDocuments")]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.Document>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName("GetAllDocuments")]
        public async Task<ActionResult> GetAllDocuments()
        {
            var profile = await _userService.GetCurrentUserContext();

            var driverIdRequest = new DriverIdRequest() { Id = profile.DriverId };
            var reply = _documentManagerClient.GetDriverDocumentsById(driverIdRequest);
            if (reply.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                var replyItemsWithDocuments = reply.Items;
                var result = _mapper.Map<List<ViewModels.Document>>(replyItemsWithDocuments);

                // sort the documents
                if (result.Count > 0)
                {
                    result.ForEach(doc =>
                        {
                            if (doc.DocumentUrl.EndsWith(".tif") || doc.DocumentUrl.EndsWith(".tiff"))
                            {
                                // Convert FileContents here
                                try
                                {
                                    doc.ErrorMessage = DocumentUtils.checkTiff2Pdf(doc.DocumentId, _cmsAdapterClient, _documentStorageAdapterClient);
                                }
                                catch
                                {
                                    doc.ErrorMessage = "This file is corrupted and cannot be opened.";
                                }
                            }
                        });
                    
                    result = result.OrderByDescending(cs => cs.CreateDate).ToList();
                }

                return Json(result);

            }
            else
            {
                _logger.LogError($"{nameof(GetAllDocuments)} failed for driverId: {profile.DriverId}", reply.ErrorDetail);
                return StatusCode(500, reply.ErrorDetail);
            }
        }


        [HttpGet("info")]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.Document>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName(nameof(GetHistory))]
        public async Task<ActionResult<DriverInfoReply>> GetHistory()
        {
            var profile = await _userService.GetCurrentUserContext();
            if (profile?.DriverLicenseNumber == null)
            {
                return NotFound();
            }

            var request = new DriverInfoRequest();
            request.DriverLicence = profile.DriverLicenseNumber;
            var reply = await _icbcAdapterClient.GetDriverInfoAsync(request);

            return Json(reply);
        }
    }
}