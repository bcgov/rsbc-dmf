using FluentValidation;
using pdipadapter.Data.ef;
using pdipadapter.Features.DigitalParticipants.Services;
using MediatR;

namespace pdipadapter.Features.DigitalParticipants.Commands;

public record CreateDigitalParticipantCommand(Guid InternalId, string Alias, string Name, string Description, string ProviderId, string TokenUrl, string AuthUrl) : IRequest<JustinIdentityProvider>;

public class CreateDigitalParticipantCommandHandler : IRequestHandler<CreateDigitalParticipantCommand, JustinIdentityProvider>
{
    private readonly IDigitalParticipantService _digitalParticipantService;
    private readonly IValidator<CreateDigitalParticipantCommand> _validator;
    public CreateDigitalParticipantCommandHandler(IDigitalParticipantService digitalParticipantService, IValidator<CreateDigitalParticipantCommand> validator)
    {
        _digitalParticipantService = digitalParticipantService;
        _validator = validator;
    }

    public async Task<JustinIdentityProvider> Handle(CreateDigitalParticipantCommand request, CancellationToken cancellationToken)
    {
        var idp = new JustinIdentityProvider
        {
            InternalId = request.InternalId,
            Name = request.Name,
            Description = request.Description,
            ProviderId = request.ProviderId,
            Alias = request.Alias,
            IsActive = true,
            TokenUrl = request.TokenUrl,
            AuthUrl = request.AuthUrl
        };
        _validator.ValidateAndThrow(request);
       return await _digitalParticipantService.CreateIdentityProvider(idp);
        //return Task.FromResult(idp);
    }
}
