using Microsoft.AspNetCore.Mvc;
using RSBC.DMF.DoctorsPortal.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RSBC.DMF.DoctorsPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : ControllerBase
    {
        private readonly ICaseQueryService caseQueryService;

        public CasesController(ICaseQueryService caseQueryService)
        {
            this.caseQueryService = caseQueryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DmerCaseListItem>>> GetCases([FromQuery] CaseSearchQuery query)
        {
            var cases = await caseQueryService.SearchCases(query);
            return Ok(cases);
        }
    }
}