using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using RSBC.DMF.MedicalPortal.API.Auth.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using static RSBC.DMF.MedicalPortal.API.Auth.AuthConstant;

namespace RSBC.DMF.MedicalPortal.API.Services
{
    public interface IUserService
    {
        Task<UserContext> GetCurrentUserContext();

        Task<UserContext> GetUserContext(ClaimsPrincipal user);

        Task<ClaimsPrincipal> Login(ClaimsPrincipal user);

        Task SetEmail(string userId, string email);
    }

    public record UserContext
    {
        // OneHealth id
        public string Id { get; set; }
        // Dynamics login ids matching the above OneHealth id
        public List<string> LoginIds { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }        
        public IEnumerable<ClinicAssignment> ClinicAssignments { get; set; }
        public ClinicAssignment CurrentClinicAssignment => ClinicAssignments.FirstOrDefault();
        public string Email { get; set; }

    }

    public record ClinicAssignment
    {
        public string PractitionerId { get; set; }
        public string Role { get; set; }
        public string ClinicId { get; set; }
        public string ClinicName { get; set; }
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
                Id = user.FindFirstValue(Claims.PreferredUsername),
                LoginIds = user.FindFirstValue(Claims.LoginIds)?.Split(',').ToList(),
                FirstName = user.FindFirstValue(ClaimTypes.GivenName),
                LastName = user.FindFirstValue(ClaimTypes.Surname),
                Email = user.FindFirstValue(Claims.Email),
                //ClinicAssignments = user.FindAll("clinic_assignment").Select(ca => JsonSerializer.Deserialize<ClinicAssignment>(ca.Value))
            });
        }

        public async Task<ClaimsPrincipal> Login(ClaimsPrincipal user)
        {
            try
            {
            logger.LogDebug("Processing login {0}", user.Identity.Name);
            logger.LogDebug(" claims:\n{0}", string.Join(",\n", user.Claims.Select(c => $"{c.Type}: {c.Value}")));

            var clinicId = configuration["CLINIC_ID"] != null
                ? configuration["CLINIC_ID"]
                : "3bec7901-541d-ec11-b82d-00505683fbf4";

                var role = user.GetRoles().SingleOrDefault();
                var loginRequest = new UserLoginRequest();
                loginRequest.UserType = UserType.MedicalPractitionerUserType;
                loginRequest.Email = user.FindFirstValue(Claims.Email);
                loginRequest.ExternalSystem = user.GetIdentityProvider();
                loginRequest.ExternalSystemUserId = user.FindFirstValue(Claims.PreferredUsername);
                loginRequest.FirstName = user.FindFirstValue(ClaimTypes.GivenName);
                loginRequest.LastName = user.FindFirstValue(ClaimTypes.Surname);
                var userProfileRequest = new UserProfile();
                userProfileRequest.MedicalPractitioner = new MedicalPractitionerProfile();
                // TODO need to uncomment this line to get roles working
                //userProfileRequest.MedicalPractitioner.Role = user.GetRoles().SingleOrDefault();
                userProfileRequest.MedicalPractitioner.Role = user.FindFirstValue(ClaimTypes.Role);
                userProfileRequest.MedicalPractitioner.Clinic = new Clinic { Id = clinicId };
                loginRequest.UserProfiles.Add(userProfileRequest);

                // TODO uncomment after roles are fixed
                //if (!user.IsInRole(Roles.Moa) || !user.IsInRole(Roles.Practitoner)) throw new Exception("User not enrolled");
            
            var loginResponse = await userManager.LoginAsync(loginRequest);
            if (loginResponse.ResultStatus == ResultStatus.Fail) throw new Exception(loginResponse.ErrorDetail);

            var searchResults = await userManager.SearchAsync(new UsersSearchRequest { UserId = loginResponse.UserId });
            if (searchResults.ResultStatus == ResultStatus.Fail) throw new Exception(searchResults.ErrorDetail);

            var userProfile = searchResults.User.SingleOrDefault();
            if (userProfile == null) throw new Exception($"User {loginResponse.UserId} not found");

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Sid, loginResponse.UserId));
                //claims.Add(new Claim(ClaimTypes.Email, loginResponse.UserEmail));
            claims.Add(new Claim(ClaimTypes.Upn, $"{userProfile.ExternalSystemUserId}@{userProfile.ExternalSystem}"));
            claims.Add(new Claim(ClaimTypes.GivenName, userProfile.FirstName));
                //claims.Add(new Claim(ClaimTypes.GivenName, userProfile.FirstName));
                //claims.Add(new Claim(ClaimTypes.Surname, userProfile.LastName));
                //claims.AddRange(userProfile.LinkedProfiles.Select(p => new Claim("clinic_assignment", JsonSerializer.Serialize(new ClinicAssignment
                //{
                //    PractitionerId = p.MedicalPractitioner.Id,
                //    Role = p.MedicalPractitioner.Role,
                //    ClinicId = p.MedicalPractitioner.Clinic.Id,
                //    ClinicName = p.MedicalPractitioner.Clinic.Name
                //}))));

            user.AddIdentity(new ClaimsIdentity(claims));

            logger.LogInformation("User {0} ({1}@{2}) logged in", userProfile.Id, userProfile.ExternalSystemUserId, userProfile.ExternalSystem);

            return user;
        }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error logging in user {0}", user.Identity.Name);
                throw;
            }
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
            var result = await userManager.SetEmailAsync(request);
          }
        }
}