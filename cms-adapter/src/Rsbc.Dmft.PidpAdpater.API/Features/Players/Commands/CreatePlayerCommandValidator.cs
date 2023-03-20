using FluentValidation;

namespace pdipadapter.Features.Players.Commands;
/// <summary>
/// 
/// </summary>
public class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
{
    public CreatePlayerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty");
        RuleFor(x => x.Goals)
            .GreaterThan(0)
            .WithMessage("Goals must be greater than zero");
    }
}

