

namespace pdipadapter.Models;

public class AgencyAssignmentModel
{
    public int Id { get; set; }
    public ICollection<UserModel> Users { get;  } = new List<UserModel>();
    public AgencyModel Agency { get; set; } = new AgencyModel();
}
