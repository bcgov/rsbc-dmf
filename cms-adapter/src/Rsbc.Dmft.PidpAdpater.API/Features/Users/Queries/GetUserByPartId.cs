using pdipadapter.Data.ef;
using pdipadapter.Features.Users.Services;
using MediatR;

namespace pdipadapter.Features.Users.Queries;

public sealed record GetUserByPartId(long partId) : IRequest<JustinUser>;
public class GetUserByPartIdHandler : IRequestHandler<GetUserByPartId, JustinUser>
{
    private readonly IUserService _userService;
    public GetUserByPartIdHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<JustinUser> Handle(GetUserByPartId request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByPartId(request.partId);
    }
}
