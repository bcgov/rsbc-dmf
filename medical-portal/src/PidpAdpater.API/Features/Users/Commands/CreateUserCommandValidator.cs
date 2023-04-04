using FluentValidation;

namespace pdipadapter.Features.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Username cannot be empty");
        RuleFor(x => x.UserName.Length)
            .LessThan(50)
            .WithMessage("Name cannot be greater than 50 character");
        RuleFor(x => x.ParticipantId)
            .NotNull()
            .NotEmpty()
            .WithMessage("ParticipantId must be unique");
    }
}

