
using FluentValidation;
using pdipadapter.Extensions;
using pdipadapter.Infrastructure.Auth;
using MediatR;
using MedicalPortal.API.Extensions;
using Rsbc.Dmf.CaseManagement;
using Microsoft.OData.Edm;

namespace MedicalPortal.API.Features.Users.Commands;
public class CreateUser
{
    public class Command : IRequest<Model>
    {
        public Guid UserId { get; set; }
        public string IdentityProvider { get; set; } = string.Empty;
        public string IdpId { get; set; } = string.Empty;
        public Date? Birthdate { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string[] Roles { get; set; } = new string[] { };
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator(IHttpContextAccessor accessor)
        {
            var user = accessor?.HttpContext?.User;

            this.RuleFor(x => x.UserId).NotEmpty().Equal(user.GetUserId());
            this.RuleFor(x => x.IdentityProvider).NotEmpty().Equal(user.GetIdentityProvider());
            this.RuleFor(x => x.IdpId).NotEmpty().Equal(user.GetIdpId());
            this.RuleFor(x => x.Roles).NotNull().ForEach(role =>
            {
                role.Must(r => (bool)(user?.GetRoles().ToList().Contains(r.ToString())));
            });

            this.RuleFor(x => x.FirstName).NotEmpty().MatchesUserClaim(user, Claims.GivenName);
            this.RuleFor(x => x.Gender).NotEmpty().MatchesUserClaim(user, Claims.Gender);
            this.RuleFor(x => x.LastName).NotEmpty().MatchesUserClaim(user, Claims.FamilyName);

            this.When(x => x.IdentityProvider == IdentityProviders.BCServicesCard, () => this.RuleFor(x => x.Birthdate).NotEmpty().Equal(user?.GetBirthdate()).WithMessage("Must match the \"birthdate\" Claim on the current User"))
                .Otherwise(() => this.RuleFor(x => x.Birthdate).Empty());
        }
    }
    public class CommandHandler : IRequestHandler<Command, Model>
    {
        private readonly IUserManager userManager;

        public CommandHandler(IUserManager userManager) => this.userManager = userManager;

        public async Task<Model> Handle(Command command, CancellationToken cancellationToken)
        {
            return await this.userManager.CreatePractitionerContact(new Practitioner
            {
                Email = command.Email,
                Birthdate = command.Birthdate,
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserId = command.UserId,
                Gender = command.Gender,
                IdpId = command.IdpId,
                Roles = command.Roles,
            });


        }
    }
}

