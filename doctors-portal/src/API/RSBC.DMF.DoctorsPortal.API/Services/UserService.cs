using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
        private readonly IConfiguration Configuration;

        public UserService(IHttpContextAccessor httpContext, IConfiguration configuration)
        {
            this.httpContext = httpContext.HttpContext;
            Configuration = configuration;
        }

        public async Task<UserContext> GetCurrentUserContext()
        {
            //TODO: map from the current principal's claims

            string clinicId = Configuration["CLINIC_ID"] != null
                ? Configuration["CLINIC_ID"]
                : "ffa45383-8ff4-eb11-b82b-00505683fbf4";

            var userCtx = new UserContext
            {
                Id = "test",
                FirstName = "test",
                LastName = "test",
                ClinitName = "test",
                ClinicId = clinicId
            };
            return await Task.FromResult(userCtx);
        }
    }
}