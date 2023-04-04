using pdipadapter.Features.Participants.Models;

namespace pdipadapter.Infrastructure.HttpClients.JustinParticipant
{
    public interface IJustinParticipantClient
    {
        Task<Participant> GetParticipantByUserName(string username, string accesToken);
        Task<Participant> GetParticipantPartId(decimal partId, string accesToken);
    }
}
