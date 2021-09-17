using Rsbc.Dmf.CaseManagement.Dynamics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface IUserManager
    {
        Task<GetUsersResponse> GetUsers(GetUsersRequest request);

        Task<LoginUserResponse> LoginUser(LoginUserRequest request);
    }

    public class GetUsersRequest
    {
        public string ByBcscGuid { get; set; }
    }

    public class GetUsersResponse
    {
        public IEnumerable<User> Items { get; set; }
    }

    public class LoginUserRequest
    {
        public string ExternalSystem { get; set; }
        public string ExternalSystemUserId { get; set; }
    }

    public class LoginUserResponse
    {
        public string Userid { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string ExternalSystem { get; set; }
        public string ExternalSystemUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    internal class UserManager : IUserManager
    {
        private readonly DynamicsContext dynamicsContext;

        public UserManager(DynamicsContext dynamicsContext)
        {
            this.dynamicsContext = dynamicsContext;
        }

        public Task<GetUsersResponse> GetUsers(GetUsersRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<LoginUserResponse> LoginUser(LoginUserRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}