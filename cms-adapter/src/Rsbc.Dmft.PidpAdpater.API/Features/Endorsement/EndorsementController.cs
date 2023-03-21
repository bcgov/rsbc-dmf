using AutoMapper;
using pdipadapter.Data.Security;
using pdipadapter.Features.Participants.Models;
using pdipadapter.Features.Participants.Queries;
using pdipadapter.Infrastructure.Auth;
using pdipadapter.Policies;
using MedicalPortal.API.Features.Endorsement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement;

namespace MedicalPortal.API.Features.Endorsement
{
    [Route("api/")]
    [ApiController]
    public class EndorsementController : ControllerBase
    {
        #region Variables
        private readonly IEndorsement endorsement;
        public EndorsementController(IEndorsement endorsement)
        {
            this.endorsement = endorsement;
        }

        #endregion
        [HttpGet("contacts/{hpdid}/endorsements")]
        [Authorize(Policy = Policies.MedicalPractitioner)]
        [Authorize(Policy = Policies.DmftEnroledUser)] //must have MOA or PRACTICTIONER claim and Must be enroled to DMFT web app
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Model.Endorsement>>> ContactEndorsements(string hpdid)
        {
            var endorsements = await endorsement.GetEndorsement(hpdid);
            return new JsonResult(endorsements);
        }
    }
   
}
