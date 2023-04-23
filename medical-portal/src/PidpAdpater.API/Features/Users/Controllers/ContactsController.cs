using pdipadapter.Features.Users.Commands;
using pdipadapter.Features.Users.Models;
using pdipadapter.Features.Users.Queries;
using pdipadapter.Infrastructure.Auth;
using pdipadapter.Kafka.Producer.Interfaces;
using MediatR;
using MedicalPortal.API.Features.Users.Commands;
using MedicalPortal.API.Features.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement;
using PidpAdapter.API.Features.Users.Models;

namespace pdipadapter.Features.Users.Controllers;

[Authorize(Policy = Infrastructure.Auth.Policies.MedicalPractitioner)] //must have an MOA or Practicitoner role in claim
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
    public async Task<ActionResult<string>> CreateContact(
                                                 [FromBody] CreateUser.Command command) 
    => await _mediator.Send(command);
    [HttpGet("{hpdid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PractitionerContactResponse>> GetContact(
                                             [FromRoute] PractitionerContactQuery.Query query)
    => await _mediator.Send(query);
}
