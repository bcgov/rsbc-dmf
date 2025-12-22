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
                    // TODO remove this after regression testing UsersSearchReply
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
                        LastName = request.LastName
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
                if (loginResult.LoginIds?.Count > 0)
                {
                    userLoginReply.LoginIds.AddRange(loginResult.LoginIds);
                }
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

        public async override Task<GetUserContactReply> GetUserContact(GetUserContactRequest request, ServerCallContext context)
        {

            var reply = new GetUserContactReply();
            try
            {

                var getContact = await _userManager.GetUserContact(new CaseManagement.GetUserContactRequest { externalSystemUserId = request.ExternalSystemUserId });
                if (getContact?.contact == null)
                {
                    reply.ResultStatus = ResultStatus.Fail; // or NotFound if you have such a status
                    reply.ErrorDetail = "Contact not found";
                    return reply;
                }

                reply.Contact = new UserContact
                   {
                       ContactId = getContact.contact.Id ?? string.Empty,
                       //ExternalSystem = getContact.contact.ExternalSystem ?? string.Empty,
                       //ExternalSystemUserId = getContact.contact.ExternalSystemUserId ?? string.Empty,
                       GivenName = getContact.contact.GivenName ?? string.Empty,
                       SecondGivenName = getContact.contact.SecondGivenName ?? string.Empty,
                       ThirdGivenName = getContact.contact.ThirdGivenName ?? string.Empty,
                       Surname = getContact.contact.SurName ?? string.Empty,
                       AddressFirstLine = getContact.contact.AddressFirstLine ?? string.Empty,
                       AddressSecondLine = getContact.contact.AddressSecondLine ?? string.Empty,
                       AddressThirdLine = getContact.contact.AddressThirdLine ?? string.Empty,
                       City = getContact.contact.City ?? string.Empty,
                       Province = getContact.contact.Province ?? string.Empty,
                       Country = getContact.contact.Country ?? string.Empty,
                       PostalCode = getContact.contact.PostalCode ?? string.Empty,
                       EmailAddress = getContact.contact.EmailAddress ?? string.Empty,
                       PhoneNumber = getContact.contact.PhoneNumber ?? string.Empty,
                       CellPhoneNumber = getContact.contact.CellPhoneNumber ?? string.Empty,

                };
                reply.ResultStatus = ResultStatus.Success;

            }
            catch (Exception ex)
            {

                
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }
            return reply;
        }

        public async override Task<SetUserContactLoginReply> SetUserContactLogin(SetUserContactLoginRequest request, ServerCallContext context)
        {
            var result = new SetUserContactLoginReply();

            try
            {
                Guid? contactId = null;
                if (request.ContactId != string.Empty)
                {
                    contactId = Guid.Parse(request.ContactId);
                }
                result.HasContact = await _userManager.SetUserContactLogin(Guid.Parse(request.LoginId), contactId);
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


        // Partner Portal


        public async override Task<PartnerPortalLoginReply> PartnerPortalLogin(PartnerPortalLoginRequest request, ServerCallContext context)
        {
            try
            {
                var loginRequest = new CaseManagement.PartnerPortalLoginRequest()
                {
                    contact = new CaseManagement.UserContact()
                    {
                        
                        ExternalSystem = request.ExternalSystem,
                        ExternalSystemUserId = request.ExternalSystemUserId,
                        GivenName = request.FirstName,
                        SurName = request.LastName
                    }

                };
                

                var loginResult = await _userManager.PartnerPortalLoginUser(loginRequest);
                var userLoginReply = new PartnerPortalLoginReply { ResultStatus = ResultStatus.Success };
                userLoginReply.UserId = loginResult.Userid;
                if (loginResult.LoginIds?.Count > 0)
                {
                    userLoginReply.LoginIds.AddRange(loginResult.LoginIds);
                }
                return userLoginReply;

            }

            catch (Exception e)
            {
                return new PartnerPortalLoginReply { ResultStatus = ResultStatus.Fail, ErrorDetail = e.ToString() };
            }
        }


        public async override Task<PartnerPortalUserSearchReply> PartnerPortalSearch(PartnerPortalUserSearchRequest request, ServerCallContext context)
        {
            try
            {
                var users = (await _userManager.PartnerPortalSearchUsers(new PartnerPortalSearchRequest
                {
                    ByExternalUserId = string.IsNullOrEmpty(request.ExternalSystemUserId) ? null : (request.ExternalSystemUserId, request.ExternalSystem),
                    //ByType = request.UserType == UserType.PartnerPortalUserType
                    ByUserId = request.UserId
                })).Items.Select(u => new User
                {
                    Id = u.Id,
                    FirstName = u.GivenName ?? string.Empty,
                    LastName = u.SurName ?? string.Empty,
                    ExternalSystem = u.ExternalSystem,
                    ExternalSystemUserId = u.ExternalSystemUserId,
                });

                return new PartnerPortalUserSearchReply { ResultStatus = ResultStatus.Success, };
            }
            catch (Exception e)
            {
                return new PartnerPortalUserSearchReply { ResultStatus = ResultStatus.Fail, ErrorDetail = e.ToString() };
            }
        }

        public async override Task<UserContactReply> CreateUserContact(UserContactRequest request, ServerCallContext context)
        {
            var reply = new UserContactReply();

            try
            {
                var userAccessRequest = _mapper.Map<CaseManagement.UserContact>(request.Contact);
                var result = await _userManager.CreateUserContact(userAccessRequest);
                if (result != null && result.Success)
                {
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    reply.ResultStatus = ResultStatus.Fail;
                }
            }
            catch (Exception ex)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = ex.Message;
            }

            return reply;
        }

    }
}
