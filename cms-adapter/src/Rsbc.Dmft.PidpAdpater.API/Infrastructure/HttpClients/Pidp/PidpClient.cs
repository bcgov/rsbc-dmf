using pdipadapter.Features.Participants.Models;
using pdipadapter.Infrastructure.HttpClients;
using MedicalPortal.API.Features.Endorsement.Model;

namespace MedicalPortal.API.Infrastructure.HttpClients.Pidp;
public class PidpClient : BaseClient, IPidpClient
{
    public PidpClient(HttpClient client, ILogger logger) : base(client, logger)
    {
    }

    public Task<Endorsement> GetEndorsements(string Hpdid, string accesToken)
    {
        throw new NotImplementedException();
    }

    public Task<Participant> GetParticipantPartId(decimal partId, string accesToken)
    {
        throw new NotImplementedException();
    }
}

