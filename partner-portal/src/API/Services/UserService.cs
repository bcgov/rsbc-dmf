using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using System.Security.Claims;
using System.Web;

namespace Rsbc.Dmf.PartnerPortal.Api.Services;

public interface IUserService
{
    Task<UserContext> GetCurrentUserContext();
    void SetDriverInfo(ViewModels.Driver driver);
    SearchContext GetDriverInfo();
    Task<ClaimsPrincipal> Login(HttpRequest request,ClaimsPrincipal user);
}

public record UserContext
{
    public string UserId { get; set; }
    public string DisplayName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdentityProvider { get; set; }
    public string Email { get; set; }
    public string ExternalUserName { get; set; }
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

        //common claim types if the expected claim isn't present
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");

        return await Task.FromResult(new UserContext
        {
            UserId = userId,
            DisplayName = user.FindFirstValue(UserClaimTypes.DisplayName),
            FirstName = user.FindFirstValue(UserClaimTypes.GivenName),
            LastName = user.FindFirstValue(UserClaimTypes.FamilyName),
            IdentityProvider = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/identityprovider") ?? user.FindFirstValue("idp"),
            Email = user.FindFirstValue(ClaimTypes.Email),
            ExternalUserName = user.FindFirstValue(ClaimTypes.Upn),
        });
    }

    public async Task<ClaimsPrincipal> Login(HttpRequest request, ClaimsPrincipal user)
    {
        logger.LogDebug("Processing login {0}", user.Identity.Name);
        logger.LogDebug(" claims:\n{0}", string.Join(",\n", user.Claims.Select(c => $"{c.Type}: {c.Value}")));

var loginRequest = new PartnerPortalLoginRequest
        {

            ExternalSystem = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/identityprovider") ?? user.FindFirstValue("idp"),
            ExternalSystemUserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub"),
            FirstName = user.FindFirstValue(ClaimTypes.GivenName) ?? user.FindFirstValue("first_name") ?? string.Empty,
            LastName = user.FindFirstValue(ClaimTypes.Surname) ?? user.FindFirstValue("last_name") ?? string.Empty,
        };

        var pathValue = request?.Path.Value ?? string.Empty;
        var isRegisterPath = pathValue.IndexOf("register", StringComparison.OrdinalIgnoreCase) >= 0;

        // Registration endpoint - skip adding claims
        if (isRegisterPath) return user;

        var loginResponse = await userManager.PartnerPortalLoginAsync(loginRequest);
        if (loginResponse.ResultStatus == ResultStatus.Fail) throw new Exception(loginResponse.ErrorDetail);

        var searchResults = await userManager.PartnerPortalSearchAsync(new PartnerPortalUserSearchRequest { UserId = loginResponse.UserId});
        if (searchResults.ResultStatus == ResultStatus.Fail) throw new Exception(searchResults.ErrorDetail);

      

        var userProfile = searchResults.User.SingleOrDefault();

        if (userProfile == null)
        throw new BadHttpRequestException($"User not found {loginResponse.UserId}");

        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Sid, loginResponse.UserId));
        //claims.Add(new Claim(ClaimTypes.Email, loginResponse.UserEmail));
        claims.Add(new Claim(ClaimTypes.Upn, $"{userProfile.ExternalSystemUserId}@{userProfile.ExternalSystem}"));
        claims.Add(new Claim(ClaimTypes.Surname, userProfile.Surname));
        claims.Add(new Claim(ClaimTypes.GivenName, userProfile.GivenName));
        claims.Add(new Claim(UserClaimTypes.DisplayName, user.Claims.Single(u => u.Type == UserClaimTypes.DisplayName).Value));

        user.AddIdentity(new ClaimsIdentity(claims));

        //logger.LogInformation("User {0} ({1}@{2}) logged in", userProfile.ContactId, userProfile.ExternalSystemUserId, userProfile.ExternalSystem);

        return user;

    }


    public void SetDriverInfo(ViewModels.Driver driver)
    {
        httpContext.HttpContext.Session.SetString(SearchDriverSession.GivenName, driver.FirstName);
        httpContext.HttpContext.Session.SetString(SearchDriverSession.Surname, driver.LastName);
        httpContext.HttpContext.Session.SetString(SearchDriverSession.DriverId, driver.Id);
        httpContext.HttpContext.Session.SetString(SearchDriverSession.DriverLicenseNumber, driver.DriverLicenseNumber);
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
