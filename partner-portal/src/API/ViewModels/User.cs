using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels
{
    public class User
    {
        public string Id { get; set; }
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string SecondGivenName { get; set; }
        public string ThirdGivenName { get; set; }
        public string LastName { get; set; }
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
        public string DFWebuserId { get; set; }
        public bool Authorized { get; set; }
        public string UserName { get; set; }
        public List<UserRole> Roles { get; set; }
        public List<AuditDetail> AuditDetails { get; set; }
        public string modifiedUserId { get; set; }
    }


    public class UserRole
    {
        public string RoleID { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class UpdateContactRole
    {
        public string RoleId { get; set; }
        public string ContactId { get; set; }
        public bool AddRole { get; set; }
    }

    public class AuditDetail
    {
        public DateTimeOffset? EntryDate { get; set; }
        public string Description { get; set; }
        public string EntryId { get; set; }
    }
}

