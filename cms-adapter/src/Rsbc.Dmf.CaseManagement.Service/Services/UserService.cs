using Grpc.Core;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement.Service.Services
{
    public class UserService : UserManager.UserManagerBase
    {
        private readonly IUserManager userManager;

        public UserService(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        public async override Task<UsersSearchReply> Search(UsersSearchRequest request, ServerCallContext context)
        {
            return await base.Search(request, context);
        }

        public async override Task<UserLoginReply> Login(UserLoginRequest request, ServerCallContext context)
        {
            return await base.Login(request, context);
        }
    }
}