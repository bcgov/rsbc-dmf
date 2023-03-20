namespace pdipadapter.Features.Participants.Models;

public record DigitalParticipant (Guid InternalId, string Alias, string Name, string Description, string ProviderId, string TokenUrl, string AuthUrl);

