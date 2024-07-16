using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.PartnerPortal.Api.Model;
using System.ComponentModel;
using System.Security.Claims;

namespace Rsbc.Dmf.PartnerPortal.Api.Services

{
    public interface IUserService
    {
        Task<UserContext> GetCurrentUserContext();
        Task<UserContext> GetUserContext(ClaimsPrincipal user);
    }

    public record UserContext
    {
        public string BirthDate { get; set; }
        public string DriverId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }

        [Description("UserId")]
        public string Id { get; set; }

        public string LastName { get; set; }

        public string ExternalUserName { get; set; }

        public string DisplayName { get; set; }
        public string DriverLicenseNumber { get; set; }
    }

    public class UserService : IUserService
    {
        private readonly UserManager.UserManagerClient userManager;
        private readonly IHttpContextAccessor httpContext;
        private readonly IConfiguration configuration;
        private readonly ILogger<UserService> logger;

        public UserService(UserManager.UserManagerClient userManager, IHttpContextAccessor httpContext, IConfiguration configuration, ILogger<UserService> logger)
        {
            this.userManager = userManager;
            this.httpContext = httpContext;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<UserContext> GetCurrentUserContext() => await GetUserContext(httpContext.HttpContext.User);

        public async Task<UserContext> GetUserContext(ClaimsPrincipal user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await Task.FromResult(new UserContext
            {
                Id = user.FindFirstValue(ClaimTypes.Sid),
                BirthDate = user.FindFirstValue(UserClaimTypes.BirthDate),
                DriverId = user.FindFirstValue(UserClaimTypes.DriverId),
                FirstName = user.FindFirstValue(UserClaimTypes.GivenName),
                LastName = user.FindFirstValue(UserClaimTypes.FamilyName),
                Email = user.FindFirstValue(ClaimTypes.Email),
                ExternalUserName = user.FindFirstValue(ClaimTypes.Upn),
                DisplayName = user.FindFirstValue(UserClaimTypes.DisplayName),
                DriverLicenseNumber = user.FindFirstValue(UserClaimTypes.DriverLicenseNumber)
            });
        }
    }
}
