using AutoMapper;
using EnumsNET;
using PidpAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Auth.Extension;
using RSBC.DMF.MedicalPortal.API.Utilities;
using RSBC.DMF.MedicalPortal.API.ViewModels;
using System.Security.Claims;
using static RSBC.DMF.MedicalPortal.API.Auth.AuthConstant;

namespace RSBC.DMF.MedicalPortal.API.Services
{
    public interface IUserService
    {
        Task<UserContext> GetCurrentUserContext();
        Task<UserContext> GetUserContext(ClaimsPrincipal user);
        ClaimsPrincipal GetUser();
        Task<ClaimsPrincipal> Login(ClaimsPrincipal user);
        Task SetEmail(string userId, string email);
    }

    public record UserContext
    {
        // Pidp User Id
        public string Id { get; set; }

        // TODO should only be accessible by Practitioner, MOA/MOM should be using LoginIds
        // TODO set this in Login instead
        public string LoginId
        {
            get
            {
                return LoginIds.First();
            }
        }

        public List<string> LoginIds { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }        
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<Endorsement> Endorsements { get; set; }
    }

    public class UserService : IUserService
    {
        private readonly UserManager.UserManagerClient _userManager;
        private readonly PidpManager.PidpManagerClient _pidpAdapterClient;
        private readonly IHttpContextAccessor httpContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration configuration;
        private readonly ILogger<UserService> _logger;
        
        public UserService(UserManager.UserManagerClient userManager, PidpManager.PidpManagerClient pidpAdapterClient, IHttpContextAccessor httpContext, IMapper mapper, IConfiguration configuration, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _pidpAdapterClient = pidpAdapterClient;
            this.httpContext = httpContext;
            _mapper = mapper;
            this.configuration = configuration;
            _logger = logger;
        }

        public async Task<UserContext> GetCurrentUserContext() => await GetUserContext(httpContext.HttpContext.User);

        public async Task<UserContext> GetUserContext(ClaimsPrincipal user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var userContext = new UserContext();
            userContext.Id = user.FindFirstValue(Claims.PreferredUsername);
            userContext.LoginIds = user.GetClaim<List<string>>(Claims.LoginIds);
            userContext.FirstName = user.FindFirstValue(ClaimTypes.GivenName);
            userContext.LastName = user.FindFirstValue(ClaimTypes.Surname);
            userContext.Email = user.FindFirstValue(Claims.Email);
            userContext.Roles = user.GetRoles();
            userContext.Endorsements = user.GetClaim<IEnumerable<Endorsement>>(Claims.Endorsements);

            return await Task.FromResult(userContext);
        }

        public ClaimsPrincipal GetUser() => httpContext.HttpContext.User;

        public async Task<ClaimsPrincipal> Login(ClaimsPrincipal user)
        {
            try
            {
                _logger.LogDebug("Processing login {0}", user.Identity.Name);
                _logger.LogDebug(" claims:\n{0}", string.Join(",\n", user.Claims.Select(c => $"{c.Type}: {c.Value}")));

                var loginRequest = new UserLoginRequest();
                loginRequest.UserType = UserType.MedicalPractitionerUserType;
                loginRequest.Email = user.HasClaim(c => c.Type == Claims.Email) ? user.FindFirstValue(Claims.Email) : string.Empty;
                loginRequest.ExternalSystem = user.GetIdentityProvider();
                loginRequest.ExternalSystemUserId = user.FindFirstValue(Claims.PreferredUsername);
                loginRequest.FirstName = user.FindFirstValue(ClaimTypes.GivenName);
                loginRequest.LastName = user.FindFirstValue(ClaimTypes.Surname);

                var loginResponse = await _userManager.LoginAsync(loginRequest);
                if (loginResponse.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Fail) throw new Exception(loginResponse.ErrorDetail);

                var searchResults = await _userManager.SearchAsync(new UsersSearchRequest { UserId = loginResponse.UserId });
                if (searchResults.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Fail) throw new Exception(searchResults.ErrorDetail);

                var userProfile = searchResults.User.SingleOrDefault();
                if (userProfile == null) throw new Exception($"User {loginResponse.UserId} not found");

                var claims = new List<Claim>();
                claims.AddClaim(Claims.LoginIds, loginResponse.LoginIds.ToList());
                var endorsements = await GetEndorsements(loginRequest.ExternalSystemUserId);
                claims.AddClaim(Claims.Endorsements, endorsements);
                user.AddIdentity(new ClaimsIdentity(claims));

                _logger.LogInformation("User {0} ({1}@{2}) logged in", userProfile.Id, userProfile.ExternalSystemUserId, userProfile.ExternalSystem);

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user {0}", user.Identity.Name);
                throw;
            }
        }

        public async Task<IEnumerable<Endorsement>> GetEndorsements(string userId)
        {
            var getEndorsementsRequest = new GetEndorsementsRequest { UserId = userId };
            var reply = await _pidpAdapterClient.GetEndorsementsAsync(getEndorsementsRequest);
            if (reply.ResultStatus == PidpAdapter.ResultStatus.Fail)
            {
                _logger.LogError($"{nameof(GetEndorsements)} error: unable to get endorsements - {reply.ErrorDetail}");
                return null;
            }
            var endorsements = _mapper.Map<IEnumerable<Endorsement>>(reply.Endorsements);

            // TODO optimize by getting all loginIds in one cms-adapter call
            //var loginIdMap = _cmsAdapterClient.GetLoginIds(new GetLoginIdsRequest { UserId = endorsement.UserId });

            // attach loginId and role to the endorsements
            foreach (var endorsement in endorsements)
            {
                // add loginId
                var searchRequest = new UsersSearchRequest { ExternalSystemUserId = endorsement.UserId, ExternalSystem = ExternalSystem.Bcsc.AsString(EnumFormat.Description) };
                var usersSearchReply = (await _userManager.SearchAsync(searchRequest));
                if (usersSearchReply.ResultStatus == Rsbc.Dmf.CaseManagement.Service.ResultStatus.Success)
                {
                    var loginId = usersSearchReply.User.FirstOrDefault()?.Id;
                    if (!string.IsNullOrEmpty(loginId))
                    {
                        endorsement.LoginId = Guid.Parse(loginId);
                    }
                }

                if (endorsement.Licences != null && endorsement.Licences.Any(endorsement => endorsement.StatusCode == LicenceStatusCode.Active))
                {
                    endorsement.Role = Roles.Practitoner;
                } 
                else 
                {
                    endorsement.Role = Roles.Moa;
                }
            }

            return endorsements;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task SetEmail (string userId, string email)
        {
            UserSetEmailRequest request = new UserSetEmailRequest()
            {
                LoginId = userId, Email = email
            };
            var result = await _userManager.SetEmailAsync(request);
        }

        public async Task<bool> IsUserInNetwork(Guid loginId)
        {
            var profile = await GetCurrentUserContext();
            return profile.Endorsements.Any(e => e.LoginId == loginId);
        }
    }
}