using pdipadapter.Features.Participants.Models;
using pdipadapter.Infrastructure.HttpClients.JustinParticipant;

namespace pdipadapter.Features.Participants.Services.ParticipantService;
public class ParticipantService : IParticipantService
{
    public Task<Participant> GetParticipantByUserName(string username)
    {
        throw new NotImplementedException();
    }

    public Task<Participant> GetParticipantPartId(long partId)
    {
        throw new NotImplementedException();
    }
}
