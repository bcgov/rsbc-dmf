using FluentValidation;
using pdipadapter.Models;
using MediatR;

namespace pdipadapter.Features.Players.Commands;

public record CreatePlayerCommand(int ShirtNo, string Name, int Appearance, int Goals) : IRequest<Player>;
public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, Player>
{
    private readonly IPlayersService _playersService;
    private readonly IValidator<CreatePlayerCommand> _validator;
    public CreatePlayerCommandHandler(IPlayersService playersService, IValidator<CreatePlayerCommand> validator)
    {
        _playersService = playersService;
        _validator = validator;

    }
    public async Task<Player> Handle(CreatePlayerCommand command, CancellationToken cancellationToken)
    {

        var player = new Player()
        {
            Name = command.Name,
            ShirtNo = command.ShirtNo,
            Apperance = command.Appearance,
            Goals = command.Goals,
        };
        _validator.ValidateAndThrow(command);
        return await _playersService.CreatePlayer(player);
    }
}


