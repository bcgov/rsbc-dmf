using pdipadapter.Data.ef;
using pdipadapter.Features.DigitalParticipants.Commands;
using pdipadapter.Features.DigitalParticipants.Queries;
using pdipadapter.Features.Participants.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace pdipadapter.Features.DigitalParticipants.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DigitalParticipantsController : ControllerBase
{
    private readonly IMediator _mediator;
    public DigitalParticipantsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    /// <summary>
    /// Retrieve all Digital Identity Providers 
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetIdentityProviders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<JustinIdentityProvider>> GetAllIdentityProviders()
    {
        return await _mediator.Send(new GetAllDigitalParticipantQuery());
    }
    /// <summary>
    /// Create new digitial identity provider
    /// </summary>
    /// <param name="digitalParticipant"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateIdentityProvider([FromBody] DigitalParticipant digitalParticipant)
    {
        var c = await _mediator.Send(new CreateDigitalParticipantCommand(
                                        digitalParticipant.InternalId, 
                                        digitalParticipant.Alias, 
                                        digitalParticipant.Name, 
                                        digitalParticipant.Description, 
                                        digitalParticipant.ProviderId,
                                        digitalParticipant.TokenUrl,
                                        digitalParticipant.AuthUrl
                                        ));
        return Ok(c);
    }
}
