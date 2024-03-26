using Microsoft.OData.Client;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface IUserManager
    {
        Task<SearchUsersResponse> SearchUsers(SearchUsersRequest request);
        Task<LoginUserResponse> LoginUser(LoginUserRequest request);
        Task<bool> SetUserEmail(string loginId, string email);
        Task<bool> UpdateEmail(Guid loginId, string email);
        Task<bool> SetDriverLogin(Guid loginId, Guid driverId);
        Task<bool> UpdateLogin(UpdateLoginRequest request);
        bool IsDriverAuthorized(string userId, Guid driverId);
    }

    public class UpdateLoginRequest
    {
        public Guid LoginId { get; set; }
        public string Email { get; set; }
        public bool NotifyByMail { get; set; }
        public bool NotifyByEmail { get; set; }
        public string ExternalSystemUserId { get; set; }
        public FullAddress Address { get; set; }
    }

    public class FullAddress
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postal { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
    }

    public class SearchUsersRequest
    {
        public string ByUserId { get; set; }
        public (string externalUserId, string externalSystem)? ByExternalUserId { get; set; }
        public UserType ByType { get; set; }
    }

    public enum UserType
    {
        MedicalPractitioner,
        Driver
    }

    public class SearchUsersResponse
    {
        public IEnumerable<User> Items { get; set; }
    }

    public class LoginUserRequest
    {
        public User User { get; set; }
    }

    public class LoginUserResponse
    {
        public string Userid { get; set; }
        public string Email { get; set; }
        public string DriverId { get; set; }
        public string DriverLicenseNumber { get; set; }
    }

    public abstract class User
    {
        public string Id { get; set; }
        public string ExternalSystem { get; set; }
        public string ExternalSystemUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string DriverId { get; set; }

        public string[] Roles { get; set;}
    }

    public class MedicalPractitionerUser : User
    {   
        public IEnumerable<ClinicAssignment> ClinicAssignments { get; set; }
    }

    public class DriverUser : User { }

    public class ClinicAssignment
    {
        public string[] Roles { get; set; }
        public Clinic Clinic { get; set; }
    }

    public class Clinic
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    internal class UserManager : IUserManager
    {
        private readonly DynamicsContext dynamicsContext;

        public UserManager(DynamicsContext dynamicsContext)
        {
            this.dynamicsContext = dynamicsContext;
        }

        public bool IsDriverAuthorized(string userId, Guid driverId)
        {
            var loggedInDriver = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_DriverId)
                .Where(l => l.dfp_userid == userId && l.statecode == (int)EntityState.Active)
                .Where(d => d._dfp_driverid_value == driverId && d.dfp_DriverId.statecode == (int)EntityState.Active)
                .ToList()
                .SingleOrDefault();

            return loggedInDriver != default;
        }

        public async Task<SearchUsersResponse> SearchUsers(SearchUsersRequest request)
        {
            IQueryable<dfp_login> query = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_DriverId)
                .Expand(l => l.dfp_login_dfp_role)
                .Where(l => l.statecode == (int)EntityState.Active);

            if (!string.IsNullOrEmpty(request.ByUserId)) query = query.Where(l => l.dfp_loginid == Guid.Parse(request.ByUserId));
            if (request.ByExternalUserId.HasValue) query = query.Where(l => l.dfp_userid == request.ByExternalUserId.Value.externalUserId &&
                l.dfp_type == (int)ParseExternalSystem(request.ByExternalUserId.Value.externalSystem));

            var users = (await ((DataServiceQuery<dfp_login>)query).GetAllPagesAsync()).ToArray();

            foreach (var user in users)
            {
                // ensure login role data is present
                
                //await dynamicsContext.LoadPropertyAsync(user, nameof(dfp_login.dfp_login_dfp_medicalpractitioner));

                if (request.ByType == UserType.Driver && user._dfp_driverid_value.HasValue)
                {
                    await dynamicsContext.LoadPropertyAsync(user.dfp_DriverId, nameof(dfp_driver.dfp_PersonId));
                }

                /*
                if (request.ByType == UserType.MedicalPractitioner)
                {
                    await dynamicsContext.LoadPropertyAsync(user, nameof(dfp_login.dfp_login_dfp_role));     

                    user.dfp_login_dfp_medicalpractitioner = new DataServiceCollection<dfp_medicalpractitioner>((await dynamicsContext.GetAllPagesAsync(dynamicsContext.dfp_medicalpractitioners
                        .Expand(d => d.dfp_PersonId)
                        .Expand(d => d.dfp_ClinicId)                        
                        .Where(d => d._dfp_loginid_value == user.dfp_loginid))).ToList());                    
                }
                */
            }
           
            dynamicsContext.DetachAll();

            IEnumerable<User> mappedUsers;
            
            switch (request.ByType)
            {
                case UserType.Driver:
                    mappedUsers = users.Select(u => new DriverUser
                    {
                        Id = u.dfp_loginid.ToString(),
                        FirstName = u.dfp_DriverId?.dfp_PersonId.firstname,
                        LastName = u.dfp_DriverId?.dfp_PersonId.lastname,
                        Email = u.dfp_DriverId?.dfp_PersonId.emailaddress1,
                        ExternalSystem = ((LoginType)u.dfp_type).ToString(),
                        ExternalSystemUserId = u.dfp_userid
                    });
                    break;
                case UserType.MedicalPractitioner:

                    // get the user's name.

                    mappedUsers = users.Select(u =>
                    {
                        var firstPractitioner = u.dfp_login_dfp_medicalpractitioner.FirstOrDefault();

                        Guid? personId = firstPractitioner?.dfp_PersonId?.contactid;

                        contact person = null;
                        if (personId != null)
                        {
                            person = dynamicsContext.contacts.Where(x => x.contactid == personId).FirstOrDefault();
                        }

                        var firstRole = u.dfp_login_dfp_role.FirstOrDefault();

                        return new MedicalPractitionerUser()
                        {
                            Id = u.dfp_loginid.ToString(),
                            FirstName = person?.firstname,
                            LastName = person?.lastname,
                            Email = person?.emailaddress1,
                            ExternalSystem = ((LoginType)u.dfp_type).ToString(),
                            ExternalSystemUserId = u.dfp_userid,
                            Roles = u.dfp_login_dfp_role.Select(r => r.dfp_name).ToArray(),
                            ClinicAssignments = u.dfp_login_dfp_medicalpractitioner.Select(mp => new ClinicAssignment
                            {
                                
                                Roles = new[] { firstRole?.dfp_name },
                                Clinic = new Clinic
                                {
                                    Id = mp.dfp_ClinicId?.accountid.ToString(),
                                    Name = mp.dfp_ClinicId?.name
                                }
                            })
                        };


                    });


                    break;    
                    default:
                        throw new ArgumentException();
            }
                        
            return new SearchUsersResponse
            {
                Items = mappedUsers.ToArray()
            };
        }

        public async Task<bool> SetDriverLogin(Guid loginId, Guid driverId)
        {
            var login = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_DriverId)
                .Expand(l => l.dfp_DriverId.dfp_PersonId)
                .Where(l => l.dfp_loginid == loginId)
                .SingleOrDefault();

            // if we change userRegistration page to not need authentication (which creates the login in method 'LoginUser')
            // we will need the following line:
            // if (login == null) { login = CreateLogin(loginId, LoginType.Bcsc); }

            // for first time login, create login and link to driver
            if (login.dfp_DriverId == null)
            {
                // Query to get the driver
                var driver = dynamicsContext.dfp_drivers.Where(x => x.dfp_driverid == driverId).FirstOrDefault();
                if (driver != null)
                {
                    dynamicsContext.SetLink(login, nameof(dfp_login.dfp_DriverId), driver);
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.Detach(driver);
                } 
                else
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> UpdateLogin(UpdateLoginRequest request)
        {
            //fetch the login and driver again
            var login = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_DriverId)
                .Expand(l => l.dfp_DriverId.dfp_PersonId)
                .Where(l => l.dfp_loginid == request.LoginId)
                .SingleOrDefault();

            // update notification preferences
            login.dfp_mail = request.NotifyByMail;
            login.dfp_email = request.NotifyByEmail;
            login.dfp_name = request.ExternalSystemUserId;
                
            dynamicsContext.UpdateObject(login);

            // update email
            if (login.dfp_DriverId.dfp_PersonId != null)
            {
                login.dfp_DriverId.dfp_PersonId.emailaddress1 = request.Email;
                if (request.Address != null)
                {
                    login.dfp_DriverId.dfp_PersonId.address1_city = request.Address.City;
                    login.dfp_DriverId.dfp_PersonId.address1_country = request.Address.Country;
                    login.dfp_DriverId.dfp_PersonId.address1_line1 = request.Address.Line1;
                    login.dfp_DriverId.dfp_PersonId.address1_postalcode = request.Address.Postal;
                    login.dfp_DriverId.dfp_PersonId.address1_stateorprovince = request.Address.Province;
                }
                dynamicsContext.UpdateObject(login.dfp_DriverId.dfp_PersonId);
            }
            else
            {
                return false;
            }

            await dynamicsContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEmail(Guid loginId, string email)
        {
            var login = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_DriverId)
                .Expand(l => l.dfp_DriverId.dfp_PersonId)
                .Where(l => l.dfp_loginid == loginId)
                .SingleOrDefault();

            // update email
            if (login.dfp_DriverId.dfp_PersonId != null)
            {
                login.dfp_DriverId.dfp_PersonId.emailaddress1 = email;

                dynamicsContext.UpdateObject(login.dfp_DriverId.dfp_PersonId);
            }
            else
            {
                return false;
            }

            await dynamicsContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetUserEmail(string loginId, string email)
        {
            bool result = false;

            var login = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_DriverId)
                .Expand(l => l.dfp_login_dfp_medicalpractitioner)                
                .Where(l => l.dfp_loginid == Guid.Parse(loginId))
                .FirstOrDefault();

            if (login != null)
            {
                // get the first medical practitioner and update their email
                var person = login.dfp_login_dfp_medicalpractitioner.FirstOrDefault();
                if (person != null)
                {
                    await dynamicsContext.LoadPropertyAsync(person, nameof(dfp_medicalpractitioner.dfp_PersonId));
                    
                    if (person.dfp_PersonId != null)
                    {
                        person.dfp_PersonId.emailaddress1 = email;

                        dynamicsContext.UpdateObject(person.dfp_PersonId);
                        dynamicsContext.SaveChanges();
                        result = true;
                    }                    
                }
            }

            return result;
        }

        public async Task<LoginUserResponse> LoginUser(LoginUserRequest request)
        {
            var loginType = ParseExternalSystem(request.User.ExternalSystem);
            var loginId = request.User.ExternalSystemUserId;
            string userEmail = null;

            var login = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_DriverId)
                .Where(l => l.dfp_userid == loginId && l.dfp_type == (int)loginType)
                .SingleOrDefault();

            if (login == null)
            {
                login = CreateLogin(loginId, loginType);
            }
            else
            {
                await dynamicsContext.LoadPropertyAsync(login, nameof(dfp_login.dfp_login_dfp_medicalpractitioner));
            }

            if (request.User is DriverUser driver)
            {
                if (!login._dfp_driverid_value.HasValue)
                {
                    //new person
                    var person = new contact
                    {
                        contactid = Guid.NewGuid(),
                        firstname = request.User.FirstName,
                        lastname = request.User.LastName
                    };
                    dynamicsContext.AddTocontacts(person);

                    // Do not add driver record at this time; it will happen during user registration
                }
            }
            else if (request.User is MedicalPractitionerUser medicalPractitioner)
            {
                //get or create person

                var person = login.dfp_login_dfp_medicalpractitioner.FirstOrDefault();
                if (person != null)
                {
                    await dynamicsContext.LoadPropertyAsync(person, nameof(dfp_medicalpractitioner.dfp_PersonId));
                    userEmail = person.dfp_PersonId?.emailaddress1;
                }
                
                var personId = person?._dfp_personid_value;
                
                contact personEntity;
                if (!personId.HasValue)
                {
                    personEntity = new contact
                    {
                        contactid = Guid.NewGuid(),
                        firstname = request.User.FirstName,
                        lastname = request.User.LastName
                    };
                    dynamicsContext.AddTocontacts(personEntity);
                }
                else
                {
                    personEntity = new contact { contactid = personId };
                };

                foreach (var cliniceAssignment in medicalPractitioner.ClinicAssignments)
                {
                    if (!login.dfp_login_dfp_medicalpractitioner.Any(mp => mp._dfp_clinicid_value == Guid.Parse(cliniceAssignment.Clinic.Id)))
                    {
                        //create a new clinic assignment if doesn't exist
                        var medicalPractitionerEntity = AddMedicalPractitioner(cliniceAssignment);
                        dynamicsContext.AddLink(login, nameof(dfp_login.dfp_login_dfp_medicalpractitioner), medicalPractitionerEntity);
                        dynamicsContext.SetLink(medicalPractitionerEntity, nameof(dfp_driver.dfp_PersonId), personEntity);
                    }
                }
            }
            await dynamicsContext.SaveChangesAsync();

            dynamicsContext.DetachAll();

            var loginUserResponse = new LoginUserResponse { Userid = login.dfp_loginid.ToString(), Email = userEmail, DriverId = login._dfp_driverid_value.ToString() };
            loginUserResponse.DriverLicenseNumber = login.dfp_DriverId?.dfp_licensenumber ?? ""; ;
            return loginUserResponse;
        }

        private LoginType ParseExternalSystem(string externalSystem) => externalSystem.ToLowerInvariant() switch
        {
            "bcsc" => LoginType.Bcsc,
            "bceid" => LoginType.Bceid,
            "idir" => LoginType.Idir,
            _ => throw new NotImplementedException(externalSystem)
        };

        // first time login
        private dfp_login CreateLogin(string userId, LoginType? loginType)
        {
            var login = new dfp_login
            {
                dfp_loginid = Guid.NewGuid(),
                dfp_userid = userId,
                dfp_type = (int?)loginType
            };
            dynamicsContext.AddTodfp_logins(login);

            dynamicsContext.SaveChanges();
            return login;
        }

        private dfp_driver AddDriver(DriverUser user)
        {
            var driver = new dfp_driver
            {
                dfp_driverid = Guid.NewGuid()
            };
            dynamicsContext.AddTodfp_drivers(driver);

            return driver;
        }

        private dfp_medicalpractitioner AddMedicalPractitioner(ClinicAssignment clinicAssignment)
        {
            var medicalPractitioner = new dfp_medicalpractitioner
            {
                dfp_medicalpractitionerid = Guid.NewGuid(),
                dfp_providerrole = clinicAssignment.Roles.Any() ? (int)Enum.Parse<ProviderRole>(clinicAssignment.Roles.FirstOrDefault()) : (int?)null
            };
            dynamicsContext.AddTodfp_medicalpractitioners(medicalPractitioner);

            var clinicEntity = dynamicsContext.accounts.Where(a => a.statecode == (int)EntityState.Active && a.accountid == Guid.Parse(clinicAssignment.Clinic.Id)).FirstOrDefault();

            if (clinicEntity == null) throw new Exception($"Clinic id {clinicAssignment.Clinic.Id} not found");

            dynamicsContext.SetLink(medicalPractitioner, nameof(dfp_medicalpractitioner.dfp_ClinicId), clinicEntity);

            return medicalPractitioner;
        }
    }

    internal enum LoginType
    {
        Bcsc = 100000000,
        Bceid = 100000001,
        Idir = 100000002
    }

    internal enum ProviderRole
    {
        Physician = 100000000,
        Dentist = 100000001,
        Optometrist = 100000005,
        Pharmacist = 10000006,
        RegisteredNurse = 100000009,
    }
}