using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.PartnerPortal.Api.ViewModels
{
    public class UserSearch
    {
        public string SystemUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UnauthorizedOnly { get; set; }
        public string ActiveUser { get; set; }
    }
}
