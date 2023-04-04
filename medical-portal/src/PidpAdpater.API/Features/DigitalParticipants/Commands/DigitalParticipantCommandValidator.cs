using FluentValidation;

namespace pdipadapter.Features.DigitalParticipants.Commands
{
    public class DigitalParticipantCommandValidator : AbstractValidator<CreateDigitalParticipantCommand>
    {
        public DigitalParticipantCommandValidator()
        {
            RuleFor(x=>x.Name)
                .NotEmpty()
                .WithMessage("IDP name cannot be empty");
            RuleFor(x => x.Name.Length)
                .LessThan(500)
                .WithMessage("Name cannot be greater than 500 character");
            RuleFor(x => x.InternalId)
                .NotNull()
                .NotEmpty()
                .Must(BeAValidGuid)
                .WithMessage("InternId must be a valid guid e.g " + Guid.NewGuid().ToString());


        }

        private bool BeAValidGuid(Guid unValidatedGuid)
        {
            try
            {
                if (unValidatedGuid != Guid.Empty && unValidatedGuid != null)
                {
                    if (Guid.TryParse(unValidatedGuid.ToString(), out Guid validatedGuid))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
