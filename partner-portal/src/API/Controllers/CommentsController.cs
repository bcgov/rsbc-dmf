using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.PartnerPortal.Api.Services;
using SharedUtils;
using System.Net;
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;
using System.Threading.Channels;
using Rsbc.Dmf.PartnerPortal.Api.ViewModels;
using Rsbc.Dmf.PartnerPortal.Api.Model;
using System.ComponentModel.Design;

namespace Rsbc.Dmf.PartnerPortal.Api.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(Policy = Policy.Driver)]
    [ApiController]
    public class CommentsController : Controller
    {
        private readonly CommentManager.CommentManagerClient _commentManagerClient;
        private readonly CaseManagerClient _caseManagerClient;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CommentsController(CommentManager.CommentManagerClient commentManagerClient, CaseManager.CaseManagerClient caseManagerClient, IUserService userService, IMapper mapper)
        {
            _commentManagerClient = commentManagerClient;
            _caseManagerClient = caseManagerClient;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("getComments")]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.Comment>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetCaseComments))]
        public async Task<ActionResult> GetCaseComments()
        {
            var result = new List<ViewModels.Comment>();

            var profile = _userService.GetDriverInfo();

            var commentsRequest = new DriverIdRequest {Id = profile.DriverId};
            var getComments = _commentManagerClient.GetCommentOnDriver(commentsRequest);

            if (getComments?.ResultStatus == ResultStatus.Success)
            {
                result = _mapper.Map<List<ViewModels.Comment>>(getComments.Items);
            }
            else
            {
                return StatusCode(500, getComments?.ErrorDetail ?? $"{nameof(getComments)} failed.");
            }

            return Json(result);
        }

    
        [HttpPost("create")]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ActionName(nameof(CreateComment))]
        public async Task<IActionResult> CreateComment([FromBody] CommentRequest commentRequest)
        {
            var profile = await _userService.GetCurrentUserContext();
            var user = _userService.GetDriverInfo();

            var mostRecentCaseReply = _caseManagerClient.GetMostRecentCaseDetail(new DriverIdRequest { Id = user.DriverId });

            var comment = new LegacyComment();
            comment.CommentText = commentRequest.CommentText ?? string.Empty;
            comment.CommentDate = DateTime.UtcNow.ToTimestamp();
            comment.CommentTypeCode = "W";
            comment.Origin = "User";
            comment.UserId = profile.DisplayName;
            comment.SignatureName = profile.DisplayName;

            comment.Driver = new CaseManagement.Service.Driver();
            comment.Driver.DriverLicenseNumber = user.DriverLicenseNumber;
            comment.Driver.Surname = user.LastName ?? string.Empty;

            // Check if we have a valid case to attach the comment to
            if (mostRecentCaseReply.ResultStatus == ResultStatus.Success && !string.IsNullOrEmpty(mostRecentCaseReply.Item?.CaseId))
            {
                // Create comment on case
                comment.CaseId = mostRecentCaseReply.Item.CaseId;
                var reply = _commentManagerClient.AddCaseComment(comment);

                if (reply.ResultStatus != ResultStatus.Success)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail ?? $"{nameof(CreateComment)} failed to create case comment.");
                }
            }
            else
            {
                // No case found, create comment directly on driver
                comment.CaseId = string.Empty;
                var reply = _commentManagerClient.AddCaseComment(comment);

                if (reply.ResultStatus != ResultStatus.Success)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, reply.ErrorDetail ?? $"{nameof(CreateComment)} failed to create driver comment.");
                }
            }

            return Ok();
        }

    }
}
