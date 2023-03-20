using pdipadapter.Data;
using pdipadapter.Data.ef;
using Microsoft.EntityFrameworkCore;

namespace pdipadapter.Features.DigitalParticipants.Services;

public class DigitalParticipantService : IDigitalParticipantService
{
    private readonly JumDbContext _context;

    public DigitalParticipantService(JumDbContext context)
    {
        _context = context;
    }

    public async Task<JustinIdentityProvider> CreateIdentityProvider(JustinIdentityProvider identityProvider)
    {
        _context.IdentityProviders.Add(identityProvider);
        await _context.SaveChangesAsync();
        return identityProvider;
    }

    public Task<int> DeleteIdentityProvider(JustinIdentityProvider identityProvider)
    {
        throw new NotImplementedException();
    }

    public Task<JustinIdentityProvider> IdentityProviderById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<JustinIdentityProvider>> IdentityProviderList()
    {
        return await _context.IdentityProviders.ToListAsync();
    }

    public Task<JustinIdentityProvider> UpdateIdentityProvider(JustinIdentityProvider identityProvider)
    {
        throw new NotImplementedException();
    }
}
