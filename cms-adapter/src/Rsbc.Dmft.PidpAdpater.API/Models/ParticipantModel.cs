

namespace pdipadapter.Models;

public class ParticipantModel
{
    public long ParticipantId { get; set; }
    public long UserId { get; set; }
    public UserModel? User { get; set; }
}
