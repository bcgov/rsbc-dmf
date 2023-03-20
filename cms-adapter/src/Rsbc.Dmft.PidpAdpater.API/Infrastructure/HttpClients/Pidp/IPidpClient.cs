using pdipadapter.Features.Participants.Models;
using MedicalPortal.API.Features.Endorsement.Model;

namespace MedicalPortal.API.Infrastructure.HttpClients.Pidp;
public interface IPidpClient
{
    Task<Endorsement> GetEndorsements(string Hpdid, string accesToken);
    Task<Participant> GetParticipantPartId(decimal partId, string accesToken);
}

