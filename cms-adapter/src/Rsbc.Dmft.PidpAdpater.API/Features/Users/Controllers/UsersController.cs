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

namespace pdipadapter.Features.Users.Controllers;

[Authorize(Policy = Infrastructure.Auth.Policies.MedicalPractitioner)] //must have an MOA or Practicitoner role in claim
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IKafkaProducer<string, UserModel> _kafkaProducer;
    private readonly PdipadapterConfiguration _config;
    public UsersController(IMediator mediator, IKafkaProducer<string, UserModel> kafkaProducer, PdipadapterConfiguration config)
    {
        _mediator = mediator;
        _kafkaProducer = kafkaProducer;
        _config = config;
    }
    [HttpPost("/contact/request")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Model>> CreateContact(
                                                 [FromBody] CreateUser.Command command) 
    => await _mediator.Send(command);
    [HttpGet("/contact/{hpdid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Practitioner>> GetUserContact(
                                             [FromBody] PractitionerContact.Query query)
    => await _mediator.Send(query);
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsers()
    {
        var e =  await _mediator.Send(new AllUsersQuery());
        return new JsonResult(e);
    }

    [HttpGet("{username:alpha}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await _mediator.Send(new GetUserQuery(username));
        return new JsonResult(user);
    }
    [HttpGet("{partId:long}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUser(long partId)
    {
        var user = await _mediator.Send(new GetUserByPartId(partId));
        return new JsonResult(user);
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddUser([FromBody] UserModel user)
    {
        var entity = await _mediator.Send(new CreateUserCommand(
            user.UserName, user.ParticipantId, user.IsDisable, user.FirstName, user.LastName, user.MiddleName,user.PreferredName, user.PhoneNumber, user.Email, user.BirthDate,
            user.AgencyId, user.PartyTypeCode, user.Roles         
            ));

        await _kafkaProducer.ProduceAsync(_config.KafkaCluster.TopicName, user.ParticipantId.ToString(), entity);
        return Ok(entity);
    }
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(long ParticipantId, [FromBody] UserModel user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        var update = await _mediator.Send(new UpdateUserCommand(
                user.UserName, user.ParticipantId, user.IsDisable,
                user.FirstName, user.LastName, user.MiddleName, user.PreferredName,
                user.PhoneNumber, user.Email, user.BirthDate, user.AgencyId,
                user.PartyTypeCode, user.Roles
            ));

        return Ok(update);
    }


}
