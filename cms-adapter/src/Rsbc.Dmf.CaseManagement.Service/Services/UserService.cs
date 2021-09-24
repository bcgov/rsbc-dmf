using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                var users = (await userManager.SearchUsers(new SearchUsersRequest
                {
                    ByExternalUserId = request.ExternalSystemUserId == null ? null : (request.ExternalSystemUserId, request.ExternalSystem),
                    ByType = request.UserType == UserType.Driver ? CaseManagement.UserType.Driver : CaseManagement.UserType.MedicalPractitioner,
                    ByUserId = request.UserId
                })).Items.Select(u => new User
                {
                    ExternalSystem = u.ExternalSystem,
                    ExternalSystemUserId = u.ExternalSystemUserId,
                    LinkedProfiles = { MapUserProfiles(u) }
                });

                return new UsersSearchReply { ResultStatus = ResultStatus.Success, User = { users } };
            }
            catch (Exception e)
            {
                return new UsersSearchReply { ResultStatus = ResultStatus.Success, ErrorDetail = e.ToString() };
            }
        }

        private IEnumerable<UserProfile> MapUserProfiles(CaseManagement.User user) => user switch
        {
            DriverUser driver => new[] { new UserProfile { Driver = new DriverProfile { Id = driver.Id } } },
            MedicalPractitionerUser medicalPractictioner => medicalPractictioner.ClinicAssignments.Select(ca => new UserProfile
            {
                MedicalPractitioner = new MedicalPractitionerProfile
                {
                    Id = medicalPractictioner.Id,
                    Role = ca.Roles.FirstOrDefault(),
                    Clinic = new Clinic { Id = ca.Clinic.Id, Name = ca.Clinic.Name }
                }
            }),
            _ => throw new NotImplementedException()
        };

        public async override Task<UserLoginReply> Login(UserLoginRequest request, ServerCallContext context)
        {
            try
            {
                var driver = request.UserProfiles.SingleOrDefault(p => p.Driver != null);
                var medicalPractitionerProfiles = request.UserProfiles.Where(p => p.MedicalPractitioner != null).ToArray();

                var loginRequest = new LoginUserRequest();

                if (driver != null)
                {
                    loginRequest.User = new DriverUser
                    {
                        ExternalSystem = request.ExternalSystem,
                        ExternalSystemUserId = request.ExternalSystemUserId,
                        FirstName = request.FirstName,
                        LastName = request.LastName
                    };
                }
                else if (medicalPractitionerProfiles.Any())
                {
                    loginRequest.User = new MedicalPractitionerUser
                    {
                        ExternalSystem = request.ExternalSystem,
                        ExternalSystemUserId = request.ExternalSystemUserId,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        ClinicAssignments = medicalPractitionerProfiles.Select(p => new ClinicAssignment
                        {
                            Roles = new[] { p.MedicalPractitioner.Role },
                            Clinic = new CaseManagement.Clinic { Id = p.MedicalPractitioner.Clinic.Id }
                        })
                    };
                }
                else
                {
                    throw new Exception($"invalid login request for user {request.ExternalSystemUserId}@{request.ExternalSystem}");
                }

                var userId = (await userManager.LoginUser(loginRequest)).Userid;

                return new UserLoginReply { ResultStatus = ResultStatus.Success, UserId = userId };
            }
            catch (Exception e)
            {
                return new UserLoginReply { ResultStatus = ResultStatus.Success, ErrorDetail = e.ToString() };
            }
        }
    }
}