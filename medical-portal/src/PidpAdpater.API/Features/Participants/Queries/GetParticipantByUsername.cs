using pdipadapter.Features.Participants.Models;
using pdipadapter.Infrastructure.HttpClients.JustinParticipant;
using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace pdipadapter.Features.Participants.Queries;

public record GetParticipantByUsernameQuery(object Username) : IRequest<Participant>;
public class GetParticipantByUsername : IRequestHandler<GetParticipantByUsernameQuery, Participant>
{
    private readonly IJustinParticipantClient _justineParticipantClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetParticipantByUsername(IJustinParticipantClient justineParticipantClient, IHttpContextAccessor httpContextAccessor)
    {
        _justineParticipantClient = justineParticipantClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Participant> Handle(GetParticipantByUsernameQuery request, CancellationToken cancellationToken)
    {
        //var accessToken = await _httpContextAccessor.HttpContext?.GetTokenAsync("access_token");//current part endpoint dont have authrotization
        return await _justineParticipantClient.GetParticipantByUserName(request?.Username.ToString(), "");
    }
}
