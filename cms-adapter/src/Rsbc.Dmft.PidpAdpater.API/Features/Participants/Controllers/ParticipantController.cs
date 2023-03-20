using pdipadapter.Features.Participants.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using pdipadapter.Features.Participants.Queries;

namespace pdipadapter.Features.Participants.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ParticipantController : ControllerBase
{
    private readonly IMediator _mediator;
    public ParticipantController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ParticipantByUsername([Required]string username)
    {
        var participant = await _mediator.Send(new GetParticipantByUsernameQuery(username));
        return new JsonResult(participant);
    }
    [HttpGet("{partId:decimal}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ParticipantById(decimal partId)
    {
        var participant = await _mediator.Send(new GetParticipantByIdQuery(partId));
        return new JsonResult(participant);
    }
}

