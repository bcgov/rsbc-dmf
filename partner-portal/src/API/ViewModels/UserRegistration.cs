using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels
{
    public class UserRegistration
    {
        public string GivenName { get; set; }
        public string? SecondGivenName { get; set; }
        public string? ThirdGivenName { get; set; }
        public string SurName { get; set; }
        public string AddressFirstLine { get; set; }
        public string? AddressSecondLine { get; set; }
        public string? AddressThirdLine { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CellPhoneNumber { get; set; }
        public string? EmailAddress { get; set; }

    }
}
