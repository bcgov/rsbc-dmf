using pdipadapter.Data.ef;

namespace pdipadapter.Features.DigitalParticipants.Services;

public interface IDigitalParticipantService
{
    Task<IEnumerable<JustinIdentityProvider>> IdentityProviderList();
    Task<JustinIdentityProvider> IdentityProviderById(int id);
    Task<JustinIdentityProvider> CreateIdentityProvider(JustinIdentityProvider identityProvider);
    Task<JustinIdentityProvider> UpdateIdentityProvider(JustinIdentityProvider identityProvider);
    Task<int> DeleteIdentityProvider(JustinIdentityProvider identityProvider);
}
