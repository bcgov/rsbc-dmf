using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pssg.Interfaces.Models
{
    public class IcbcNotificationsFileResult
    {
        public Dictionary<string, IFormFile> NotificationFiles { get; set; }
    }
}
