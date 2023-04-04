//using FluentValidation;
using pdipadapter.Models;
using MediatR;

namespace pdipadapter.Features.Players.Queries;
public record GetAllPlayersQuery : IRequest<IEnumerable<Player>>;
public class GetAllPlayersQueryHandler : IRequestHandler<GetAllPlayersQuery, IEnumerable<Player>>
{
    private readonly IPlayersService _playerService;
    //private readonly IMediator _mediator;
    //private readonly IValidator<GetAllPlayersQuery> _validator;
    public GetAllPlayersQueryHandler(IPlayersService playerService)
    {
        //_mediator = mediator;
        _playerService = playerService;
        //_validator = validator;
    }
    public async Task<IEnumerable<Player>> Handle(GetAllPlayersQuery query, CancellationToken cancellationToken)
    {
        //_validator.ValidateAndThrow(query);
        return await _playerService.GetPlayersList();
    }
}


