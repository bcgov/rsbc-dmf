using pdipadapter.Features.Participants.Models;

namespace pdipadapter.Features.Participants.Services.ParticipantService
{
    public interface IParticipantService
    {
        Task<Participant> GetParticipantByUserName(string username);
        Task<Participant> GetParticipantPartId(long partId);
    }
}
