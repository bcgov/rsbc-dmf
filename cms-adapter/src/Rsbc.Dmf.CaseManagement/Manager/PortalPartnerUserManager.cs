using Microsoft.OData.Client;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface IPortalPartnerUserManager
    {
        Task<IEnumerable<PortalUser>> SearchSystemUsers(SearchPortalPatnerUsersRequest request);
        Task UpdateContact(PortalUser contact);
        Task<IEnumerable<UserRoles>> GetContactRoles();
        Task AddContactRole(string roleId, string contactId, string modifiedBy);
        Task RemoveContactRole(Guid roleId, Guid contactId);
        Task<CurrentLoginUser> GetCurrentLoginUser(string userId);
    }

    public class SearchPortalPatnerUsersRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public bool? UnauthorizedOnly { get; set; }
        public int? ActiveUser { get; set; }
        public string ByUserId { get; set; }
    }


    public class PortalUser
    {
        public Guid? Id { get; set; }
        public bool Active { get; set; }
        public bool Authorized { get; set; }
        public string FirstName { get; set; }
        public string SecondGivenName { get; set; }
        public string ThirdGivenName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public string PhoneNumber { get; set; }
        public string CellNumber { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public DateTimeOffset? EffectiveDate { get; set; }
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string DFWebuserId { get; set; }
        public string LastName { get; set; }
        public IEnumerable<UserRoles> UserRoles { get; set; }
        public IEnumerable<AuditDetails> AuditDetails { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class UserRoles
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }

    public class AuditDetails
    {
        public DateTimeOffset? EntryDate { get; set; }
        public string Description { get; set; }
        public string EntryId { get; set; }
    }

    public class CurrentLoginUser
    {
        public List<string> UserRoles { get; set; }
    }


    internal class PortalPartnerUserManager : IPortalPartnerUserManager
    {
        private readonly DynamicsContext dynamicsContext;


        public PortalPartnerUserManager(DynamicsContext dynamicsContext)
        {
            this.dynamicsContext = dynamicsContext;
        }

        public async Task<IEnumerable<PortalUser>> SearchSystemUsers(SearchPortalPatnerUsersRequest request)
        {
            try
            {
                IQueryable<dfp_login> query = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_Person).Where(l => l.dfp_Person != null);

                IQueryable<dfp_login> query1 = dynamicsContext.dfp_logins
                .Expand(l => l.dfp_Person)
                .Expand(l => l.dfp_Person.bcgov_contact_bcgov_portalroleassignment_Person)
                .Expand(l => l.dfp_Person.bcgov_contact_bcgov_portalroleassignment_Person.Select(p => p.bcgov_PortalRole)).Where(l => l.dfp_Person != null);

                if (!string.IsNullOrEmpty(request.FirstName))
                {
                    query = query.Where(u => u.dfp_Person.firstname.Contains(request.FirstName));
                }
                if (!string.IsNullOrEmpty(request.LastName))
                {
                    query = query.Where(u => u.dfp_Person.lastname.Contains(request.LastName));
                }
                if (!string.IsNullOrEmpty(request.UserId))
                {
                    query = query.Where(u => u.dfp_Person.bcgov_userid.Contains(request.ByUserId));
                }
                if (request.UnauthorizedOnly == true)
                {
                    query = query.Where(u => u.dfp_Person.bcgov_approvalstatus == null || u.dfp_Person.bcgov_approvalstatus == 931490000);
                }
                if (request.ActiveUser == 0)
                {
                    query = query.Where(u => u.dfp_Person.bcgov_activeinportal != true);
                }
                if (request.ActiveUser == 1)
                {
                    query = query.Where(u => u.dfp_Person.bcgov_activeinportal == true);
                }


                var users = (await ((DataServiceQuery<dfp_login>)query).GetAllPagesAsync()).ToArray();

                var personIds = users
                    .Select(u => u.dfp_Person?.contactid)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                var result = new List<PortalUser>();
                foreach (var user in users)
                {
                    var roleQuery = dynamicsContext.bcgov_portalroleassignments
                        .Expand(r => r.bcgov_PortalRole)
                        .Expand(r => r.bcgov_Person)
                        .Where(r => r.bcgov_Person.contactid == user.dfp_Person.contactid);
                    var roleAssignments = (await ((DataServiceQuery<bcgov_portalroleassignment>)roleQuery).GetAllPagesAsync()).ToArray();

                    var auditQuery = dynamicsContext.bcgov_portalauditdetailses
                        .Where(r => r._bcgov_person_value == user.dfp_Person.contactid).OrderByDescending(r => r.createdon);

                    var auditDetails = (await ((DataServiceQuery<bcgov_portalauditdetails>)auditQuery).GetAllPagesAsync()).ToArray();

                    var portalUser = new PortalUser
                    {
                        Id = user.dfp_Person?.contactid,
                        Active = (user.dfp_Person?.bcgov_expirydate == null || DateTimeOffset.Now < user?.dfp_Person?.bcgov_expirydate) && DateTimeOffset.Now > user?.dfp_Person?.bcgov_effectivedate,
                        Authorized = user.dfp_Person?.bcgov_approvalstatus == 931490001,
                        AddressLine1 = user.dfp_Person?.address1_line1,
                        AddressLine2 = user.dfp_Person?.address1_line2,
                        AddressLine3 = user.dfp_Person?.address1_line3,
                        City = user.dfp_Person?.address1_city,
                        Province = user.dfp_Person?.address1_stateorprovince,
                        Country = user.dfp_Person?.address1_country,
                        PostCode = user.dfp_Person?.address1_postalcode,
                        PhoneNumber = user.dfp_Person?.address1_telephone1,
                        CellNumber = user.dfp_Person?.mobilephone,
                        Email = user.dfp_Person?.emailaddress1,
                        FirstName = user.dfp_Person?.firstname,
                        SecondGivenName = user.dfp_Person?.middlename,
                        ThirdGivenName = user.dfp_Person?.bcgov_thirdgivenname,
                        LastName = user.dfp_Person?.lastname,
                        ExpiryDate = user.dfp_Person?.bcgov_expirydate,
                        EffectiveDate = user.dfp_Person?.bcgov_effectivedate,
                        DFWebuserId = user.dfp_Person?.bcgov_dfwebuserid,
                        Domain = GetDomainName(user.dfp_type),
                        UserName = user.dfp_name,
                        UserRoles = roleAssignments.Select(r => new UserRoles
                        {
                            Id = r.bcgov_PortalRole?.bcgov_portalroleid?.ToString(),
                            Name = r.bcgov_PortalRole?.bcgov_name,
                            Description = r.bcgov_PortalRole?.bcgov_description
                        }).ToList(),
                        AuditDetails = auditDetails?.Select(r => new AuditDetails
                        {
                            EntryId = r.bcgov_entryid,
                            EntryDate = r.createdon,
                            Description = r.bcgov_description
                        }).ToList()
                    };

                    result.Add(portalUser);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<UserRoles>> GetContactRoles()
        {
            IQueryable<bcgov_portalrole> query = dynamicsContext.bcgov_portalroles;

            var roles = (await ((DataServiceQuery<bcgov_portalrole>)query).GetAllPagesAsync()).ToArray();

            var result = roles.Select(r =>
            {
                return new UserRoles
                {
                    Id = r.bcgov_portalroleid.ToString(),
                    Name = r.bcgov_name,
                    Description = r.bcgov_description
                };
            });
            return result;
        }
        public async Task AddContactRole(string roleId, string contactId, string modifiedBy)
        {
            var contactGuid = new Guid(contactId);
            var roleGuid = new Guid(roleId);

            var contact = dynamicsContext.contacts
                .Where(c => c.contactid == contactGuid)
                .Single();

            var portalRole = dynamicsContext.bcgov_portalroles
                .Where(c => c.bcgov_portalroleid == roleGuid)
                .Single();

            var roleAssignment = new bcgov_portalroleassignment
            {
                bcgov_portalroleassignmentid = Guid.NewGuid()
            };

            dynamicsContext.AddObject("bcgov_portalroleassignments", roleAssignment);
            dynamicsContext.SetLink(roleAssignment, "bcgov_Person", contact);    
            dynamicsContext.SetLink(roleAssignment, "bcgov_PortalRole", portalRole); 

            var auditDetail = new bcgov_portalauditdetails
            {
                bcgov_description = "User Assigned " + portalRole.bcgov_name + " Role",
                bcgov_entryid = modifiedBy
            };
            dynamicsContext.AddObject("bcgov_portalauditdetailses", auditDetail);
            dynamicsContext.SetLink(auditDetail, "bcgov_Person", contact);

            await dynamicsContext.SaveChangesAsync();
        }
        public async Task RemoveContactRole(Guid roleId, Guid contactId)
        {
            var contact = dynamicsContext.contacts
                .Expand(c => c.bcgov_contact_bcgov_portalroleassignment_Person)
                .Where(c => c.contactid == contactId)
                .FirstOrDefault();

            var roleAssignment = contact.bcgov_contact_bcgov_portalroleassignment_Person
                .FirstOrDefault(r => r._bcgov_portalrole_value == roleId);

            contact.bcgov_contact_bcgov_portalroleassignment_Person.Remove(roleAssignment);

            dynamicsContext.DeleteObject(roleAssignment);

            await dynamicsContext.SaveChangesAsync();
        }
        public async Task UpdateContact(PortalUser contact)
        {
            try
            {
                var contactToUpdate = dynamicsContext.contacts
                .Where(c => c.contactid == contact.Id)
                .FirstOrDefault();

                var contactIsUnauthorized = contactToUpdate.bcgov_approvalstatus == null || contactToUpdate.bcgov_approvalstatus == 931490000;

                if (contactToUpdate == null)
                {
                    throw new Exception("Contact not found.");
                }
                var auditDetails = await AddContactInfoAuditDetailsAsync(contactToUpdate, contact);

                contactToUpdate.bcgov_expirydate = contact.ExpiryDate;
                contactToUpdate.bcgov_effectivedate = contact.EffectiveDate;
                contactToUpdate.bcgov_approvalstatus = contact.Authorized == true ? 931490001 : 931490000;
                contactToUpdate.address1_line1 = contact.AddressLine1;
                contactToUpdate.address1_line2 = contact.AddressLine2;
                contactToUpdate.address1_line3 = contact.AddressLine3;
                contactToUpdate.address1_city = contact.City;
                contactToUpdate.address1_stateorprovince = contact.Province;
                contactToUpdate.address1_country = contact.Country;
                contactToUpdate.address1_postalcode = contact.PostCode;
                contactToUpdate.address1_telephone1 = contact.PhoneNumber;
                contactToUpdate.mobilephone = contact.CellNumber;
                contactToUpdate.emailaddress1 = contact.Email;
                contactToUpdate.firstname = contact.FirstName;
                contactToUpdate.middlename = contact.SecondGivenName;
                contactToUpdate.bcgov_thirdgivenname = contact.ThirdGivenName;
                contactToUpdate.lastname = contact.LastName;
                contactToUpdate.bcgov_dfwebuserid = contact.DFWebuserId;

                dynamicsContext.UpdateObject(contactToUpdate);
                await dynamicsContext.SaveChangesAsync();

                if (contactIsUnauthorized)
                {
                    IQueryable<bcgov_portalrole> query = dynamicsContext.bcgov_portalroles.Where(r => r.bcgov_roleid == "USER");

                    var userRole = (await ((DataServiceQuery<bcgov_portalrole>)query).GetAllPagesAsync()).FirstOrDefault();

                    await AddContactRole(userRole.bcgov_portalroleid.Value.ToString(), contact.Id.Value.ToString(), contact.ModifiedBy);

                    var auditDetail = new bcgov_portalauditdetails
                    {
                        bcgov_description = "User Role Assigned",
                        bcgov_entryid = contact.ModifiedBy

                    };
                    auditDetails.Add(auditDetail);

                }

                foreach (var auditDetail in auditDetails)
                {
                    dynamicsContext.AddObject("bcgov_portalauditdetailses", auditDetail);
                    dynamicsContext.SetLink(auditDetail, "bcgov_Person", contactToUpdate);
                }

                await dynamicsContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private async Task<List<bcgov_portalauditdetails>> AddContactInfoAuditDetailsAsync(contact contactToUpdate, PortalUser contact)
        {

            var result = new List<bcgov_portalauditdetails>();
            if (contactToUpdate.bcgov_approvalstatus == null || contactToUpdate.bcgov_approvalstatus == 931490000)
            {
                var auditDetail = new bcgov_portalauditdetails
                {
                    bcgov_description = "User Approved",
                    bcgov_entryid = contact.ModifiedBy

                };
                result.Add(auditDetail);
            }
            if (!CompareString(contactToUpdate.address1_line1, contact.AddressLine1) ||
                !CompareString(contactToUpdate.address1_line2, contact.AddressLine2) ||
                !CompareString(contactToUpdate.address1_line3, contact.AddressLine3) ||
                !CompareString(contactToUpdate.address1_city, contact.City) ||
                !CompareString(contactToUpdate.address1_stateorprovince, contact.Province) ||
                !CompareString(contactToUpdate.address1_country, contact.Country) ||
                !CompareString(contactToUpdate.address1_postalcode, contact.PostCode) ||
                !CompareString(contactToUpdate.address1_telephone1, contact.PhoneNumber) ||
                !CompareString(contactToUpdate.mobilephone, contact.CellNumber) ||
                !CompareString(contactToUpdate.emailaddress1, contact.Email))
            {
                var auditDetail = new bcgov_portalauditdetails
                {
                    bcgov_description = "User Address Updated",
                    bcgov_entryid = contact.ModifiedBy

                };
                result.Add(auditDetail);
            }

            if (!CompareString(contactToUpdate.firstname, contact.FirstName) ||
                !CompareString(contactToUpdate.middlename, contact.SecondGivenName) ||
                !CompareString(contactToUpdate.bcgov_thirdgivenname, contact.ThirdGivenName) ||
                !CompareString(contactToUpdate.lastname, contact.LastName))
            {
                var auditDetail = new bcgov_portalauditdetails
                {
                    bcgov_description = "User Info Updated",
                    bcgov_entryid = contact.ModifiedBy

                };
                result.Add(auditDetail);
            }

            if (contactToUpdate.bcgov_expirydate !=null && (contactToUpdate.bcgov_expirydate != contact.ExpiryDate))
            {
                var auditDetail = new bcgov_portalauditdetails
                {
                    bcgov_description = "User Expiry Date Updated",
                    bcgov_entryid = contact.ModifiedBy

                };
                result.Add(auditDetail);
            }

            return result;
        }
        public async Task<CurrentLoginUser> GetCurrentLoginUser(string userId)
        {
            var login = dynamicsContext.dfp_logins
                .Where(l => l.dfp_userid == userId)
                .FirstOrDefault();

            var roleQuery = dynamicsContext.bcgov_portalroleassignments
                       .Expand(r => r.bcgov_PortalRole)
                       .Expand(r => r.bcgov_Person)
                       .Where(r => r.bcgov_Person.contactid == login._dfp_person_value);
            var roleAssignments = (await ((DataServiceQuery<bcgov_portalroleassignment>)roleQuery).GetAllPagesAsync()).ToArray();

            return new CurrentLoginUser
            {
                UserRoles = roleAssignments.Select(r => r.bcgov_PortalRole?.bcgov_name).ToList()
            };
        }
        private string GetDomainName(int? domainId)
        {
            return domainId switch
            {
                100000000 => "BC Service Card",
                100000001 => "Buisness BCeID",
                100000002 => "Idir",
                100000003 => "One Health",
                100000004 => "MSEntra",
                null => null
            };
        }
        private bool CompareString(string a, string b)
        {

            return (a ?? "") == (b ?? "");
        }
    }
}

public enum Domain
{
    [Description("Pending Approval")]
    Pending = 1,

    [Description("Approved")]
    Approved = 2,

    [Description("Rejected")]
    Rejected = 3
}
