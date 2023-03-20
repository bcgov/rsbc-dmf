using FluentValidation;
using pdipadapter.Features.Players.Queries;
using pdipadapter.Models;
using MediatR;

namespace pdipadapter.Features.Players.Commands;
public sealed record UpdatePlayerCommand(int Id, string Name, int Appearance, int Goals, int ShirtNo) : IRequest<Player>;
public class UpdatePlayerCommandHandler: IRequestHandler<UpdatePlayerCommand, Player>
{
    //private readonly IMediator _mediator;
    private readonly IPlayersService _playersService;
    private readonly IValidator<UpdatePlayerCommand> _validator;
    public UpdatePlayerCommandHandler(IPlayersService playersService, IValidator<UpdatePlayerCommand> validator)
    {
        //_mediator = mediator;
        _validator = validator;
        _playersService = playersService;
    }

    public async Task<Player> Handle(UpdatePlayerCommand command, CancellationToken cancellationToken)
    {
        _validator.ValidateAndThrow(command);
        var allPlayers = await _playersService.GetPlayersList();
        var player = allPlayers.First(p => p.Id == command.Id);
        if (player == null) return default;
        player.ShirtNo = command.ShirtNo;
        player.Name = command.Name;
        player.Goals = command.Goals;
        player.Apperance = command.Appearance;
        return await _playersService.UpdatePlayer(player); 

    }
}
