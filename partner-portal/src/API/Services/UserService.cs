using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using System.Security.Claims;

namespace Rsbc.Dmf.PartnerPortal.Api.Services

{
    public interface IUserService
    {
        Task<UserContext> GetCurrentUserContext();
        Task<UserContext> GetUserContext(ClaimsPrincipal user);
        Task<ClaimsPrincipal> Login(ClaimsPrincipal user);
        void SetDriverInfo(ViewModels.Driver driver);
        SearchContext GetDriverInfo();
    }

    public record UserContext
    {
        //public string BirthDate { get; set; }
        //public string DriverId { get; set; }
        //public string Email { get; set; }
        //public string FirstName { get; set; }
        public string UserId { get; set; }
        //public string LastName { get; set; }
        //public string ExternalUserName { get; set; }
        public string DisplayName { get; set; }
    }

    public record SearchContext
    {
        public string DriverId { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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
                UserId = user.FindFirstValue(UserClaimTypes.UserId),
                DisplayName = user.FindFirstValue(UserClaimTypes.DisplayName),
            });
        }
        public void SetDriverInfo(ViewModels.Driver driver)
        {
            httpContext.HttpContext.Session.SetString(SearchDriverSession.GivenName, driver.FirstName);
            httpContext.HttpContext.Session.SetString(SearchDriverSession.Surname, driver.LastName);
            httpContext.HttpContext.Session.SetString(SearchDriverSession.DriverId, driver.Id);
            httpContext.HttpContext.Session.SetString(SearchDriverSession.DriverLicenseNumber, driver.LicenseNumber);
        }

        public SearchContext GetDriverInfo()
        {
            var user = new SearchContext();     
            user.FirstName = httpContext.HttpContext.Session.GetString(SearchDriverSession.GivenName); 
            user.LastName = httpContext.HttpContext.Session.GetString(SearchDriverSession.Surname); 
            user.DriverId = httpContext.HttpContext.Session.GetString(SearchDriverSession.DriverId); 
            user.DriverLicenseNumber = httpContext.HttpContext.Session.GetString(SearchDriverSession.DriverLicenseNumber); 
            return user;
        }
    }
}
