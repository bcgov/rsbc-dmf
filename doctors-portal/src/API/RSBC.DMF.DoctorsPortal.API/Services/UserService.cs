using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RSBC.DMF.DoctorsPortal.API.Services
{
    public interface IUserService
    {
        Task<UserContext> GetCurrentUserContext();
    }

    public record UserContext
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClinicId { get; set; }
        public string ClinitName { get; set; }
    }

    public class UserService : IUserService
    {
        private readonly HttpContext httpContext;

        public UserService(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext.HttpContext;
        }

        public async Task<UserContext> GetCurrentUserContext()
        {
            //TODO: map from the current principal's claims
            var userCtx = new UserContext
            {
                Id = "test",
                FirstName = "test",
                LastName = "test",
                ClinitName = "test",
                ClinicId = "ffa45383-8ff4-eb11-b82b-00505683fbf4"
            };
            return await Task.FromResult(userCtx);
        }
    }
}