using pdipadapter.Infrastructure.Auth;
using MediatR;
using MedicalPortal.API.Features.Users.Commands;
using MedicalPortal.API.Features.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PidpAdapter.API.Features.Users.Models;
using PidpAdapter.API.Infrastructure;
using pdipadapter.Infrastructure.Services;

namespace pdipadapter.Features.Users.Controllers;

[Authorize(Policy = Infrastructure.Auth.Policies.MedicalPractitioner)] //must have an MOA or Practicitoner role in claim
[Authorize(Policy = Infrastructure.Auth.Policies.DmftEnroledUser)]
[Route("api/[controller]")]
[ApiController]
public class ContactsController : PidpAdapterControllerBase
{
    private readonly IMediator _mediator;
    private readonly PdipadapterConfiguration _config;
    private readonly ILogger<PidpAdapterControllerBase> logger;
    public ContactsController(IMediator mediator, PdipadapterConfiguration config, IPidpAdapterAuthorizationService authorizationService, ILogger<PidpAdapterControllerBase> logger) : base(authorizationService, mediator, logger)
    {
        _mediator = mediator;
        _config = config;
        this.logger = logger;
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
    {
        return await this.AuthorizeContactBeforeHandleAsync<PractitionerContactQuery.Query, List<PractitionerContactQuery.Model>>(
        query.contactId,
        query
    );
    }
    [HttpGet("/api/{contactId}/contacts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PractitionerContactResponse>> GetContact(
                                         [FromRoute] ContactQuery.Query query)
=> await _mediator.Send(query);
}
