using Microsoft.OData.Client;
using Microsoft.OData.Edm;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface IUserManager
    {
        Task<SearchUsersResponse> SearchUsers(SearchUsersRequest request);

        Task<LoginUserResponse> LoginUser(LoginUserRequest request);

        Task<bool> SetUserEmail(string userId, string email);
        Task<Model> CreatePractitionerContact(Practitioner practitioner);
        Task<Practitioner> GetPractitionerContact(string hpdid);
    }

    public class Practitioner
    {
        public Guid UserId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string IdpId { get; set; } = string.Empty;
        public Date? Birthdate { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
        public string[] Roles { get; set; } = new string[] { };
        //public List<MedicalPractitioner> MedicalPractitioner { get; set; } = new List<MedicalPractitioner>();
    }

    public class MedicalPractitioner
    {
        public string ClinicName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
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
    }
    public class Model
    {
        public Guid? ContactId { get; set; }
        public Guid? MedicalPractictionerId { get; set; }
    }
    public abstract class User
    {
        public string Id { get; set; }
        public string ExternalSystem { get; set; }
        public string ExternalSystemUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string[] Roles { get; set;}
        public Date? Birthday { get; set; }
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
                if (request.ByType == UserType.MedicalPractitioner)
                {
                    await dynamicsContext.LoadPropertyAsync(user, nameof(dfp_login.dfp_login_dfp_role));     

                    user.dfp_login_dfp_medicalpractitioner = new Collection<dfp_medicalpractitioner>((await dynamicsContext.GetAllPagesAsync(dynamicsContext.dfp_medicalpractitioners
                        .Expand(d => d.dfp_PersonId)
                        .Expand(d => d.dfp_ClinicId)                        
                        .Where(d => d._dfp_loginid_value == user.dfp_loginid))).ToList());                    
                }
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

        public async Task<bool> SetUserEmail(string userId, string email)
        {
            bool result = false;

            var login = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_DriverId)
                .Expand(l => l.dfp_login_dfp_medicalpractitioner)                
                .Where(l => l.dfp_loginid == Guid.Parse(userId))
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

        public async Task<Practitioner> GetPractitionerContact(string hpdid)
        {
            var contact =  dynamicsContext.contacts
                .Expand(med => med.dfp_contact_dfp_medicalpractitioner)
                .Where(contact => contact.externaluseridentifier == hpdid) //contactId is the hpdid from health bcsc idp
                .SingleOrDefault();

            if (contact != null)
            {
                
                await dynamicsContext.LoadPropertyAsync(contact, nameof(contact.contactid));
                await dynamicsContext.LoadPropertyAsync(contact, nameof(contact.firstname));
                await dynamicsContext.LoadPropertyAsync(contact, nameof(contact.lastname));
                await dynamicsContext.LoadPropertyAsync(contact, nameof(contact.emailaddress1));
                await dynamicsContext.LoadPropertyAsync(contact, nameof(contact.birthdate));
                await dynamicsContext.LoadPropertyAsync(contact, nameof(contact.dfp_contact_dfp_medicalpractitioner));
                return new Practitioner
                {
                    IdpId = contact.contactid.ToString(),
                    FirstName = contact.firstname,
                    LastName = contact.lastname,
                    Birthdate = contact.birthdate,
                    Email = contact.emailaddress1
                };
            }

            return null;
        }
        public async Task<Model> CreatePractitionerContact(Practitioner practitioner)
        {

            if (practitioner == null) throw new InvalidDataException();

            //get default clinic

            var clinic = dynamicsContext.accounts
                .Where(clinic => clinic.accountid == new Guid("3bec7901-541d-ec11-b82d-00505683fbf4"))
                .FirstOrDefault();//downtown victoria clinic

            var contact = new contact
            {
                firstname = practitioner.FirstName,
                lastname = practitioner.LastName,
                contactid = practitioner.UserId,
                externaluseridentifier = practitioner.IdpId,
                emailaddress1 = practitioner.Email,
                birthdate = practitioner.Birthdate,
                gendercode = (int?)ParseExternalGender(practitioner.Gender)
            };
            var medPractitioner = new dfp_medicalpractitioner
            {
                dfp_fullname = $"{practitioner.FirstName} {practitioner.LastName}",
                dfp_medicalpractitionerid = practitioner.UserId,
                dfp_providerrole = practitioner.Roles.Any() ? (int)Enum.Parse<ProviderRole>(practitioner.Roles.FirstOrDefault()) : (int?)null
            };
            dynamicsContext.AddTocontacts(contact);
            dynamicsContext.AddTodfp_medicalpractitioners(medPractitioner);

            dynamicsContext.SetLink(medPractitioner, nameof(dfp_medicalpractitioner.dfp_PersonId), contact);
            dynamicsContext.SetLink(medPractitioner, nameof(dfp_medicalpractitioner.dfp_ClinicId), clinic);

            await dynamicsContext.SaveChangesAsync();

            dynamicsContext.DetachAll();

            return new Model
            {
                ContactId = contact.contactid,
                MedicalPractictionerId = medPractitioner.dfp_medicalpractitionerid
            };

        }
        public async Task<LoginUserResponse> LoginUser(LoginUserRequest request)
        {
            var loginType = ParseExternalSystem(request.User.ExternalSystem);
            var loginId = request.User.ExternalSystemUserId;
            string userEmail = request.User.Email ?? null;

            var login = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_DriverId)
                .Expand(a => a.dfp_login_dfp_medicalpractitioner)
                .Where(l => l.dfp_userid == loginId && l.dfp_type == (int)loginType)
                .SingleOrDefault();

            if (login == null)
            {
                //first time login
                login = new dfp_login
                {
                    dfp_loginid = Guid.NewGuid(),
                    dfp_userid = loginId,
                    dfp_type = (int)loginType
                };
                dynamicsContext.AddTodfp_logins(login);
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

                    //new driver
                    var driverEntity = AddDriver(driver);
                    dynamicsContext.SetLink(login, nameof(dfp_login.dfp_DriverId), driverEntity);
                    dynamicsContext.SetLink(driverEntity, nameof(dfp_driver.dfp_PersonId), person);
                }
                //TODO: update driver
            }
            else if (request.User is MedicalPractitionerUser medicalPractitioner)
            {
                //get or create person //A person is a contact

                //var person = login.dfp_login_dfp_medicalpractitioner.FirstOrDefault(); //this line does not make any sense. A person is unique and line only returns the first person on the list which might not be the right person
                var person = login.dfp_login_dfp_medicalpractitioner
                    .Where(person => person.dfp_PersonId.contactid == new Guid(request.User.ExternalSystemUserId))
                    .SingleOrDefault();
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
                        contactid = new Guid(request.User.ExternalSystemUserId), //using the healthcare hpdid for identification
                        firstname = request.User.FirstName,
                        lastname = request.User.LastName,
                        emailaddress1 = request.User.Email,
                        birthdate = request.User.Birthday
                    };
                    dynamicsContext.AddTocontacts(personEntity);
                }
                else
                {
                    personEntity = new contact { contactid = personId };
                };

                foreach (var cliniceAssignment in medicalPractitioner.ClinicAssignments) //there wont be any clinic for first time login. I do not understand this line of code
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

            return new LoginUserResponse { Userid = login.dfp_loginid.ToString() , Email = userEmail };
        }

        private LoginType ParseExternalSystem(string externalSystem) => externalSystem.ToLowerInvariant() switch
        {
            "bcsc" => LoginType.Bcsc,
            "bceid" => LoginType.Bceid,
            "idir" => LoginType.Idir,
            _ => throw new NotImplementedException(externalSystem)
        };
        private Gender ParseExternalGender(string gender) => gender.ToLowerInvariant() switch
        {
            "male" => Gender.male,
            "female" => Gender.female,
            "other" => Gender.other,
            _ => throw new NotImplementedException(gender)
        };
        public enum Gender
        {
            male = 1,
            female = 2,
            other = 3
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
        PRACTITIONER = 100000000,
        Dentist = 100000001,
        Optometrist = 100000005,
        Pharmacist = 10000006,
        RegisteredNurse = 100000009,
    }
}