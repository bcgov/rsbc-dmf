
using FluentValidation;
using pdipadapter.Extensions;
using pdipadapter.Infrastructure.Auth;
using MediatR;
using MedicalPortal.API.Extensions;
using PidpAdapter.API.Features.Users.Models;
using Rsbc.Dmf.CaseManagement.Service;
using Google.Protobuf.WellKnownTypes;
using NodaTime;
using Microsoft.IdentityModel.Tokens;

namespace MedicalPortal.API.Features.Users.Commands;
public class CreateUser
{
    public class Command : IRequest<string>
    {
        public Guid UserId { get; set; }
        public string IdentityProvider { get; set; } = string.Empty;
        public string IdpId { get; set; } = string.Empty;
        public DateTime? Birthdate { get; set; }
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
            //this.RuleFor(x => x.IdentityProvider).NotEmpty().Equal(user.GetIdentityProvider());
            this.RuleFor(x => x.IdpId).NotEmpty().Equal(user.GetIdpId());
            ////this.RuleFor(x => x.Roles).NotNull().ForEach(role =>
            ////{
            ////    role.Must(r => (bool)(user?.GetRoles().ToList().Contains(r.ToString())));
            ////});

            ////this.RuleFor(x => x.Gender).NotEmpty().Equal(user?.GetGender()).WithMessage($"Must match the \"gender\" Claim on the current User");

            this.RuleFor(x => x.FirstName).NotEmpty().MatchesUserClaim(user, Claims.GivenName);
            ////this.RuleFor(x => x.Gender).NotEmpty().MatchesUserClaim(user, Claims.Gender);
            this.RuleFor(x => x.LastName).NotEmpty().MatchesUserClaim(user, Claims.FamilyName);

            this.RuleFor(x => x.Birthdate).NotEmpty().Equal(user?.GetBirthdate()).WithMessage($"Must match the \"birthdate\" Claim on the current User");

            ////this.When(x => x.IdentityProvider == IdentityProviders.BCServicesCard, () => this.RuleFor(x => x.Birthdate).NotEmpty().Equal(user?.GetBirthdate()).WithMessage("Must match the \"birthdate\" Claim on the current User"))
            ////    .Otherwise(() => this.RuleFor(x => x.Birthdate).Empty());
        }
    }
    public class CommandHandler : IRequestHandler<Command, string>
    {
        private readonly UserManager.UserManagerClient userManager;
        private readonly ILogger logger;

        public CommandHandler(UserManager.UserManagerClient userManager, ILogger logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<string> Handle(Command command, CancellationToken cancellationToken)
        {
            var contactExist = await userManager.GetPractitionerContactAsync(new PractitionerRequest { Hpdid = command.IdpId });
            if (!contactExist.ContactId.IsNullOrEmpty())
            {
                this.logger.LogInformation("Practitioner contact already exist");
                return contactExist.ContactId;
            }
            //if (!contactExist.ContactId.IsNullOrEmpty() && contactExist != command)
            //{
            // possibly change in pidp info you should 
            //    //update the contact in dynamics
            //}
            var pReply = await userManager.CreatePractitionerContactAsync(new PractitionerContactRequest
            {
                Email = command.Email,
                Birthdate = Timestamp.FromDateTime(command.Birthdate!.Value.ToUniversalTime()),
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserId = command.UserId.ToString(),
                Gender = "male",
                IdpId = command.IdpId,
                Role = command.Roles.Contains("MOA") ? "MOA" : command.Roles.Contains("PRACTITIONER") ? "PRACTITIONER" : string.Empty
            });
            return pReply.ContactId.ToString();

        }
    }
}

