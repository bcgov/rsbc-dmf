using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement.Service
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
                    ByExternalUserId = string.IsNullOrEmpty(request.ExternalSystemUserId) ? null : (request.ExternalSystemUserId, request.ExternalSystem),
                    ByType = request.UserType == UserType.DriverUserType ? CaseManagement.UserType.Driver : CaseManagement.UserType.MedicalPractitioner,
                    ByUserId = request.UserId
                })).Items.Select(u => new User
                {
                    Id = u.Id,
                    FirstName = u.FirstName ?? string.Empty,
                    LastName = u.LastName ?? string.Empty,
                    ExternalSystem = u.ExternalSystem,
                    ExternalSystemUserId = u.ExternalSystemUserId,
                    LinkedProfiles = { MapUserProfiles(u) },                                        
                });

                return new UsersSearchReply { ResultStatus = ResultStatus.Success, User = { users } };
            }
            catch (Exception e)
            {
                return new UsersSearchReply { ResultStatus = ResultStatus.Fail, ErrorDetail = e.ToString() };
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
                    Role = ca.Roles.FirstOrDefault() ?? String.Empty,
                    Clinic = new Clinic { Id = ca.Clinic.Id, Name = ca.Clinic.Name }
                }
            }),
            _ => throw new NotImplementedException()
        };

        public async override Task<UserLoginReply> Login(UserLoginRequest request, ServerCallContext context)
        {
            try
            {
                var loginRequest = new LoginUserRequest();

                if (request.UserType == UserType.DriverUserType)
                {
                    var driver = request.UserProfiles.SingleOrDefault(p => p.Driver != null);
                    loginRequest.User = new DriverUser
                    {
                        ExternalSystem = request.ExternalSystem,
                        ExternalSystemUserId = request.ExternalSystemUserId,
                        FirstName = request.FirstName,
                        LastName = request.LastName
                    };
                }
                else if (request.UserType == UserType.MedicalPractitionerUserType)
                {
                    var medicalPractitionerProfiles = request.UserProfiles.Where(p => p.MedicalPractitioner != null).ToArray();

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

                var loginResult = await userManager.LoginUser(loginRequest);

                var userId = loginResult.Userid;
                var userEmail = loginResult.Email;

                return new UserLoginReply { ResultStatus = ResultStatus.Success, UserId = userId, UserEmail = userEmail ?? String.Empty };
            }
            catch (Exception e)
            {
                return new UserLoginReply { ResultStatus = ResultStatus.Fail, ErrorDetail = e.ToString() };
            }
        }


        public async override Task<ResultStatusReply> SetEmail(UserSetEmailRequest request, ServerCallContext context)
        {
            ResultStatusReply result = new ResultStatusReply();
            result.ResultStatus = ResultStatus.Fail;

            if (await userManager.SetUserEmail(request.UserId, request.Email))
            {
                result.ResultStatus = ResultStatus.Success;
            }
            
            return result;
        }
    }
}