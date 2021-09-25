using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace RSBC.DMF.DoctorsPortal.API.Services
{
    public interface IUserService
    {
        Task<UserContext> GetCurrentUserContext();

        Task<UserContext> GetUserContext(ClaimsPrincipal user);

        Task<ClaimsPrincipal> Login(ClaimsPrincipal user);
    }

    public record UserContext
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<ClinicAssignment> ClinicAssignments { get; set; }
        public ClinicAssignment CurrentClinicAssignment => ClinicAssignments.FirstOrDefault();
    }

    public record ClinicAssignment
    {
        public string Role { get; set; }
        public string ClinicId { get; set; }
        public string ClinicName { get; set; }
    }

    public class UserService : IUserService
    {
        private readonly UserManager.UserManagerClient userManager;
        private readonly IHttpContextAccessor httpContext;
        private readonly IConfiguration configuration;

        public UserService(UserManager.UserManagerClient userManager, IHttpContextAccessor httpContext, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.httpContext = httpContext;
            this.configuration = configuration;
        }

        public async Task<UserContext> GetCurrentUserContext() => await GetUserContext(httpContext.HttpContext.User);

        public async Task<UserContext> GetUserContext(ClaimsPrincipal user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            return await Task.FromResult(new UserContext
            {
                Id = user.FindFirstValue(ClaimTypes.Sid),
                FirstName = user.FindFirstValue(ClaimTypes.GivenName),
                LastName = user.FindFirstValue(ClaimTypes.Surname),
                ClinicAssignments = user.FindAll("clinic_assignment").Select(ca => JsonSerializer.Deserialize<ClinicAssignment>(ca.Value))
            });
        }

        public async Task<ClaimsPrincipal> Login(ClaimsPrincipal user)
        {
            var clinicId = configuration["CLINIC_ID"] != null
                ? configuration["CLINIC_ID"]
                : "3bec7901-541d-ec11-b82d-00505683fbf4";

            var loginRequest = new UserLoginRequest
            {
                UserType = UserType.MedicalPractitioner,
                ExternalSystem = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/identityprovider"),
                ExternalSystemUserId = user.FindFirstValue(ClaimTypes.NameIdentifier),
                FirstName = user.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                LastName = user.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                UserProfiles = { new UserProfile
                {
                    MedicalPractitioner = new MedicalPractitionerProfile
                    {
                        Role = "Physician",
                        Clinic = new Clinic { Id = clinicId }
                    }
                }
                }
            };
            var loginResponse = await userManager.LoginAsync(loginRequest);
            if (loginResponse.ResultStatus == ResultStatus.Fail) throw new Exception(loginResponse.ErrorDetail);

            var searchResults = await userManager.SearchAsync(new UsersSearchRequest { UserId = loginResponse.UserId });
            if (searchResults.ResultStatus == ResultStatus.Fail) throw new Exception(searchResults.ErrorDetail);

            var userProfile = searchResults.User.SingleOrDefault();
            if (userProfile == null) throw new Exception($"User {loginResponse.UserId} not found");

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Sid, loginResponse.UserId));
            claims.AddRange(userProfile.LinkedProfiles.Select(p => new Claim("clinic_assignment", JsonSerializer.Serialize(new ClinicAssignment
            {
                Role = p.MedicalPractitioner.Role,
                ClinicId = p.MedicalPractitioner.Clinic.Id,
                ClinicName = p.MedicalPractitioner.Clinic.Name
            }))));

            user.AddIdentity(new ClaimsIdentity(claims));

            return user;
        }
    }
}