using pdipadapter.Data;
using pdipadapter.Data.ef;
using Microsoft.EntityFrameworkCore;

namespace pdipadapter.Features.Persons.Services;

public class PersonService : IPersonService
{
    private readonly JumDbContext _context;
    public PersonService(JumDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<JustinPerson>> AllPerson()
    {
        return await _context.People.ToListAsync();
    }

    public async Task<IEnumerable<JustinPerson>> AllPersonAsync()
    {
        return await _context.People.ToListAsync();
    }

    public async Task<long> CreatePerson(JustinPerson person)
    {
        _context.People.Add(person);
        await _context.SaveChangesAsync();
        return person.PersonId;
    }
}
