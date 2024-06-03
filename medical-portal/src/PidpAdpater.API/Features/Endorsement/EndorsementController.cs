using pdipadapter.Infrastructure.Auth;
using MedicalPortal.API.Features.Endorsement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // TODO return no content and not found status codes, fix the false 200 Ok status returns and replace with no content and not found errors
        [HttpGet("contacts/{hpdid}/endorsements")]
        [Authorize(Policy = Policies.MedicalPractitioner)]
        [Authorize(Policy = Policies.DmftEnroledUser)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Model.Endorsement>>> ContactEndorsements(string hpdid)
        {
            // TODO remove the @bcsc part, it doesn't work AND I proved it with hard-coded ids that without the bcsc it does work by using valid and invalid ids
            var endorsements = await endorsement.GetEndorsement(hpdid);
            return new JsonResult(endorsements);
        }
    }
}
