using pdipadapter.Data.ef;
using pdipadapter.Features.Persons.Commands;
using pdipadapter.Features.Persons.Models;
using pdipadapter.Features.Persons.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace pdipadapter.Features.Persons.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PersonsController(IMediator mediator)
        {
            _mediator = mediator;   
        }
        [HttpGet]
        public async Task<IEnumerable<JustinPerson>> AllPerson()
        {
            return await _mediator.Send(new PersonQuery());
        }
        [HttpPost]
        public async Task<ActionResult<long>> CreatePerson([FromBody] Person person)
        {
            //var personCommand = new CreatePersonCommand
            var p = await _mediator.Send(request: new CreatePersonCommand(
                person.Surname,
                person.FirstName,
                person.MiddleNames,
                person.PreferredName,
                person.BirthDate
          ));
            return Ok(p);
        }
    }
}
