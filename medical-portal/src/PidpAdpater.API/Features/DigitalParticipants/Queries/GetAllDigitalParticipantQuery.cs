using pdipadapter.Data.ef;
using pdipadapter.Features.DigitalParticipants.Services;
using MediatR;

namespace pdipadapter.Features.DigitalParticipants.Queries;

public record GetAllDigitalParticipantQuery : IRequest<IEnumerable<JustinIdentityProvider>>;

public class GetAllDigitalParticipantQueryHandler : IRequestHandler<GetAllDigitalParticipantQuery, IEnumerable<JustinIdentityProvider>>    
{
    private readonly IDigitalParticipantService _digitalParticipant;
    public GetAllDigitalParticipantQueryHandler(IDigitalParticipantService digitalParticipant)
    {
        _digitalParticipant = digitalParticipant;
    }


    public async Task<IEnumerable<JustinIdentityProvider>> Handle(GetAllDigitalParticipantQuery request, CancellationToken cancellationToken)
    {
        return await _digitalParticipant.IdentityProviderList();
    }
}
