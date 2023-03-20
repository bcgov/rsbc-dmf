

namespace pdipadapter.Models;

public class DigitalParticipantModel
{
    public long Id { get; set; }
    public string BusinessIdentifier { get; set; } = string.Empty;
    public ParticipantModel Participant { get; set; } = new ParticipantModel();
}
