using pdipadapter.Data.ef;

namespace pdipadapter.Features.Users.Services;

public interface IUserService
{
    Task<IEnumerable<JustinUser>> AllUsersList();
    Task<JustinUser> GetUserById(long id);
    Task<JustinUser>GetUserByUserName(string username);
    Task<JustinUser> GetUserByPartId(long partId);
    Task<JustinUser> AddUser(JustinUser user);
    Task<JustinUser> UpdateUser(JustinUser user);
    Task<long> DeleteUser(JustinUser user);
}
