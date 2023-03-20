using pdipadapter.Data.ef;
using pdipadapter.Features.Users.Services;
using MediatR;

namespace pdipadapter.Features.Users.Queries;


public sealed record GetUserQuery(string username) : IRequest<JustinUser>;
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, JustinUser>
{
    private readonly IUserService _userService;
    public GetUserQueryHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<JustinUser> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByUserName(request.username);
    }
}
