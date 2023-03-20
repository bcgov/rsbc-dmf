using pdipadapter.Data.ef;
using pdipadapter.Features.Persons.Services;
using MediatR;

namespace pdipadapter.Features.Persons.Queries;

public sealed record PersonQuery : IRequest<IEnumerable<JustinPerson>>;
public class PersonQueryHandler : IRequestHandler<PersonQuery, IEnumerable<JustinPerson>>
{
    private IPersonService _personService;
    public PersonQueryHandler(IPersonService personService)
    {
        _personService = personService;
    }

    public async Task<IEnumerable<JustinPerson>> Handle(PersonQuery request, CancellationToken cancellationToken)
    {
        return await _personService.AllPersonAsync();
    }
}
