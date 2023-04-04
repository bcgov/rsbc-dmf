using FluentValidation;

namespace pdipadapter.Features.Players.Commands;
    public class UpdatePlayerCommandValidation : AbstractValidator<UpdatePlayerCommand>
{
    public UpdatePlayerCommandValidation()
    {
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Id).NotEmpty();
    }
}

