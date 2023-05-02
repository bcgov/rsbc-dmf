using pdipadapter.Infrastructure.Auth;
using MediatR;
using MedicalPortal.API.Features.Users.Commands;
using MedicalPortal.API.Features.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PidpAdapter.API.Features.Users.Models;

namespace pdipadapter.Features.Users.Controllers;

[Authorize(Policy = Infrastructure.Auth.Policies.MedicalPractitioner)] //must have an MOA or Practicitoner role in claim
[Authorize(Policy = Infrastructure.Auth.Policies.DmftEnroledUser)]
[Route("api/[controller]")]
[ApiController]
public class ContactsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly PdipadapterConfiguration _config;
    public ContactsController(IMediator mediator, PdipadapterConfiguration config)
    {
        _mediator = mediator;
        _config = config;
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<string>> CreateContact(
                                                 [FromBody] CreateUser.Command command) 
    => await _mediator.Send(command);
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PractitionerContactQuery.Model>>> GetContacts(
                                             [FromQuery] PractitionerContactQuery.Query query)
    => await _mediator.Send(query);
    [HttpGet("/api/{contactId}/Contacts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PractitionerContactResponse>> GetContact(
                                         [FromRoute] ContactQuery.Query query)
=> await _mediator.Send(query);
}
