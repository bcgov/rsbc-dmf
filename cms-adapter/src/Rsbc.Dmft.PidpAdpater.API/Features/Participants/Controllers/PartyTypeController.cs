using pdipadapter.Data.ef;
using pdipadapter.Data.Security;
using pdipadapter.Features.Participants.Queries;
using pdipadapter.Policies;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace pdipadapter.Features.Participants.Controllers;

[HasPermission(Permissions.AdminUsers)]
[Route("api/[controller]")]
[ApiController]
public class PartyTypeController : ControllerBase
{
    private readonly IMediator _mediator;
    public PartyTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<IEnumerable<JustinPartyType>> GetpartyType()
    {
        return await _mediator.Send(new GetAllPartyTypeQuery());
    }
    
}
