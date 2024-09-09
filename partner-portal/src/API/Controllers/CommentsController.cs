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

    }
}
