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
        Task<ClaimsPrincipal> Login(ClaimsPrincipal user);
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


        public async Task<ClaimsPrincipal> Login(ClaimsPrincipal user)
        {
            logger.LogDebug("Processing login {0}", user.Identity.Name);
            logger.LogDebug(" claims:\n{0}", string.Join(",\n", user.Claims.Select(c => $"{c.Type}: {c.Value}")));

            var loginRequest = new UserLoginRequest
            {
                UserType = UserType.DriverUserType,
                ExternalSystem = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/identityprovider") ?? user.FindFirstValue("idp"),
                ExternalSystemUserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub"),
                FirstName = user.FindFirstValue(UserClaimTypes.GivenName) ?? user.FindFirstValue("first_name") ?? string.Empty,
                LastName = user.FindFirstValue(UserClaimTypes.FamilyName) ?? user.FindFirstValue("last_name") ?? string.Empty,
                UserProfiles = { new UserProfile() }
            };
            var loginResponse = await userManager.LoginAsync(loginRequest);
            if (loginResponse.ResultStatus == ResultStatus.Fail) throw new Exception(loginResponse.ErrorDetail);

            var searchResults = await userManager.SearchAsync(new UsersSearchRequest { UserId = loginResponse.UserId, UserType = UserType.DriverUserType });
            if (searchResults.ResultStatus == ResultStatus.Fail) throw new Exception(searchResults.ErrorDetail);

            var userProfile = searchResults.User.SingleOrDefault();
            if (userProfile == null) throw new Exception($"User {loginResponse.UserId} not found");

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Sid, loginResponse.UserId));
            claims.Add(new Claim(ClaimTypes.Email, loginResponse.UserEmail));
            claims.Add(new Claim(ClaimTypes.Upn, $"{userProfile.ExternalSystemUserId}@{userProfile.ExternalSystem}"));
            claims.Add(new Claim(ClaimTypes.GivenName, userProfile.FirstName));
            claims.Add(new Claim(ClaimTypes.Surname, userProfile.LastName));
            claims.Add(new Claim(UserClaimTypes.DisplayName, user.Claims.Single(u => u.Type == UserClaimTypes.DisplayName).Value));

            if (!string.IsNullOrEmpty(loginResponse.DriverId))
            {
                claims.Add(new Claim(UserClaimTypes.DriverId, loginResponse.DriverId));
                claims.Add(new Claim(UserClaimTypes.DriverLicenseNumber, loginResponse.DriverLicenseNumber));
            }
            user.AddIdentity(new ClaimsIdentity(claims));

            logger.LogInformation("User {0} ({1}@{2}) logged in", userProfile.Id, userProfile.ExternalSystemUserId, userProfile.ExternalSystem);

            return user;
        }
    }
}
