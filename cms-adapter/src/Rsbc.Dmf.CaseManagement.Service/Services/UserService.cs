using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement.Service
{
    public class UserService : UserManager.UserManagerBase
    {
        private readonly IUserManager _userManager;
        private readonly IMapper _mapper;

        public UserService(IUserManager userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        #region Practitioner

        public async override Task<PractitionerReply> GetPractitionerContact(PractitionerRequest request, ServerCallContext context)
        {
            try
            {
                var getPcontact = await _userManager.GetPractitionerContact(new CaseManagement.PractitionerRequest { hpdid = request.Hpdid });

                if (getPcontact.contactId == string.Empty) { return new PractitionerReply(); }

                return new PractitionerReply
                {
                    FirstName = getPcontact.FirstName,
                    LastName = getPcontact.LastName,
                    Email = getPcontact.Email,
                    ContactId = getPcontact.contactId,
                    Gender = getPcontact.Gender,
                    IdpId = getPcontact.IdpId,
                    Birthdate = Timestamp.FromDateTime(DateTime.SpecifyKind(getPcontact.Birthdate.Value, DateTimeKind.Utc)),
                    Role = getPcontact.Role,
                    ClinicName = getPcontact.ClinicName
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion Practitioner

        public async override Task<UsersSearchReply> Search(UsersSearchRequest request, ServerCallContext context)
        {
            try
            {
                var users = (await _userManager.SearchUsers(new SearchUsersRequest
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

                var loginResult = await _userManager.LoginUser(loginRequest);

                var userLoginReply = new UserLoginReply { ResultStatus = ResultStatus.Success };
                userLoginReply.DriverLicenseNumber = loginResult.DriverLicenseNumber;
                userLoginReply.UserId = loginResult.Userid;
                userLoginReply.DriverId = loginResult.DriverId;
                userLoginReply.UserEmail = loginResult.Email ?? String.Empty;
                return userLoginReply;
            }
            catch (Exception e)
            {
                return new UserLoginReply { ResultStatus = ResultStatus.Fail, ErrorDetail = e.ToString() };
            }
        }

        // same as UpdateEmail but for medical practitioners
        public async override Task<ResultStatusReply> SetEmail(UserSetEmailRequest request, ServerCallContext context)
        {
            ResultStatusReply result = new ResultStatusReply();
            result.ResultStatus = ResultStatus.Fail;

            if (await _userManager.SetUserEmail(request.LoginId, request.Email))
            {
                result.ResultStatus = ResultStatus.Success;
            }
            
            return result;
        }

        public async override Task<SetDriverLoginReply> SetDriverLogin(SetDriverLoginRequest request, ServerCallContext context)
        {
            var result = new SetDriverLoginReply();

            try
            {
                Guid? driverId = null;
                if (request.DriverId != string.Empty)
                {
                    driverId = Guid.Parse(request.DriverId);
                }
                result.HasDriver = await _userManager.SetDriverLogin(Guid.Parse(request.LoginId), driverId);
                result.ResultStatus = ResultStatus.Success;
            } 
            catch (Exception ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = ex.Message;
            }

            return result;
        }

        public async override Task<ResultStatusReply> UpdateLogin(UpdateLoginRequest request, ServerCallContext context)
        {
            var result = new ResultStatusReply();

            try
            {
                var userUpdateRequest = _mapper.Map<CaseManagement.UpdateLoginRequest>(request);
                await _userManager.UpdateLogin(userUpdateRequest);
                result.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = ex.Message;
            }

            return result;
        }

        // same as SetEmail but for driver portal
        public async override Task<ResultStatusReply> UpdateEmail(UserSetEmailRequest request, ServerCallContext context)
        {
            var result = new ResultStatusReply();

            try
            {
                await _userManager.UpdateEmail(Guid.Parse(request.LoginId), request.Email);
                result.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = ex.Message;
            }

            return result;
        }
    }
}
