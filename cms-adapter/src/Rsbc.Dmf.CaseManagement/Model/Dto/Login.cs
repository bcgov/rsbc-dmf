using System;
using System.Collections.Generic;
using System.Text;

namespace Rsbc.Dmf.CaseManagement.Dto
{
    public class Login
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        //public LoginType
        public string UserId { get; set; }
        public Driver Driver { get; set; }
    }
}
